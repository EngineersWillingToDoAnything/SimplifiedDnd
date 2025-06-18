namespace SimplifiedDnd.Application.Abstractions.Queries;

public record Page(int Index, int Size) {
  public int StartingIndex => Size * Index;
  public int EndingIndex => StartingIndex + Size;
  
  public static readonly Page Infinite = new(-1, -1);

  public bool IsValid() {
    return this == Infinite || Index >= 0 && Size >= 1;
  }
}