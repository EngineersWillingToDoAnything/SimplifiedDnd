using FluentValidation;

namespace SimplifiedDnd.Application.Characters.GetCharacters;

internal sealed class GetCharactersQueryValidator : AbstractValidator<GetCharactersQuery> {
  public GetCharactersQueryValidator() {
    RuleFor(query => query.Page)
      .Must(page => page?.IsValid() ?? true)
      .WithMessage("Pagination index must be greater than 1 and size must be greater than 0");
  }
}