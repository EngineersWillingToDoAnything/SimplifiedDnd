using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using SimplifiedDnd.Application.Abstractions.Core;
using SimplifiedDnd.Application.Characters.CreateCharacter;
using SimplifiedDnd.DataBase.Entities;
using SimplifiedDnd.Database.IntegrationTests.Abstractions;
using SimplifiedDnd.Domain.Characters;

namespace SimplifiedDnd.Database.IntegrationTests.Characters;

public class CreateCharacterCommandHandlerTests(
  IntegrationTestWebAppFactory factory
) : BaseIntegrationTest(factory) {
  private static CancellationToken TestContextToken => TestContext.Current.CancellationToken;

  [Fact(
    DisplayName = "Creates a new character",
    Explicit = true)]
  public async Task HandlerCreatesCharacter() {
    // Arrange
    var command = new CreateCharacterCommand {
      Name = $"{Guid.CreateVersion7()}",
      PlayerName = "...",
      Classes = [
        new DndClass {
          Name = await GetClassNameFromDb(),
          Level = Level.MinLevel
        }
      ],
      SpecieName = await GetSpecieNameFromDb(),
    };

    // Act
    Result<Guid> productId = await Sender.Send(command, TestContextToken);

    // Assert
    CharacterDbEntity? character = await DbContext.Characters
      .SingleOrDefaultAsync(
        c => c.Id == productId.Value,
        TestContextToken);

    character.Should().NotBeNull();
  }

  [Fact(
    DisplayName = "Assigns a class to the character for each given class",
    Explicit = true)]
  public async Task HandlerAssignsClassToCharacterForEveryClass() {
    // Arrange
    var command = new CreateCharacterCommand {
      Name = $"{Guid.CreateVersion7()}",
      PlayerName = "...",
      Classes = [
        new DndClass {
          Name = await GetClassNameFromDb(),
          Level = Level.MinLevel
        },
        new DndClass {
          Name = await GetClassNameFromDb(1),
          Level = Level.MinLevel
        }
      ],
      SpecieName = await GetSpecieNameFromDb(),
    };

    // Act
    Result<Guid> characterId = await Sender.Send(command, TestContextToken);

    // Assert
    List<string> characterClasses = await DbContext.CharacterClasses
      .Where(cc => cc.CharacterId == characterId.Value)
      .Include(cc => cc.Class)
      .Select(cc => cc.Class!.Name)
      .ToListAsync(TestContextToken);

    characterClasses.Should().HaveCount(command.Classes.Count)
      .And.Contain(command.Classes.Select(c => c.Name));
  }

  [Fact(
    DisplayName = "Assigns first class as main class",
    Explicit = true)]
  public async Task HandlerAssignsFirstClassAsMainClass() {
    // Arrange
    string className = await GetClassNameFromDb();
    var command = new CreateCharacterCommand {
      Name = $"{Guid.CreateVersion7()}",
      PlayerName = "...",
      Classes = [
        new DndClass {
          Name = className,
          Level = Level.MinLevel
        }
      ],
      SpecieName = await GetSpecieNameFromDb(),
    };

    // Act
    Result<Guid> characterId = await Sender.Send(command, TestContextToken);

    // Assert
    CharacterClassDbEntity? characterClass = await DbContext.CharacterClasses
      .Include(cc => cc.Class)
      .SingleOrDefaultAsync(
        cc => cc.CharacterId == characterId.Value &&
              cc.Class!.Name == className,
        TestContextToken);

    characterClass.Should().NotBeNull();
    characterClass.IsMainClass.Should().BeTrue();
  }

  [Fact(
    DisplayName = "Assigns other classes not as main class",
    Explicit = true)]
  public async Task HandlerAssignsOtherClassesNotAsMainClass() {
    // Arrange
    var command = new CreateCharacterCommand {
      Name = $"{Guid.CreateVersion7()}",
      PlayerName = "...",
      Classes = [
        new DndClass {
          Name = await GetClassNameFromDb(),
          Level = Level.MinLevel
        },
        new DndClass {
          Name = await GetClassNameFromDb(1),
          Level = Level.MinLevel
        },
        new DndClass {
          Name = await GetClassNameFromDb(2),
          Level = Level.MinLevel
        }
      ],
      SpecieName = await GetSpecieNameFromDb(),
    };

    // Act
    Result<Guid> characterId = await Sender.Send(command, TestContextToken);

    // Assert
    List<string> characterClasses = await DbContext
      .CharacterClasses
      .Where(cc => cc.CharacterId == characterId.Value &&
                   !cc.IsMainClass)
      .Include(cc => cc.Class)
      .Select(cc => cc.Class!.Name)
      .ToListAsync(TestContextToken);

    characterClasses.Should().HaveCount(command.Classes.Count - 1)
      .And.Contain(command.Classes.Skip(1).Select(c => c.Name));
  }

  [Fact(
    DisplayName = "Assigns the specie to the character",
    Explicit = true)]
  public async Task HandlerAssignsSpecieToCharacter() {
    // Arrange
    var command = new CreateCharacterCommand {
      Name = $"{Guid.CreateVersion7()}",
      PlayerName = "...",
      Classes = [
        new DndClass { Name = await GetClassNameFromDb() }
      ],
      SpecieName = await GetSpecieNameFromDb(),
    };

    // Act
    Result<Guid> characterId = await Sender.Send(command, TestContextToken);

    // Assert
    bool characterIsAssociatedToSpecie = await DbContext.Characters
      .Include(c => c.Specie)
      .AnyAsync(c =>
          c.Id == characterId.Value &&
          c.Specie!.Name == command.SpecieName,
        TestContextToken);

    characterIsAssociatedToSpecie.Should().BeTrue();
  }

  private async Task<string> GetClassNameFromDb(int skipAmount = 0) {
    return await DbContext.Classes.Skip(skipAmount)
      .OrderBy(c => c.Id)
      .Select(c => c.Name)
      .FirstAsync(TestContextToken);
  }

  private async Task<string> GetSpecieNameFromDb() {
    return await DbContext.Species
      .Select(c => c.Name)
      .FirstAsync(TestContextToken);
  }
}