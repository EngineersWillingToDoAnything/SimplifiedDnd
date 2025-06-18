namespace SimplifiedDnd.Application.Abstractions.Queries;

public record Page(int Index, int Size) {
  public int StartingIndex => Size * Index;
  public int EndingIndex => StartingIndex + Size;
  
  public static readonly Page Infinite = new(-1, -1);

  /// <summary>
  /// Determines whether the page configuration is valid.
  /// </summary>
  /// <returns>True if the page is infinite or has a non-negative index and a size of at least one; otherwise, false.</returns>
  public bool IsValid() {
    return this == Infinite || Index >= 0 && Size >= 1;
  }
}