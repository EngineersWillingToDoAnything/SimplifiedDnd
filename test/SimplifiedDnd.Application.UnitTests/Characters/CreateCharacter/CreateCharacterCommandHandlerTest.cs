using SimplifiedDnd.Application.Abstractions.Characters;
using SimplifiedDnd.Application.Abstractions.Core;
using SimplifiedDnd.Application.Characters;
using SimplifiedDnd.Application.Characters.CreateCharacter;
using SimplifiedDnd.Domain;
using SimplifiedDnd.Domain.Characters;

namespace SimplifiedDnd.Application.UnitTests.Characters.CreateCharacter;

public class CreateCharacterCommandHandlerTest {
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

    _handler = new CreateCharacterCommandHandler(
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
      Classes = [new DndClass { Name = "-" }],
    };

    _characterRepository
      .CheckCharacterExistsAsync(command.Name, command.PlayerName, TestContextToken)
      .Returns(true);

    // Act
    Result<Guid> result = await _handler.Handle(command, TestContextToken);

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
      Classes = [new DndClass { Name = "-" }],
    };

    _characterRepository
      .CheckCharacterExistsAsync(Arg.Any<string>(), Arg.Any<string>(), TestContextToken)
      .Returns(false);
    _specieRepository.GetSpecieAsync(command.SpecieName, TestContextToken)
      .Returns((Specie?)null);

    // Act
    Result<Guid> result = await _handler.Handle(command, TestContextToken);

    // Assert
    result.IsSuccess.Should().BeFalse();
    result.Error.Should().Be(CharacterError.NonExistingSpecie);
  }

  [Fact(DisplayName = "Returns error if class was not found")]
  public async Task HandlerReturnsErrorWithNonExistingClass() {
    // Arrange
    var command = new CreateCharacterCommand {
      Name = "-",
      PlayerName = "-",
      SpecieName = "-",
      Classes = [new DndClass { Name = "-" }],
    };

    _characterRepository
      .CheckCharacterExistsAsync(Arg.Any<string>(), Arg.Any<string>(), TestContextToken)
      .Returns(false);
    _specieRepository.GetSpecieAsync(Arg.Any<string>(), TestContextToken)
      .Returns(new Specie { Name = "-", Size = Size.Small, Speed = 0 });
    _classRepository.CheckClassExistsAsync(Arg.Any<string>(), TestContextToken)
      .Returns(false);

    // Act
    Result<Guid> result = await _handler.Handle(command, TestContextToken);

    // Assert
    result.Error.Should().Be(CharacterError.NonExistingClass);
  }

  [Fact(DisplayName = "Returns error if a class was found but another class was not")]
  public async Task HandlerReturnsErrorWithAnyNonExistingClass() {
    // Arrange
    var command = new CreateCharacterCommand {
      Name = "-",
      PlayerName = "-",
      SpecieName = "-",
      Classes = [
        new DndClass { Name = "Bard" },
        new DndClass { Name = "-" }
      ],
    };

    _characterRepository
      .CheckCharacterExistsAsync(Arg.Any<string>(), Arg.Any<string>(), TestContextToken)
      .Returns(false);
    _specieRepository.GetSpecieAsync(Arg.Any<string>(), TestContextToken)
      .Returns(new Specie { Name = "-", Size = Size.Small, Speed = 0 });
    _classRepository.CheckClassExistsAsync(Arg.Any<string>(), TestContextToken)
      .Returns(true, false);

    // Act
    Result<Guid> result = await _handler.Handle(command, TestContextToken);

    // Assert
    result.IsSuccess.Should().BeFalse();
    result.Error.Should().Be(CharacterError.NonExistingClass);
  }

  [Fact(DisplayName = "Saves character with given name")]
  public async Task HandlerSavesCharacterWithGivenName() {
    // Arrange
    var command = new CreateCharacterCommand {
      Name = "Test",
      PlayerName = "-",
      SpecieName = "-",
      Classes = [new DndClass { Name = "-" }]
    };

    _characterRepository
      .CheckCharacterExistsAsync(Arg.Any<string>(), Arg.Any<string>(), TestContextToken)
      .Returns(false);
    _specieRepository.GetSpecieAsync(Arg.Any<string>(), TestContextToken)
      .Returns(new Specie { Name = "-", Size = Size.Tiny, Speed = 1, });
    _classRepository.CheckClassExistsAsync(Arg.Any<string>(), TestContextToken)
      .Returns(true);

    // Act
    await _handler.Handle(command, TestContextToken);

    // Assert
    _characterRepository.Received(1)
      .SaveCharacter(Arg.Is<Character>(c => c.Name == command.Name));
  }

  [Fact(DisplayName = "Saves character with given player name")]
  public async Task HandlerSavesCharacterWithGivenPlayerName() {
    // Arrange
    var command = new CreateCharacterCommand {
      Name = "-",
      PlayerName = "Test",
      SpecieName = "-",
      Classes = [new DndClass { Name = "-" }]
    };

    _characterRepository
      .CheckCharacterExistsAsync(Arg.Any<string>(), Arg.Any<string>(), TestContextToken)
      .Returns(false);
    _specieRepository.GetSpecieAsync(Arg.Any<string>(), TestContextToken)
      .Returns(new Specie { Name = "-", Size = Size.Tiny, Speed = 1, });
    _classRepository.CheckClassExistsAsync(Arg.Any<string>(), TestContextToken)
      .Returns(true);

    // Act
    await _handler.Handle(command, TestContextToken);

    // Assert
    _characterRepository.Received(1)
      .SaveCharacter(Arg.Is<Character>(c => c.PlayerName == command.PlayerName));
  }

  [Fact(DisplayName = "Saves character with given specie")]
  public async Task HandlerSavesCharacterWithGivenSpecie() {
    // Arrange
    var command = new CreateCharacterCommand {
      Name = "-",
      PlayerName = "-",
      SpecieName = "human",
      Classes = [new DndClass { Name = "-" }]
    };
    var expectedSpecie = new Specie {
      Name = "human",
      Size = Size.Tiny,
      Speed = 1,
    };

    _characterRepository
      .CheckCharacterExistsAsync(Arg.Any<string>(), Arg.Any<string>(), TestContextToken)
      .Returns(false);
    _specieRepository.GetSpecieAsync(Arg.Any<string>(), TestContextToken)
      .Returns(expectedSpecie);
    _classRepository.CheckClassExistsAsync(Arg.Any<string>(), TestContextToken)
      .Returns(true);

    // Act
    await _handler.Handle(command, TestContextToken);

    // Assert
    _characterRepository.Received(1)
      .SaveCharacter(Arg.Is<Character>(c => c.Specie == expectedSpecie));
  }

  [Fact(DisplayName = "Saves character with main class as first class")]
  public async Task HandlerSavesCharacterWithMainClassFromFirstClass() {
    // Arrange
    var command = new CreateCharacterCommand {
      Name = "-",
      PlayerName = "-",
      SpecieName = "-",
      Classes = [new DndClass { Name = "Bard" }]
    };

    _characterRepository
      .CheckCharacterExistsAsync(Arg.Any<string>(), Arg.Any<string>(), TestContextToken)
      .Returns(false);
    _specieRepository.GetSpecieAsync(Arg.Any<string>(), TestContextToken)
      .Returns(new Specie { Name = "-", Size = Size.Tiny, Speed = 1, });
    _classRepository.CheckClassExistsAsync(Arg.Any<string>(), TestContextToken)
      .Returns(true);

    // Act
    await _handler.Handle(command, TestContextToken);

    // Assert
    _characterRepository.Received(1)
      .SaveCharacter(Arg.Is<Character>(c => c.MainClass == command.Classes.First()));
  }

  [Fact(DisplayName = "Saves character with multi class from extra classes")]
  public async Task HandlerSavesCharacterWithClassesFromRestOfClasses() {
    // Arrange
    var command = new CreateCharacterCommand() {
      Name = "-",
      PlayerName = "-",
      SpecieName = "-",
      Classes = [
        new DndClass { Name = "-" },
        new DndClass { Name = "Barbarian" },
        new DndClass { Name = "Bard" }
      ]
    };
    IEnumerable<DndClass> expectedClasses = command.Classes.Skip(1);

    _characterRepository
      .CheckCharacterExistsAsync(Arg.Any<string>(), Arg.Any<string>(), TestContextToken)
      .Returns(false);
    _specieRepository.GetSpecieAsync(Arg.Any<string>(), TestContextToken)
      .Returns(new Specie { Name = "-", Size = Size.Tiny, Speed = 1, });
    _classRepository.CheckClassExistsAsync(Arg.Any<string>(), TestContextToken)
      .Returns(true, true, true);

    // Act
    await _handler.Handle(command, TestContextToken);

    // Assert
    _characterRepository.Received(1)
      .SaveCharacter(Arg.Is<Character>(character =>
        character.Classes.All(c => expectedClasses.Contains(c)) &&
        expectedClasses.All(c => character.Classes.Contains(c))));
  }

  [Fact(DisplayName = "Returns guid version 7 of the character")]
  public async Task HandlerReturnsGuidV7OfTheCharacter() {
    // Arrange
    var command = new CreateCharacterCommand {
      Name = "-",
      PlayerName = "-",
      SpecieName = "-",
      Classes = [new DndClass { Name = "-" }]
    };

    _characterRepository
      .CheckCharacterExistsAsync(command.Name, command.PlayerName, TestContextToken)
      .Returns(false);
    _specieRepository.GetSpecieAsync(command.SpecieName, TestContextToken)
      .Returns(new Specie { Name = "-", Size = Size.Tiny, Speed = 1, });
    _classRepository.CheckClassExistsAsync(Arg.Any<string>(), TestContextToken)
      .Returns(true);

    // Act
    Result<Guid> result = await _handler.Handle(command, TestContextToken);

    // Assert
    result.IsSuccess.Should().BeTrue();
    result.Value.Version.Should().Be(7);
  }

  [Fact(DisplayName = "Handler saves changes in one iteration (unit of work)")]
  public async Task HandlerSavesChangesInOneIteration() {
    // Arrange
    var command = new CreateCharacterCommand() {
      Name = "-",
      PlayerName = "-",
      SpecieName = "-",
      Classes = [new DndClass { Name = "-" }]
    };

    _characterRepository
      .CheckCharacterExistsAsync(Arg.Any<string>(), Arg.Any<string>(), TestContextToken)
      .Returns(false);
    _specieRepository.GetSpecieAsync(Arg.Any<string>(), TestContextToken)
      .Returns(new Specie { Name = "-", Size = Size.Tiny, Speed = 1, });
    _classRepository.CheckClassExistsAsync(Arg.Any<string>(), TestContextToken)
      .Returns(true);

    // Act
    await _handler.Handle(command, TestContextToken);

    // Assert
    await _unitOfWork.Received(1)
      .SaveChangesAsync(TestContextToken);
  }
}