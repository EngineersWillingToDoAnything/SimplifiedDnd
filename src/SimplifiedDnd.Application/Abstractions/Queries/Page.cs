namespace SimplifiedDnd.Application.Abstractions.Queries;

public record Page(int Index, int Size) {
  public int SkipAmount => Size * Index;
  
  public static readonly Page Infinite = new(-1, -1);

  public bool IsValid() {
    return this == Infinite || Index >= 0 && Size >= 1;
  }
}