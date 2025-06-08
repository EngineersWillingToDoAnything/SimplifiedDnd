namespace SimplifiedDnd.DataBase.Entities;

internal class CharacterClassDbEntity {
  public Guid CharacterId { get; init; }
  public CharacterDbEntity? Character { get; init; }
  
  public int ClassId { get; init; }
  public ClassDbEntity? Class { get; init; }
  
  public bool IsMainClass { get; init; }
}