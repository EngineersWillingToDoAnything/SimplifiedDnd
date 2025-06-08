using Microsoft.EntityFrameworkCore;
using SimplifiedDnd.Application.Abstractions.Characters;
using SimplifiedDnd.Application.Abstractions.Queries;
using SimplifiedDnd.DataBase.Abstractions;
using SimplifiedDnd.DataBase.Contexts;
using SimplifiedDnd.DataBase.Entities;
using SimplifiedDnd.DataBase.Extensions;
using SimplifiedDnd.Domain.Characters;
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
    var entity = new CharacterDbEntity {
      Id = character.Id,
      Name = character.Name,
      PlayerName = character.PlayerName,
    };

    context.Characters.Add(entity);
  }

  private sealed class CharacterFilterBuilder(CharacterFilter filter) : IFilterBuilder<CharacterDbEntity> {
    public Expression<Func<CharacterDbEntity, bool>> Build() {
      Expression<Func<CharacterDbEntity, bool>> predicate = ExpressionExtension.True<CharacterDbEntity>();

      if (filter.Name is not null) {
        predicate = predicate.And(ContainsNameExpression);
      }
      if (filter.Species.Count != 0) {
        predicate = predicate.And(BelongsToAnyOfTheSpeciesExpression);
      }

      return predicate;
    }

    private Expression<Func<CharacterDbEntity, bool>> ContainsNameExpression =>
      c => filter.Name != null &&
           c.Name.Contains(filter.Name, StringComparison.InvariantCultureIgnoreCase);

    private Expression<Func<CharacterDbEntity, bool>> BelongsToAnyOfTheSpeciesExpression =>
      c => filter.Species.Contains(c.Specie!.Name, StringComparer.InvariantCultureIgnoreCase);
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
        _ => order.Ascending
          ? queryable.OrderBy(c => c.Id)
          : queryable.OrderByDescending(c => c.Id)
      };
    }
  }
}