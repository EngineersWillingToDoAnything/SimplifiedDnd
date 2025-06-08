using SimplifiedDnd.Domain.Characters;

namespace SimplifiedDnd.DataBase.Entities;

internal class CharacterDbEntity {
  public Guid Id { get; init; }
  public required string Name { get; init; }
  public required string PlayerName { get; init; }
  
  public int SpecieId { get; init; }
  public SpecieDbEntity? Specie { get; init; }

  internal Character ToDomain() {
    return new Character {
      Id = Id,
      Name = Name,
      PlayerName = PlayerName,
      Specie = Specie?.ToDomain()
    };
  }
}