using SimplifiedDnd.Application.Characters.CreateCharacter;
using SimplifiedDnd.Domain.Characters;

namespace SimplifiedDnd.Application.UnitTests.Characters.CreateCharacter;

public sealed class CreateCharacterCommandHandlerTest {
  private static CancellationToken TestContextToken => TestContext.Current.CancellationToken;

  private readonly CreateCharacterCommandHandler _handler;

  public CreateCharacterCommandHandlerTest() {
    _handler = new();
  }

  [Fact]
  public async Task HandlerReturnsCharacterWithGivenValues() {
    var command = new CreateCharacterCommand() {
      Name = "Test",
      PlayerName = "Test",
    };

    Character character = await _handler.Handle(command, TestContextToken);

    character.Name.Should().Be(command.Name);
    character.PlayerName.Should().Be(command.PlayerName);
  }
}