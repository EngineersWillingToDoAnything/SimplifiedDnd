using System.Globalization;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using SimplifiedDnd.Application.Abstractions.Characters;
using SimplifiedDnd.Application.Abstractions.Core;
using SimplifiedDnd.Application.Abstractions.Queries;
using SimplifiedDnd.Application.Characters.GetCharacters;
using SimplifiedDnd.Database.IntegrationTests.Abstractions;
using SimplifiedDnd.DataBase.Entities;
using SimplifiedDnd.Domain.Characters;

namespace SimplifiedDnd.Database.IntegrationTests.Characters;

public class GetCharactersQueryHandlerTests(
  IntegrationTestWebAppFactory factory
) : BaseIntegrationTest(factory) {
  private static CancellationToken TestContextToken => TestContext.Current.CancellationToken;

  [Fact(
    DisplayName = "Returns all characters with empty request",
    Explicit = false)]
  public async Task HandlerReturnsAllCharactersWithEmptyRequest() {
    // Arrange
    var request = new GetCharactersQuery();

    // Act
    Result<PaginatedResult<Character>> result = await Sender.Send(
      request, TestContextToken);

    // Assert
    List<Guid> dbCharacters = await DbContext.Characters
      .Select(c => c.Id)
      .ToListAsync(TestContextToken);

    IReadOnlyCollection<Character> characters = result.Value.Values;

    characters.Should().HaveCount(dbCharacters.Count);
    characters.Select(c => c.Id).Should().BeEquivalentTo(dbCharacters);
  }

  [Fact(
    DisplayName = "Returns exact amount of characters",
    Explicit = false)]
  public async Task HandlerReturnsExactAmountOfCharacters() {
    // Arrange
    int pageSize = await DbContext.Characters.CountAsync(TestContextToken) - 1;
    var request = new GetCharactersQuery {
      Page = new Page(0, pageSize)
    };

    // Act
    Result<PaginatedResult<Character>> result = await Sender.Send(
      request, TestContextToken);

    // Assert
    IReadOnlyCollection<Character> characters = result.Value.Values;

    characters.Should().HaveCount(pageSize);
  }

  [Fact(
    DisplayName = "Returns less characters if index * size + size is greater than pages",
    Explicit = false)]
  public async Task HandlerReturnsLessCharactersIfIndexTimesSizePlusSizeIsGreaterThanPages() {
    // Arrange
    int pageSize = await DbContext.Characters.CountAsync(TestContextToken) - 1;
    var request = new GetCharactersQuery {
      Page = new Page(1, pageSize)
    };

    // Act
    Result<PaginatedResult<Character>> result = await Sender.Send(
      request, TestContextToken);

    // Assert
    IReadOnlyCollection<Character> characters = result.Value.Values;

    characters.Should().HaveCountLessThanOrEqualTo(request.Page.Size);
  }

  private class ValidOrderKeys() : TheoryData<string>(
    nameof(Character.Id),
    nameof(Character.Name),
    nameof(Character.MainClass),
    nameof(Character.Specie)
  );

  [Theory(DisplayName = "Orders by specific key",
  Explicit = false)]
  [ClassData(typeof(ValidOrderKeys),
  Explicit = false)]
  public async Task HandlerOrdersBySpecificKey(string orderKey) {
    // Arrange
    var request = new GetCharactersQuery {
      Order = Order.CreateAscending(orderKey)
    };

    // Act
    Result<PaginatedResult<Character>> result = await Sender.Send(
      request, TestContextToken);

    // Assert
    IReadOnlyCollection<Character> characters = result.Value.Values;
    List<Guid> dbCharactersId = await OrderQuery(DbContext.Characters, orderKey)
      .Select(c => c.Id)
      .ToListAsync(TestContextToken);

    characters.Select(c => c.Id).Should().BeEquivalentTo(dbCharactersId);

    static IOrderedQueryable<CharacterDbEntity> OrderQuery(
      IQueryable<CharacterDbEntity> queryable, string key
    ) {
      TextInfo textInfo = CultureInfo.InvariantCulture.TextInfo;

      return textInfo.ToTitleCase(key) switch {
        nameof(Character.Name) => queryable.OrderBy(c => c.Name),
        nameof(Character.Specie) => queryable.OrderBy(c => c.Specie!.Name),
        nameof(Character.MainClass) => queryable.OrderBy(c => c.CharacterClasses.Single(cc => cc.IsMainClass).Class!.Name),
        _ => queryable.OrderBy(c => c.Id)
      };
    }
  }

  [Fact(
    DisplayName = "Orders by Name",
    Explicit = false)]
  public async Task HandlerOrdersByName() {
    // Arrange
    const string orderKey = nameof(Character.Name);
    var request = new GetCharactersQuery {
      Order = Order.CreateAscending(orderKey)
    };

    // Act
    Result<PaginatedResult<Character>> result = await Sender.Send(request, TestContextToken);

    // Assert
    IReadOnlyCollection<Character> characters = result.Value.Values;
    List<Guid> dbCharactersId = await DbContext.Characters
      .OrderBy(c => c.Name)
      .Select(c => c.Id)
      .ToListAsync(TestContextToken);

    characters.Select(c => c.Id).Should().BeEquivalentTo(dbCharactersId);
  }

  [Fact(
    DisplayName = "Orders by Specie",
    Explicit = false)]
  public async Task HandlerOrdersBySpecie() {
    // Arrange
    const string orderKey = nameof(Character.Specie);
    var request = new GetCharactersQuery {
      Order = Order.CreateAscending(orderKey)
    };

    // Act
    Result<PaginatedResult<Character>> result = await Sender.Send(request, TestContextToken);

    // Assert
    IReadOnlyCollection<Character> characters = result.Value.Values;
    List<Guid> dbCharactersId = await DbContext.Characters
      .Include(c => c.Specie)
      .OrderBy(c => c.Specie!.Name)
      .Select(c => c.Id)
      .ToListAsync(TestContextToken);

    characters.Select(c => c.Id).Should().BeEquivalentTo(dbCharactersId);
  }

  [Fact(
    DisplayName = "Orders by MainClass",
    Explicit = false)]
  public async Task HandlerOrdersByMainClass() {
    // Arrange
    const string orderKey = nameof(Character.MainClass);
    var request = new GetCharactersQuery {
      Order = Order.CreateAscending(orderKey)
    };

    // Act
    Result<PaginatedResult<Character>> result = await Sender.Send(request, TestContextToken);

    // Assert
    IReadOnlyCollection<Character> characters = result.Value.Values;
    List<Guid> dbCharactersId = await DbContext.Characters
      .Include(c => c.CharacterClasses)
      .ThenInclude(cc => cc.Class)
      .OrderBy(c => c.CharacterClasses.Single(cc => cc.IsMainClass).Class!.Name)
      .Select(c => c.Id)
      .ToListAsync(TestContextToken);

    characters.Select(c => c.Id).Should().BeEquivalentTo(dbCharactersId);
  }

  [Fact(
    DisplayName = "Orders by character id",
    Explicit = false)]
  public async Task HandlerOrdersById() {
    // Arrange
    const string orderKey = nameof(Character.Id);
    var request = new GetCharactersQuery {
      Order = Order.CreateAscending(orderKey)
    };

    // Act
    Result<PaginatedResult<Character>> result = await Sender.Send(request, TestContextToken);

    // Assert
    IReadOnlyCollection<Character> characters = result.Value.Values;
    List<Guid> dbCharactersId = await DbContext.Characters
      .OrderBy(c => c.Id)
      .Select(c => c.Id)
      .ToListAsync(TestContextToken);

    characters.Select(c => c.Id).Should().BeEquivalentTo(dbCharactersId);
  }

  [Fact(
    DisplayName = "Orders by Name Descending",
    Explicit = false)]
  public async Task HandlerOrdersByNameDescending() {
    // Arrange
    const string orderKey = nameof(Character.Name);
    var request = new GetCharactersQuery {
      Order = Order.CreateDescending(orderKey)
    };

    // Act
    Result<PaginatedResult<Character>> result = await Sender.Send(request, TestContextToken);

    // Assert
    IReadOnlyCollection<Character> characters = result.Value.Values;
    List<Guid> dbCharactersId = await DbContext.Characters
      .OrderByDescending(c => c.Name)
      .Select(c => c.Id)
      .ToListAsync(TestContextToken);

    characters.Select(c => c.Id).Should().BeEquivalentTo(dbCharactersId);
  }

  [Fact(
    DisplayName = "Orders by Specie Descending",
    Explicit = false)]
  public async Task HandlerOrdersBySpecieDescending() {
    // Arrange
    const string orderKey = nameof(Character.Specie);
    var request = new GetCharactersQuery {
      Order = Order.CreateDescending(orderKey)
    };

    // Act
    Result<PaginatedResult<Character>> result = await Sender.Send(request, TestContextToken);

    // Assert
    IReadOnlyCollection<Character> characters = result.Value.Values;
    List<Guid> dbCharactersId = await DbContext.Characters
      .Include(c => c.Specie)
      .OrderByDescending(c => c.Specie!.Name)
      .Select(c => c.Id)
      .ToListAsync(TestContextToken);

    characters.Select(c => c.Id).Should().BeEquivalentTo(dbCharactersId);
  }

  [Fact(
    DisplayName = "Orders by MainClass Descending",
    Explicit = false)]
  public async Task HandlerOrdersByMainClassDescending() {
    // Arrange
    const string orderKey = nameof(Character.MainClass);
    var request = new GetCharactersQuery {
      Order = Order.CreateDescending(orderKey)
    };

    // Act
    Result<PaginatedResult<Character>> result = await Sender.Send(request, TestContextToken);

    // Assert
    IReadOnlyCollection<Character> characters = result.Value.Values;
    List<Guid> dbCharactersId = await DbContext.Characters
      .Include(c => c.CharacterClasses)
      .ThenInclude(cc => cc.Class)
      .OrderByDescending(c => c.CharacterClasses.Single(cc => cc.IsMainClass).Class!.Name)
      .Select(c => c.Id)
      .ToListAsync(TestContextToken);

    characters.Select(c => c.Id).Should().BeEquivalentTo(dbCharactersId);
  }

  [Fact(
    DisplayName = "Orders by Id Descending",
    Explicit = false)]
  public async Task HandlerOrdersByIdDescending() {
    // Arrange
    const string orderKey = nameof(Character.Id);
    var request = new GetCharactersQuery {
      Order = Order.CreateDescending(orderKey)
    };

    // Act
    Result<PaginatedResult<Character>> result = await Sender.Send(request, TestContextToken);

    // Assert
    IReadOnlyCollection<Character> characters = result.Value.Values;
    List<Guid> dbCharactersId = await DbContext.Characters
      .OrderByDescending(c => c.Id)
      .Select(c => c.Id)
      .ToListAsync(TestContextToken);

    characters.Select(c => c.Id).Should().BeEquivalentTo(dbCharactersId);
  }

  [Fact(
    DisplayName = "Filters by part of the name",
    Explicit = false)]
  public async Task HandlerFiltersByPartOfTheName() {
    // Arrange
    string partOfName = "Test";
    var request = new GetCharactersQuery {
      Filter = new CharacterFilter { Name = partOfName }
    };

    // Act
    Result<PaginatedResult<Character>> result = await Sender.Send(
      request, TestContextToken);

    // Assert
    IReadOnlyCollection<Character> characters = result.Value.Values;
    List<Guid> dbCharactersId = await DbContext.Characters
      .Where(c => c.Name.Contains(partOfName))
      .Select(c => c.Id)
      .ToListAsync(TestContextToken);

    characters.Select(c => c.Id).Should().BeEquivalentTo(dbCharactersId);
  }

  [Fact(
    DisplayName = "Filters by part of the name, matching ignoring case",
    Explicit = false)]
  public async Task HandlerFiltersByPartOfTheNameMatchingIgnoringCase() {
    // Arrange
    const string partOfName = "tESt";
    var request = new GetCharactersQuery {
      Filter = new CharacterFilter { Name = partOfName }
    };

    // Act
    Result<PaginatedResult<Character>> result = await Sender.Send(
      request, TestContextToken);

    // Assert
    IReadOnlyCollection<Character> characters = result.Value.Values;
    List<Guid> dbCharactersId = await DbContext.Characters
#pragma warning disable CA1304, CA1311, CA1862
      .Where(c => c.Name.ToUpper().Contains(partOfName.ToUpper()))
#pragma warning restore CA1862, CA1311, CA1304
      .Select(c => c.Id)
      .ToListAsync(TestContextToken);

    characters.Select(c => c.Id).Should().BeEquivalentTo(dbCharactersId);
  }

  [Fact(
    DisplayName = "Filters by the main class name being in set",
    Explicit = false)]
  public async Task HandlerFiltersByMainClassNameInSet() {
    // Arrange
    string[] classNames = ["Artificer", "Bard"];
    var request = new GetCharactersQuery {
      Filter = new CharacterFilter { Classes = classNames }
    };

    // Act
    Result<PaginatedResult<Character>> result = await Sender.Send(
      request, TestContextToken);

    // Assert
    IReadOnlyCollection<Character> characters = result.Value.Values;
    List<Guid> dbCharactersId = await DbContext.Characters
      .Where(c => c.CharacterClasses.Any(cc =>
        cc.IsMainClass && classNames.Contains(cc.Class!.Name)))
      .Select(c => c.Id)
      .ToListAsync(TestContextToken);

    characters.Select(c => c.Id).Should().BeEquivalentTo(dbCharactersId);
  }

  [Fact(
    DisplayName = "Filters by the main class name being in set, matching ignoring case",
    Explicit = false)]
  public async Task HandlerFiltersByMainClassNameInSetMatchingIgnoringCase() {
    // Arrange
    string[] classNames = ["Warrior", "Mage"];
    var request = new GetCharactersQuery {
      Filter = new CharacterFilter { Classes = classNames }
    };

    // Act
    Result<PaginatedResult<Character>> result = await Sender.Send(
      request, TestContextToken);

    // Assert
    IReadOnlyCollection<Character> characters = result.Value.Values;
    List<Guid> dbCharactersId = await DbContext.Characters
#pragma warning disable CA1304, CA1311, CA1862
      .Where(c => c.CharacterClasses.Any(cc =>
        cc.IsMainClass && classNames
          .Select(name => name.ToUpper())
          .Contains(cc.Class!.Name.ToUpper())))
#pragma warning restore CA1304, CA1311, CA1862
      .Select(c => c.Id)
      .ToListAsync(TestContextToken);
    characters.Select(c => c.Id).Should().BeEquivalentTo(dbCharactersId);
  }

  [Fact(
    DisplayName = "Filters by the specie name being in set",
    Explicit = false)]
  public async Task HandlerFiltersBySpecieNameInSet() {
    // Arrange
    string[] specieNames = ["Elf", "Dwarf"];
    var request = new GetCharactersQuery {
      Filter = new CharacterFilter { Species = specieNames }
    };

    // Act
    Result<PaginatedResult<Character>> result = await Sender.Send(
      request, TestContextToken);

    // Assert
    IReadOnlyCollection<Character> characters = result.Value.Values;
    List<Guid> dbCharactersId = await DbContext.Characters
      .Where(c => specieNames.Contains(c.Specie!.Name))
      .Select(c => c.Id)
      .ToListAsync(TestContextToken);

    characters.Select(c => c.Id).Should().BeEquivalentTo(dbCharactersId);
  }

  [Fact(
    DisplayName = "Filters by the specie name being in set, matching ignoring case",
    Explicit = false)]
  public async Task HandlerFiltersBySpecieNameInSetMatchingIgnoringCase() {
    // Arrange
    string[] specieNames = ["Elf", "Dwarf"];
    var request = new GetCharactersQuery {
      Filter = new CharacterFilter { Species = specieNames }
    };

    // Act
    Result<PaginatedResult<Character>> result = await Sender.Send(
      request, TestContextToken);

    // Assert
    IReadOnlyCollection<Character> characters = result.Value.Values;
    List<Guid> dbCharactersId = await DbContext.Characters
#pragma warning disable CA1304, CA1311, CA1862
      .Where(c => specieNames
        .Select(name => name.ToUpper())
        .Contains(c.Specie!.Name.ToUpper()))
#pragma warning restore CA1304, CA1311, CA1862
      .Select(c => c.Id)
      .ToListAsync(TestContextToken);

    characters.Select(c => c.Id).Should().BeEquivalentTo(dbCharactersId);
  }
}