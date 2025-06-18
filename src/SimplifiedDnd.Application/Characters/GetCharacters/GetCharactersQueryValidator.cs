using FluentValidation;

namespace SimplifiedDnd.Application.Characters.GetCharacters;

internal sealed class GetCharactersQueryValidator : AbstractValidator<GetCharactersQuery> {
  /// <summary>
  /// Initializes a new instance of the <see cref="GetCharactersQueryValidator"/> class, defining validation rules for pagination, ordering, and filtering parameters in a <see cref="GetCharactersQuery"/>.
  /// </summary>
  public GetCharactersQueryValidator() {
    RuleFor(query => query.Page)
      .Must(GetCharactersQuery.PageIsValid)
      .WithMessage("Pagination index must be greater than 1 and size must be greater than 0");
    
    RuleFor(query => query.Order)
      .Must(GetCharactersQuery.OrderIsValid)
      .WithMessage("Key to order by isn't valid");

    RuleFor(query => query.Filter.Classes)
      .Must(classes => classes.All(c => !string.IsNullOrWhiteSpace(c)))
      .WithMessage("All classes must have a value")
      .When(query => query.Filter.Classes.Count > 0);
    
    RuleFor(query => query.Filter.Species)
      .Must(species => species.All(s => !string.IsNullOrWhiteSpace(s)))
      .WithMessage("All species must have a value")
      .When(query => query.Filter.Species.Count > 0);
  }
}