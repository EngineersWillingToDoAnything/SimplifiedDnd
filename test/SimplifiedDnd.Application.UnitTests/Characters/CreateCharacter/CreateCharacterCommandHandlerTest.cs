using SimplifiedDnd.Application.Abstractions.Characters;
using SimplifiedDnd.Application.Abstractions.Core;
using SimplifiedDnd.Application.Characters;
using SimplifiedDnd.Application.Characters.CreateCharacter;
using SimplifiedDnd.Domain;
using SimplifiedDnd.Domain.Characters;

namespace SimplifiedDnd.Application.UnitTests.Characters.CreateCharacter;

public sealed class CreateCharacterCommandHandlerTest {
  private static CancellationToken TestContextToken => TestContext.Current.CancellationToken;

  private readonly CreateCharacterCommandHandler _handler;

  private readonly ICharacterRepository _characterRepository;
  private readonly ISpecieRepository _specieRepository;
  private readonly IClassRepository _classRepository;
  private readonly IUnitOfWork _unitOfWork;

  public CreateCharacterCommandHandlerTest() {
    _characterRepository = Substitute.For<ICharacterRepository>();
    _specieRepository = Substitute.For<ISpecieRepository>();
    _classRepository = Substitute.For<IClassRepository>();
    _unitOfWork = Substitute.For<IUnitOfWork>();

    _handler = new(
      _characterRepository,
      _specieRepository,
      _classRepository,
      _unitOfWork);
  }

  [Fact(DisplayName = "Returns error if character already exists")]
  public async Task HandlerReturnsErrorWithExistingCharacter() {
    // Arrange
    var command = new CreateCharacterCommand() {
      Name = "Batman",
      PlayerName = "Beta tester 1",
      SpecieName = "-",
      ClassName = "-",
    };

    _characterRepository
      .CheckCharacterExistsAsync(command.Name, command.PlayerName, TestContextToken)
      .Returns(true);

    // Act
    Result<Character> result = await _handler.Handle(command, TestContextToken);

    // Assert
    result.IsSuccess.Should().BeFalse();
    result.Error.Should().Be(CharacterError.AlreadyExists);
  }

  [Fact(DisplayName = "Returns error if specie was not found")]
  public async Task HandlerReturnsErrorWithNonExistingSpecie() {
    // Arrange
    var command = new CreateCharacterCommand() {
      Name = "-",
      PlayerName = "-",
      SpecieName = "-",
      ClassName = "-"
    };

    _characterRepository
      .CheckCharacterExistsAsync(command.Name, command.PlayerName, TestContextToken)
      .Returns(false);
    _specieRepository.GetSpecieAsync(command.SpecieName, TestContextToken)
      .Returns((Specie?)null);

    // Act
    Result<Character> result = await _handler.Handle(command, TestContextToken);

    // Assert
    result.IsSuccess.Should().BeFalse();
    result.Error.Should().Be(CharacterError.NonExistingSpecie);
  }

  [Fact(DisplayName = "Returns error if class was not found")]
  public async Task HandlerReturnsErrorWithNonExistingClass() {
    // Arrange
    var command = new CreateCharacterCommand() {
      Name = "-",
      PlayerName = "-",
      SpecieName = "-",
      ClassName = "-"
    };

    _characterRepository
      .CheckCharacterExistsAsync(command.Name, command.PlayerName, TestContextToken)
      .Returns(false);
    _specieRepository.GetSpecieAsync(command.SpecieName, TestContextToken)
      .Returns(new Specie {
        Name = "Elf",
        Size = Size.Small,
        Speed = 0
      });

    _classRepository.GetClassAsync(command.ClassName, TestContextToken)
      .Returns((DndClass?)null);

    // Act
    Result<Character> result = await _handler.Handle(command, TestContextToken);

    // Assert
    result.IsSuccess.Should().BeFalse();
    result.Error.Should().Be(CharacterError.NonExistingClass);
  }

  [Fact(DisplayName = "Returns character with given attributes")]
  public async Task HandlerReturnsCharacterWithGivenAttributes() {
    // Arrange
    var command = new CreateCharacterCommand() {
      Name = "Test",
      PlayerName = "Test",
      SpecieName = "human",
      ClassName = "Bardo"
    };

    _characterRepository
      .CheckCharacterExistsAsync(command.Name, command.PlayerName, TestContextToken)
      .Returns(false);

    var specie = new Specie {
      Name = "human",
      Size = Size.Tiny,
      Speed = 1,
    };
    _specieRepository.GetSpecieAsync(command.SpecieName, TestContextToken)
      .Returns(specie);
    
    var mainClass = new DndClass {
      Name = "Bardo",
      Level = Level.MaxLevel
    };
    _classRepository.GetClassAsync(command.ClassName, TestContextToken)
      .Returns(mainClass);

    // Act
    Result<Character> result = await _handler.Handle(command, TestContextToken);

    // Assert
    result.IsSuccess.Should().BeTrue();
    result.Value.Name.Should().Be(command.Name);
    result.Value.PlayerName.Should().Be(command.PlayerName);
    result.Value.Specie.Should().BeEquivalentTo(specie);
    result.Value.MainClass.Should().BeEquivalentTo(mainClass);
  }

  [Fact(DisplayName = "Returns character with guid version 7")]
  public async Task HandlerReturnsCharacterWithGuidV7() {
    // Arrange
    var command = new CreateCharacterCommand() {
      Name = "Test",
      PlayerName = "Test",
      SpecieName = "human",
      ClassName = "Bardo",
    };

    _characterRepository
      .CheckCharacterExistsAsync(command.Name, command.PlayerName, TestContextToken)
      .Returns(false);

    _specieRepository.GetSpecieAsync(command.SpecieName, TestContextToken)
      .Returns(new Specie {
        Name = "human",
        Size = Size.Tiny,
        Speed = 1,
      });
    
    _classRepository.GetClassAsync(command.ClassName, TestContextToken)
      .Returns(new DndClass {
        Name = "Bardo",
        Level = Level.MaxLevel
      });

    // Act
    Result<Character> result = await _handler.Handle(command, TestContextToken);

    // Assert
    result.IsSuccess.Should().BeTrue();
    result.Value.Id.Version.Should().Be(7);
  }

  [Fact(DisplayName = "Saves character to repository")]
  public async Task HandlerSavesCharacterToRepository() {
    // Arrange
    var command = new CreateCharacterCommand() {
      Name = "Test",
      PlayerName = "Test",
      SpecieName = "human",
      ClassName = "Bardo",
    };

    _characterRepository
      .CheckCharacterExistsAsync(command.Name, command.PlayerName, TestContextToken)
      .Returns(false);

    _specieRepository.GetSpecieAsync(command.SpecieName, TestContextToken)
      .Returns(new Specie {
        Name = "human",
        Size = Size.Tiny,
        Speed = 1,
      });
    
    _classRepository.GetClassAsync(command.ClassName, TestContextToken)
      .Returns(new DndClass {
        Name = "Bardo",
        Level = Level.MaxLevel
      });

    // Act
    await _handler.Handle(command, TestContextToken);

    // Assert
    _characterRepository.Received(1)
      .SaveCharacter(Arg.Any<Character>());
  }

  [Fact(DisplayName = "Handler saves changes in one iteration (unit of work)")]
  public async Task HandlerSavesChangesInOneIteration() {
    // Arrange
    var command = new CreateCharacterCommand() {
      Name = "Test",
      PlayerName = "Test",
      SpecieName = "human",
      ClassName = "Bardo",
    };

    _characterRepository
      .CheckCharacterExistsAsync(command.Name, command.PlayerName, TestContextToken)
      .Returns(false);

    _specieRepository.GetSpecieAsync(command.SpecieName, TestContextToken)
      .Returns(new Specie {
        Name = "human",
        Size = Size.Tiny,
        Speed = 1,
      });

    _classRepository.GetClassAsync(command.ClassName, TestContextToken)
      .Returns(new DndClass {
        Name = "Bardo",
        Level = Level.MaxLevel
      });
    
    // Act
    await _handler.Handle(command, TestContextToken);

    // Assert
    await _unitOfWork.Received(1)
      .SaveChangesAsync(TestContextToken);
  }
}