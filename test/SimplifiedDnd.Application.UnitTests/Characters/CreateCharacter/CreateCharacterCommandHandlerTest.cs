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

  private readonly ISpecieRepository _specieRepository;
  private readonly ICharacterRepository _characterRepository;
  private readonly IUnitOfWork _unitOfWork;

  public CreateCharacterCommandHandlerTest() {
    _specieRepository = Substitute.For<ISpecieRepository>();
    _characterRepository = Substitute.For<ICharacterRepository>();
    _unitOfWork = Substitute.For<IUnitOfWork>();

    _handler = new(
      _specieRepository,
      _characterRepository,
      _unitOfWork);
  }

  [Fact(DisplayName = "Handler returns error if character already exists")]
  public async Task HandlerReturnsErrorWithExistingCharacter() {
    // Arrange
    var command = new CreateCharacterCommand() {
      Name = "Batman",
      PlayerName = "Beta tester 1",
      SpecieName = "-",
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

  [Fact(DisplayName = "Handler returns error if specie was not found")]
  public async Task HandlerReturnsErrorWithNonExistingSpecie() {
    // Arrange
    var command = new CreateCharacterCommand() {
      Name = "-",
      PlayerName = "-",
      SpecieName = "-",
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

  [Fact(DisplayName = "Handler returns character with given attributes")]
  public async Task HandlerReturnsCharacterWithGivenAttributes() {
    // Arrange
    var command = new CreateCharacterCommand() {
      Name = "Test",
      PlayerName = "Test",
      SpecieName = "human",
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

    // Act
    Result<Character> result = await _handler.Handle(command, TestContextToken);

    // Assert
    result.IsSuccess.Should().BeTrue();
    result.Value.Name.Should().Be(command.Name);
    result.Value.PlayerName.Should().Be(command.PlayerName);
    result.Value.Specie.Should().BeEquivalentTo(specie);
  }

  [Fact(DisplayName = "Handler saves character to repository")]
  public async Task HandlerSavesCharacterToRepository() {
    // Arrange
    var command = new CreateCharacterCommand() {
      Name = "Test",
      PlayerName = "Test",
      SpecieName = "human",
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

    // Act
    await _handler.Handle(command, TestContextToken);

    // Assert
    await _unitOfWork.Received(1)
      .SaveChangesAsync(TestContextToken);
  }
}