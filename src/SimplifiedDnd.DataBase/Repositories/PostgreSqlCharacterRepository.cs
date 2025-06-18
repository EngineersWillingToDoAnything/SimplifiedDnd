using Microsoft.EntityFrameworkCore;
using SimplifiedDnd.Application.Abstractions.Characters;
using SimplifiedDnd.Application.Abstractions.Queries;
using SimplifiedDnd.DataBase.Abstractions;
using SimplifiedDnd.DataBase.Contexts;
using SimplifiedDnd.DataBase.Entities;
using SimplifiedDnd.DataBase.Extensions;
using SimplifiedDnd.Domain.Characters;
using System.Diagnostics;
using System.Globalization;
using System.Linq.Expressions;

namespace SimplifiedDnd.DataBase.Repositories;

internal sealed class PostgreSqlCharacterRepository(
  MainDbContext context
) : ICharacterRepository {
  /// <summary>
  /// Asynchronously determines whether a character with the specified name and player name exists in the database.
  /// </summary>
  /// <param name="name">The character's name to search for.</param>
  /// <param name="playerName">The player name associated with the character.</param>
  /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
  /// <returns>True if a matching character exists; otherwise, false.</returns>
  public async Task<bool> CheckCharacterExistsAsync(
    string name, string playerName, CancellationToken cancellationToken
  ) {
    return await context.Characters.AnyAsync(
      c => c.Name == name && c.PlayerName == playerName,
      cancellationToken);
  }

  /// <summary>
  /// Asynchronously retrieves a paginated and ordered list of characters from the database, filtered by the specified criteria.
  /// </summary>
  /// <param name="page">Pagination information specifying the starting index and page size.</param>
  /// <param name="order">Ordering criteria for the result set.</param>
  /// <param name="filter">Filter criteria to apply to the character query.</param>
  /// <param name="cancellationToken">Token to observe while waiting for the task to complete.</param>
  /// <returns>A paginated result containing the list of matching characters and the total count.</returns>
  public async Task<PaginatedResult<Character>> GetCharactersAsync(
    Page page, Order order, CharacterFilter filter, CancellationToken cancellationToken
  ) {
    IQueryable<CharacterDbEntity> query = context.Characters
      .Include(c => c.Specie)
      .Include(c => c.CharacterClasses)
      .ThenInclude(cls => cls.Class)
      .Where(new CharacterFilterBuilder(filter).Build())
      .AsNoTracking();

    int totalAmount = await query.CountAsync(cancellationToken);

    query = new CharacterOrderBuilder(order).Build(query);
    if (page != Page.Infinite) {
      query = query.Skip(page.StartingIndex).Take(page.Size);
    }

    List<CharacterDbEntity> entities = await query.ToListAsync(cancellationToken);

    return new PaginatedResult<Character> {
      Values = [..entities.Select(entity => entity.ToDomain())],
      TotalAmount = totalAmount
    };
  }

  /// <summary>
  /// Saves a new character and its associated species and classes to the database.
  /// </summary>
  /// <param name="character">The character to be saved, including its main class, all classes, and species.</param>
  public void SaveCharacter(Character character) {
    Debug.Assert(character.MainClass is not null);
    Debug.Assert(character.Classes is not null);
    Debug.Assert(character.Specie is not null);

#pragma warning disable CA1304, CA1311, CA1862
    context.Characters.Add(new CharacterDbEntity {
      Id = character.Id,
      Name = character.Name,
      PlayerName = character.PlayerName,
      SpecieId = context.Species.Single(entity =>
        entity.Name.ToUpper() == character.Specie.Name.ToUpper()).Id,
    });

    context.CharacterClasses.Add(new CharacterClassDbEntity {
      CharacterId = character.Id,
      ClassId = context.Classes.Single(entity =>
        entity.Name.ToUpper() == character.MainClass.Name.ToUpper()).Id,
      IsMainClass = true
    });

    context.CharacterClasses.AddRange(character.Classes
      .Select(dndClass => new CharacterClassDbEntity {
        CharacterId = character.Id,
        ClassId = context.Classes.Single(entity =>
          entity.Name.ToUpper() == dndClass.Name.ToUpper()).Id,
        IsMainClass = false,
      }));
#pragma warning restore CA1304, CA1311, CA1862
  }

  private sealed class CharacterFilterBuilder(CharacterFilter filter) : IFilterBuilder<CharacterDbEntity> {
    private readonly string? _formattedName = filter.Name?.ToUpperInvariant();
    private readonly IEnumerable<string> _formattedSpeciesName = filter.Species.Select(s => s.ToUpperInvariant());
    private readonly IEnumerable<string> _formattedClassesName = filter.Classes.Select(c => c.ToUpperInvariant());

    /// <summary>
    /// Builds a LINQ expression predicate for filtering <see cref="CharacterDbEntity"/> objects based on the specified character filter criteria.
    /// </summary>
    /// <returns>
    /// An expression representing the combined filter conditions for character name, species, and classes.
    /// </returns>
    public Expression<Func<CharacterDbEntity, bool>> Build() {
      Expression<Func<CharacterDbEntity, bool>> predicate = ExpressionExtension.True<CharacterDbEntity>();

      if (filter.Name is not null) {
        predicate = predicate.And(ContainsNameExpression);
      }

      if (filter.Species.Count != 0) {
        predicate = predicate.And(BelongsToAnyOfTheSpeciesExpression);
      }

      if (filter.Classes.Count != 0) {
        predicate = predicate.And(BelongsToAnyOfTheClassesExpression);
      }

      return predicate;
    }

#pragma warning disable CA1304, CA1311, CA1862
    private Expression<Func<CharacterDbEntity, bool>> ContainsNameExpression =>
      character => _formattedName != null &&
                   character.Name.ToUpper().Contains(_formattedName);

    private Expression<Func<CharacterDbEntity, bool>> BelongsToAnyOfTheSpeciesExpression =>
      character => _formattedSpeciesName.Contains(character.Specie!.Name.ToUpper());

    private Expression<Func<CharacterDbEntity, bool>> BelongsToAnyOfTheClassesExpression =>
      character => character.CharacterClasses.Any(characterClass =>
        _formattedClassesName.Contains(characterClass.Class!.Name.ToUpper()));
#pragma warning restore CA1862, CA1311, CA1304
  }

  private sealed class CharacterOrderBuilder(Order order) : IOrderBuilder<CharacterDbEntity> {
    /// <summary>
    /// Orders a queryable collection of character entities based on the specified order key and direction.
    /// </summary>
    /// <param name="queryable">The queryable collection of character entities to order.</param>
    /// <returns>An <see cref="IOrderedQueryable{CharacterDbEntity}"/> ordered by the specified property and direction.</returns>
    public IOrderedQueryable<CharacterDbEntity> Build(IQueryable<CharacterDbEntity> queryable) {
      TextInfo textInfo = CultureInfo.InvariantCulture.TextInfo;

      return textInfo.ToTitleCase(order.Key) switch {
        nameof(Character.Name) => order.Ascending
          ? queryable.OrderBy(c => c.Name)
          : queryable.OrderByDescending(c => c.Name),
        nameof(Character.Specie) => order.Ascending
          ? queryable.OrderBy(c => c.Specie!.Name)
          : queryable.OrderByDescending(c => c.Specie!.Name),
        nameof(Character.MainClass) => order.Ascending
          ? queryable.OrderBy(c => c.CharacterClasses.Single(cc => cc.IsMainClass).Class!.Name)
          : queryable.OrderByDescending(c => c.CharacterClasses.Single(cc => cc.IsMainClass).Class!.Name),
        _ => order.Ascending
          ? queryable.OrderBy(c => c.Id)
          : queryable.OrderByDescending(c => c.Id)
      };
    }
  }
}