using FluentValidation.TestHelper;
using SimplifiedDnd.Application.Abstractions.Characters;
using SimplifiedDnd.Application.Abstractions.Queries;
using SimplifiedDnd.Application.Characters.GetCharacters;
using SimplifiedDnd.Domain.Characters;

namespace SimplifiedDnd.Application.UnitTests.Characters.GetCharacters;

public sealed class GetCharactersQueryValidatorTest {
  private readonly GetCharactersQueryValidator _validator;

  public GetCharactersQueryValidatorTest() {
    _validator = new GetCharactersQueryValidator();
  }

  [Fact(DisplayName = "Returns valid with empty query")]
  public void ValidatorReturnsSuccessWithEmptyQuery() {
    // Arrange
    var query = new GetCharactersQuery();

    // Act
    TestValidationResult<GetCharactersQuery>? result = _validator.TestValidate(query);

    // Assert
    result.ShouldNotHaveAnyValidationErrors();
  }

  [Fact(DisplayName = "Returns invalid with page index less than 0")]
  public void ValidatorReturnsErrorWithInvalidPageIndex() {
    // Arrange
    var query = new GetCharactersQuery {
      Page = new Page(-1, 0)
    };

    // Act
    TestValidationResult<GetCharactersQuery>? result = _validator.TestValidate(query);

    // Assert
    result.ShouldHaveValidationErrorFor(q => q.Page)
      .WithErrorMessage("Pagination index must be greater than 1 and size must be greater than 0")
      .Only();
  }

  [Fact(DisplayName = "Returns invalid with page size less than 1")]
  public void ValidatorReturnsErrorWithInvalidPageSize() {
    // Arrange
    var query = new GetCharactersQuery {
      Page = new Page(0, 0)
    };

    // Act
    TestValidationResult<GetCharactersQuery>? result = _validator.TestValidate(query);

    // Assert
    result.ShouldHaveValidationErrorFor(q => q.Page)
      .WithErrorMessage("Pagination index must be greater than 1 and size must be greater than 0")
      .Only();
  }

  [Fact(DisplayName = "Returns valid with infinite page")]
  public void ValidatorReturnsSuccessWithInfinitePage() {
    // Arrange
    var query = new GetCharactersQuery {
      Page = Page.Infinite
    };

    // Act
    TestValidationResult<GetCharactersQuery>? result = _validator.TestValidate(query);

    // Assert
    result.ShouldNotHaveValidationErrorFor(q => q.Page);
  }

  [Fact(DisplayName = "Returns valid with valid page")]
  public void ValidatorReturnsSuccessWithValidPage() {
    // Arrange
    var query = new GetCharactersQuery {
      Page = new Page(0, 1)
    };

    // Act
    TestValidationResult<GetCharactersQuery>? result = _validator.TestValidate(query);

    // Assert
    result.ShouldNotHaveValidationErrorFor(q => q.Page);
  }

  [Theory(DisplayName = "Returns invalid with invalid order by key")]
  [InlineData(true)]
  [InlineData(false)]
  public void ValidatorReturnsErrorWithInvalidOrderByKey(bool orderAscending) {
    // Arrange
    var query = new GetCharactersQuery {
      Order = orderAscending ? Order.CreateAscending(string.Empty) : Order.CreateDescending(string.Empty)
    };

    // Act
    TestValidationResult<GetCharactersQuery>? result = _validator.TestValidate(query);

    // Assert
    result.ShouldHaveValidationErrorFor(q => q.Order)
      .WithErrorMessage("Key to order by isn't valid")
      .Only();
  }

  [Theory(DisplayName = "Returns valid with valid order by key")]
  [InlineData(nameof(Character.Id))]
  [InlineData(nameof(Character.Name))]
  [InlineData(nameof(Character.MainClass))]
  [InlineData(nameof(Character.Specie))]
  public void ValidatorReturnsSuccessWithValidOrderByKey(string orderByKey) {
    // Arrange
    var query = new GetCharactersQuery {
      Order = Order.CreateDescending(orderByKey)
    };

    // Act
    TestValidationResult<GetCharactersQuery>? result = _validator.TestValidate(query);

    // Assert
    result.ShouldNotHaveValidationErrorFor(q => q.Order);
  }
  
  [Theory(DisplayName = "Returns invalid with empty class")]
  [InlineData(null)]
  [InlineData("")]
  [InlineData(" ")]
  public void ValidatorReturnsErrorWithEmptyClass(string? emptyClass) {
    // Arrange
    var query = new GetCharactersQuery {
      Filter = new CharacterFilter {
        Classes = [emptyClass!]
      }
    };

    // Act
    TestValidationResult<GetCharactersQuery>? result = _validator.TestValidate(query);

    // Assert
    result.ShouldHaveValidationErrorFor(q => q.Filter.Classes)
      .WithErrorMessage("All classes must have a value")
      .Only();
  }

  [Theory(DisplayName = "Returns invalid with empty specie")]
  [InlineData(null)]
  [InlineData("")]
  [InlineData(" ")]
  public void ValidatorReturnsErrorWithEmptySpecie(string? emptySpecie) {
    // Arrange
    var query = new GetCharactersQuery {
      Filter = new CharacterFilter {
        Species = [emptySpecie!]
      }
    };

    // Act
    TestValidationResult<GetCharactersQuery>? result = _validator.TestValidate(query);

    // Assert
    result.ShouldHaveValidationErrorFor(q => q.Filter.Species)
      .WithErrorMessage("All species must have a value")
      .Only();
  }
}