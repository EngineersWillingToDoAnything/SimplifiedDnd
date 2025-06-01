using FluentValidation;

namespace SimplifiedDnd.Application.Characters.GetCharacters;

internal sealed class GetCharactersQueryValidator : AbstractValidator<GetCharactersQuery> {
  public GetCharactersQueryValidator() {
    RuleFor(query => query.Page)
      .Must(GetCharactersQuery.PageIsValid)
      .WithMessage("Pagination index must be greater than 1 and size must be greater than 0");
    
    RuleFor(query => query.Order)
      .Must(GetCharactersQuery.OrderIsValid)
      .WithMessage("Key to order by isn't valid");
  }
}