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
  public async Task<bool> CheckCharacterExistsAsync(
    string name, string playerName, CancellationToken cancellationToken
  ) {
    return await context.Characters.AnyAsync(
      c => c.Name == name && c.PlayerName == playerName,
      cancellationToken);
  }

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