using SimplifiedDnd.Application.Abstractions.Characters;
using SimplifiedDnd.Application.Abstractions.Core;
using SimplifiedDnd.Application.Abstractions.Queries;
using SimplifiedDnd.Application.Characters.GetCharacters;
using SimplifiedDnd.Domain.Characters;

namespace SimplifiedDnd.Application.UnitTests.Characters.GetCharacters;

public sealed class GetCharactersQueryHandlerTest {
  private static CancellationToken TestContextToken => TestContext.Current.CancellationToken;
  private readonly GetCharactersQueryHandler _handler;

  private readonly IReadOnlyCharacterRepository _repository;

  public GetCharactersQueryHandlerTest() {
    _repository = Substitute.For<IReadOnlyCharacterRepository>();

    _handler = new GetCharactersQueryHandler(_repository);
  }

  [Fact(DisplayName = "Returns characters")]
  public async Task HandlerReturnsCharacters() {
    // Arrange
    var query = new GetCharactersQuery();

    _repository
      .GetCharactersAsync(Arg.Any<Page>(), Arg.Any<Order>(), Arg.Any<CharacterFilter>(), TestContextToken)
      .Returns(new PaginatedResult<Character> {
        Values = [],
        TotalAmount = 0
      });

    // Act
    Result<PaginatedResult<Character>> result = await _handler.Handle(
      query, TestContextToken);

    // Assert
    result.IsSuccess.Should().BeTrue();
    result.Value.Values.Should().NotBeNull();
  }

  [Fact(DisplayName = "Doesn't paginate if page is not specified")]
  public async Task HandlerDoesNotPaginateIfPageIsNotSpecified() {
    // Arrange
    var query = new GetCharactersQuery();

    // Act
    await _handler.Handle(query, TestContextToken);

    // Assert
    await _repository.Received(1)
      .GetCharactersAsync(
        Arg.Is<Page>(page => page == Page.Infinite),
        Arg.Any<Order>(),
        Arg.Any<CharacterFilter>(),
        TestContextToken);
  }

  [Fact(DisplayName = "Returns exact amount of characters")]
  public async Task HandlerReturnsPaginatedCharacters() {
    // Arrange
    var page = new Page(0, 3);
    var query = new GetCharactersQuery {
      Page = page,
    };

    _repository.GetCharactersAsync(query.Page, Arg.Any<Order>(), Arg.Any<CharacterFilter>(), TestContextToken)
      .Returns(new PaginatedResult<Character> {
        Values = Enumerable.Repeat(new Character {
          Id = Guid.CreateVersion7(),
          Name = "Aquiles",
          PlayerName = "Homero"
        }, page.Size).ToList(),
        TotalAmount = page.EndingIndex
      });

    // Act
    Result<PaginatedResult<Character>> result = await _handler.Handle(
      query, TestContextToken);

    // Assert
    result.IsSuccess.Should().BeTrue();
    result.Value.Values.Should().HaveCount(page.Size);
    result.Value.TotalAmount.Should().Be(page.EndingIndex);
  }

  [Fact(DisplayName = "Returns less characters if index * size + size is greater than pages")]
  public async Task HandlerReturnsLessPaginatedCharacters() {
    // Arrange
    var page = new Page(1, 3);
    var query = new GetCharactersQuery {
      Page = page,
    };
    const int totalAmount = 4;

    _repository.GetCharactersAsync(query.Page, Arg.Any<Order>(), Arg.Any<CharacterFilter>(), TestContextToken)
      .Returns(new PaginatedResult<Character> {
        Values = Enumerable.Repeat(new Character {
          Id = Guid.CreateVersion7(),
          Name = "Spider-man",
          PlayerName = "Peter Parker"
        }, (totalAmount - page.StartingIndex) % page.Size).ToList(),
        TotalAmount = totalAmount
      });

    // Act
    Result<PaginatedResult<Character>> result = await _handler.Handle(
      query, TestContextToken);

    // Assert
    result.IsSuccess.Should().BeTrue();
    result.Value.Values.Should().NotHaveCount(totalAmount);
    result.Value.TotalAmount.Should().Be(totalAmount);
  }

  [Fact(DisplayName = "Orders by id in an ascending order if is not specified")]
  public async Task HandlerOrdersByIdInAscendingOrderByDefault() {
    // Arrange
    var query = new GetCharactersQuery();

    // Act
    await _handler.Handle(query, TestContextToken);

    // Assert
    await _repository.Received(1)
      .GetCharactersAsync(
        Arg.Any<Page>(),
        Arg.Is<Order>(order => order.Key == nameof(Character.Id) &&
                               order.Ascending),
        Arg.Any<CharacterFilter>(),
        TestContextToken);
  }

#pragma warning disable S1144
  private class ValidOrderKeys() : TheoryData<string>(
#pragma warning restore S1144
    nameof(Character.Id),
    nameof(Character.Name),
    nameof(Character.MainClass),
    nameof(Character.Specie)
  );

  [Theory(DisplayName = "Orders by specific key")]
  [ClassData(typeof(ValidOrderKeys))]
  public async Task HandlerOrdersBySpecificKey(string orderKey) {
    // Arrange
    var query = new GetCharactersQuery {
      Order = Order.CreateAscending(orderKey)
    };

    // Act
    await _handler.Handle(query, TestContextToken);

    // Assert
    await _repository.Received(1)
      .GetCharactersAsync(
        Arg.Any<Page>(),
        Arg.Is<Order>(order => order.Key == orderKey),
        Arg.Any<CharacterFilter>(),
        TestContextToken);
  }

  [Theory(DisplayName = "Orders in the specified order")]
  [InlineData(true)]
  [InlineData(false)]
  public async Task HandlerOrdersBySpecificOrder(bool orderAscending) {
    // Arrange
    var query = new GetCharactersQuery {
      Order = orderAscending
        ? Order.CreateAscending(nameof(Character.Id))
        : Order.CreateDescending(nameof(Character.Id))
    };

    // Act
    await _handler.Handle(query, TestContextToken);

    // Assert
    await _repository.Received(1)
      .GetCharactersAsync(
        Arg.Any<Page>(),
        Arg.Is<Order>(order => order.Ascending == orderAscending),
        Arg.Any<CharacterFilter>(),
        TestContextToken);
  }

  [Fact(DisplayName = "Filters by part of the name")]
  public async Task HandlerFiltersByPartOfName() {
    // Arrange
    var filter = new CharacterFilter {
      Name = "Dark"
    };
    var query = new GetCharactersQuery {
      Filter = filter
    };

    // Act
    await _handler.Handle(query, TestContextToken);

    // Assert
    await _repository.Received(1)
      .GetCharactersAsync(
        Arg.Any<Page>(),
        Arg.Any<Order>(),
        Arg.Is<CharacterFilter>(f => f.Name == filter.Name),
        TestContextToken);
  }

  [Fact(DisplayName = "Filters by the main class name being in set")]
  public async Task HandlerFiltersByMainClassBeingInSet() {
    // Arrange
    var filter = new CharacterFilter {
      Classes = ["Barbarian", "fighter"]
    };
    var query = new GetCharactersQuery {
      Filter = filter
    };

    // Act
    await _handler.Handle(query, TestContextToken);

    // Assert
    await _repository.Received(1)
      .GetCharactersAsync(
        Arg.Any<Page>(),
        Arg.Any<Order>(),
        Arg.Is<CharacterFilter>(f => f.Classes.SequenceEqual(filter.Classes)),
        TestContextToken);
  }

  [Fact(DisplayName = "Filters by the specie name being in set")]
  public async Task HandlerFiltersBySpeciesBeingInSet() {
    // Arrange
    var filter = new CharacterFilter {
      Species = ["Dragonborn", "tiefling"]
    };
    var query = new GetCharactersQuery {
      Filter = filter
    };

    // Act
    await _handler.Handle(query, TestContextToken);

    // Assert
    await _repository.Received(1)
      .GetCharactersAsync(
        Arg.Any<Page>(),
        Arg.Any<Order>(),
        Arg.Is<CharacterFilter>(f => f.Species.SequenceEqual(filter.Species)),
        TestContextToken);
  }
}