using FluentValidation.TestHelper;
using SimplifiedDnd.Application.Abstractions.Queries;
using SimplifiedDnd.Application.Characters.GetCharacters;

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
}