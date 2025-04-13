using NSubstitute;
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

  public CreateCharacterCommandHandlerTest() {
    _specieRepository = Substitute.For<ISpecieRepository>();
    _handler = new(_specieRepository);
  }
  
  [Fact]
  public async Task HandlerReturnsErrorWithNonExistingSpecie() {
    // Arrange
    var command = new CreateCharacterCommand() {
      Name = string.Empty,
      PlayerName = string.Empty,
      SpecieName = string.Empty,
    };
    
    _specieRepository.GetSpecieAsync(command.SpecieName, TestContextToken)
      .Returns((Specie?)null);

    // Act
    Result<Character> result = await _handler.Handle(command, TestContextToken);

    // Assert
    result.IsSuccess.Should().BeFalse();
    result.Error.Should().Be(CharacterError.NonExistingSpecie);
  }

  [Fact]
  public async Task HandlerReturnsCharacterWithGivenAttributes() {
    // Arrange
    var command = new CreateCharacterCommand() {
      Name = "Test",
      PlayerName = "Test",
      SpecieName = "human",
    };

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
    Character character = result.Value;
    character.Name.Should().Be(command.Name);
    character.PlayerName.Should().Be(command.PlayerName);
    character.Specie.Should().Be(specie);
  }
}