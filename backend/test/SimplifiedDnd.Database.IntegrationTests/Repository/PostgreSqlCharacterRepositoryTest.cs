using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using SimplifiedDnd.Application.Abstractions.Characters;
using SimplifiedDnd.Application.Abstractions.Queries;
using SimplifiedDnd.DataBase.Entities;
using SimplifiedDnd.Database.IntegrationTests.Abstractions;
using SimplifiedDnd.Database.IntegrationTests.Extensions;
using SimplifiedDnd.DataBase.Repositories;
using SimplifiedDnd.Domain;
using SimplifiedDnd.Domain.Characters;

namespace SimplifiedDnd.Database.IntegrationTests.Repository;

#pragma warning disable CA1515
// ReSharper disable once UnusedType.Global
public sealed class PostgreSqlCharacterRepositoryTest {
#pragma warning restore CA1515
#pragma warning disable CA1034
  // ReSharper disable once UnusedType.Global
  public class AddCharacter : BaseIntegrationTest {
#pragma warning restore CA1034
    private readonly PostgreSqlCharacterRepository _repository;

    public AddCharacter(IntegrationTestWebAppFactory factory) :
      base(factory) {
      _repository = GetRequiredService<ICharacterRepository, PostgreSqlCharacterRepository>();
    }

    [Fact(
      DisplayName = "Doesn't add character without saving changes",
      Explicit = true)]
    public void RepositoryDoesNotAddCharacterWithoutSavingChanges() {
      // Arrange
      string specieName = Guid.CreateVersion7().ToString();
      string className = Guid.CreateVersion7().ToString();

      DbContext.Species.Add(new SpecieDbEntity {
        Name = specieName,
        Speed = 0,
      });

      DbContext.Classes.Add(new ClassDbEntity {
        Name = className,
      });
      DbContext.SaveChanges();

      var character = new Character {
        Id = Guid.CreateVersion7(),
        Name = Guid.CreateVersion7().ToString(),
        PlayerName = "...",
        MainClass = new DndClass {
          Name = className,
          Level = Level.MinLevel
        },
        Specie = new Specie {
          Name = specieName,
          Size = Size.Tiny,
          Speed = 0,
        },
      };

      // Act
      _repository.AddCharacter(character);

      // Assert
      CharacterDbEntity? addedCharacter = DbContext.Characters.SingleOrDefault(entity =>
        entity.Id == character.Id);

      addedCharacter.Should().BeNull();
    }

    [Fact(
      DisplayName = "Adds character if changes saved",
      Explicit = true)]
    public void RepositoryAddsCharacterIfChangesSaved() {
      // Arrange
      string specieName = Guid.CreateVersion7().ToString();
      string className = Guid.CreateVersion7().ToString();

      DbContext.Species.Add(new SpecieDbEntity {
        Name = specieName,
        Speed = 0,
      });

      DbContext.Classes.Add(new ClassDbEntity {
        Name = className,
      });
      DbContext.SaveChanges();

      var character = new Character {
        Id = Guid.CreateVersion7(),
        Name = Guid.CreateVersion7().ToString(),
        PlayerName = "...",
        MainClass = new DndClass {
          Name = className,
          Level = Level.MinLevel
        },
        Specie = new Specie {
          Name = specieName,
          Size = Size.Tiny,
          Speed = 0,
        },
      };

      // Act
      _repository.AddCharacter(character);
      DbContext.SaveChanges();

      // Assert
      CharacterDbEntity? addedCharacter = DbContext.Characters.SingleOrDefault(entity =>
        entity.Id == character.Id);

      addedCharacter.Should().NotBeNull();
    }

    [Fact(
      DisplayName = "Adds character with a class for each given class",
      Explicit = true)]
    public void RepositoryAddsCharacterWithGivenClasses() {
      // Arrange
      string specieName = Guid.CreateVersion7().ToString();
      var classNames = Enumerable.Range(0, 3)
        .Select(_ => Guid.CreateVersion7().ToString())
        .ToList();

      DbContext.Species.Add(new SpecieDbEntity {
        Name = specieName,
        Speed = 0,
      });

      DbContext.Classes.AddRange(classNames.Select(className =>
        new ClassDbEntity { Name = className, }));
      DbContext.SaveChanges();

      var character = new Character {
        Id = Guid.CreateVersion7(),
        Name = Guid.CreateVersion7().ToString(),
        PlayerName = "...",
        MainClass = new DndClass {
          Name = classNames[0],
          Level = Level.MinLevel,
        },
        Classes = classNames.Skip(1)
          .Select(className => new DndClass {
            Name = className,
            Level = Level.MinLevel,
          })
          .ToList(),
        Specie = new Specie {
          Name = specieName,
          Size = Size.Tiny,
          Speed = 0,
        },
      };

      // Act
      _repository.AddCharacter(character);
      DbContext.SaveChanges();

      // Assert
      var characterClasses = DbContext.CharacterClasses
        .Where(entity => entity.CharacterId == character.Id)
        .ToList();

      characterClasses.Should().HaveCount(classNames.Count);
    }

    [Fact(
      DisplayName = "Adds character with main class",
      Explicit = true)]
    public void RepositoryAddsCharacterWithMainClass() {
      // Arrange
      string specieName = Guid.CreateVersion7().ToString();
      string className = Guid.CreateVersion7().ToString();

      DbContext.Species.Add(new SpecieDbEntity {
        Name = specieName,
        Speed = 0,
      });

      DbContext.Classes.Add(new ClassDbEntity {
        Name = className,
      });
      DbContext.SaveChanges();

      var character = new Character {
        Id = Guid.CreateVersion7(),
        Name = Guid.CreateVersion7().ToString(),
        PlayerName = "...",
        MainClass = new DndClass {
          Name = className,
          Level = Level.MinLevel,
        },
        Specie = new Specie {
          Name = specieName,
          Size = Size.Tiny,
          Speed = 0,
        },
      };

      // Act
      _repository.AddCharacter(character);
      DbContext.SaveChanges();

      // Assert
      CharacterClassDbEntity? mainCharacterClass = DbContext.CharacterClasses
        .SingleOrDefault(entity => entity.CharacterId == character.Id && entity.IsMainClass);

      mainCharacterClass.Should().NotBeNull();
    }

    [Fact(
      DisplayName = "Adds character with other classes not as main class",
      Explicit = true)]
    public void RepositoryAddsCharacterWithOtherClassesNotAsMainClass() {
      // Arrange
      string specieName = Guid.CreateVersion7().ToString();
      var classNames = Enumerable.Range(0, 3)
        .Select(_ => Guid.CreateVersion7().ToString())
        .ToList();

      DbContext.Species.Add(new SpecieDbEntity {
        Name = specieName,
        Speed = 0,
      });

      DbContext.Classes.AddRange(classNames.Select(className =>
        new ClassDbEntity { Name = className, }));
      DbContext.SaveChanges();

      var character = new Character {
        Id = Guid.CreateVersion7(),
        Name = Guid.CreateVersion7().ToString(),
        PlayerName = "...",
        MainClass = new DndClass {
          Name = classNames[0],
          Level = Level.MinLevel,
        },
        Classes = classNames.Skip(1)
          .Select(className => new DndClass {
            Name = className,
            Level = Level.MinLevel,
          })
          .ToList(),
        Specie = new Specie {
          Name = specieName,
          Size = Size.Tiny,
          Speed = 0,
        },
      };

      // Act
      _repository.AddCharacter(character);
      DbContext.SaveChanges();

      // Assert
      var characterClasses = DbContext.CharacterClasses
        .Where(entity => entity.CharacterId == character.Id && !entity.IsMainClass)
        .ToList();

      characterClasses.Should().HaveCount(character.Classes.Count);
    }

    [Fact(
      DisplayName = "Adds character with specie",
      Explicit = true)]
    public void RepositoryAddsCharacterWithSpecie() {
      // Arrange
      string specieName = Guid.CreateVersion7().ToString();
      string className = Guid.CreateVersion7().ToString();

      DbContext.Species.Add(new SpecieDbEntity {
        Name = specieName,
        Speed = 0,
      });

      DbContext.Classes.Add(new ClassDbEntity {
        Name = className,
      });
      DbContext.SaveChanges();

      var character = new Character {
        Id = Guid.CreateVersion7(),
        Name = Guid.CreateVersion7().ToString(),
        PlayerName = "...",
        MainClass = new DndClass {
          Name = className,
          Level = Level.MinLevel,
        },
        Specie = new Specie {
          Name = specieName,
          Size = Size.Tiny,
          Speed = 0,
        },
      };

      // Act
      _repository.AddCharacter(character);
      DbContext.SaveChanges();

      // Assert
      bool characterIsAssociatedToSpecie = DbContext.Characters
        .Any(entity => entity.Id == character.Id &&
                       entity.Specie!.Name == character.Specie.Name);

      characterIsAssociatedToSpecie.Should().BeTrue();
    }
  }

#pragma warning disable CA1034
  // ReSharper disable once UnusedType.Global
  public class CheckCharacterExistsAsync : BaseIntegrationTest {
#pragma warning restore CA1034
    private readonly PostgreSqlCharacterRepository _repository;

    public CheckCharacterExistsAsync(IntegrationTestWebAppFactory factory) :
      base(factory) {
      _repository = GetRequiredService<ICharacterRepository, PostgreSqlCharacterRepository>();
    }

    [Fact(
      DisplayName = "Returns false if an exception was thrown",
      Explicit = true)]
    public async Task RepositoryReturnsFalseIfAnExceptionIsThrown() {
      // Arrange
      DbContext.SetInvalidConnectionString();

      // Act
      bool doesCharacterExists = await _repository.CheckCharacterExistsAsync(
        string.Empty, string.Empty, TestContext.Current.CancellationToken);
      DbContext.RestoreConnectionString();

      // Assert
      doesCharacterExists.Should().BeFalse();
    }

    [Fact(
      DisplayName = "Returns false if there are no characters",
      Explicit = true)]
    public async Task RepositoryReturnsFalseIfThereAreNoCharacters() {
      // Act
      bool doesCharacterExists = await _repository.CheckCharacterExistsAsync(
        string.Empty, string.Empty, TestContext.Current.CancellationToken);

      // Assert
      doesCharacterExists.Should().BeFalse();
    }

    [Fact(
      DisplayName = "Returns false if character has same name but is from a different player",
      Explicit = true)]
    public async Task RepositoryReturnsFalseIfCharacterHasSameNameButDifferentPlayer() {
      // Arrange
      const string Name = "DragonForce";

      DbContext.Characters.Add(new CharacterDbEntity {
        Id = Guid.CreateVersion7(),
        Name = Name,
        PlayerName = "Player 1",
        Specie = new SpecieDbEntity {
          Name = Guid.CreateVersion7().ToString(),
          Speed = 0,
        }
      });
      await DbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

      // Act
      bool doesCharacterExists = await _repository.CheckCharacterExistsAsync(
        Name, string.Empty, TestContext.Current.CancellationToken);

      // Assert
      doesCharacterExists.Should().BeFalse();
    }

    [Fact(
      DisplayName = "Returns false if character is from the same player but has a different name",
      Explicit = true)]
    public async Task RepositoryReturnsFalseIfCharacterIsFromSamePlayerButHasDifferentName() {
      // Arrange
      const string PlayerName = "Bruce";
      DbContext.Characters.Add(new CharacterDbEntity {
        Id = Guid.CreateVersion7(),
        Name = "Loki",
        PlayerName = PlayerName,
        Specie = new SpecieDbEntity {
          Name = Guid.CreateVersion7().ToString(),
          Speed = 0,
        }
      });
      await DbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

      // Act
      bool doesCharacterExists = await _repository.CheckCharacterExistsAsync(
        string.Empty, PlayerName, TestContext.Current.CancellationToken);

      // Assert
      doesCharacterExists.Should().BeFalse();
    }

    [Fact(
      DisplayName = "Returns true if a character matches both name and player name",
      Explicit = true)]
    public async Task RepositoryReturnsTrueIfCharacterMatchesBothNameAndPlayerName() {
      // Arrange
      const string Name = "2Pac";
      const string PlayerName = "Michael";
      DbContext.Characters.Add(new CharacterDbEntity {
        Id = Guid.CreateVersion7(),
        Name = Name,
        PlayerName = PlayerName,
        Specie = new SpecieDbEntity {
          Name = Guid.CreateVersion7().ToString(),
          Speed = 0,
        }
      });
      await DbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

      // Act
      bool doesCharacterExists = await _repository.CheckCharacterExistsAsync(
        Name, PlayerName, TestContext.Current.CancellationToken);

      // Assert
      doesCharacterExists.Should().BeTrue();
    }

    [Fact(
      DisplayName = "Returns true if a character matches having different case in name",
      Explicit = true)]
    public async Task RepositoryReturnsTrueIfCharacterMatchesHavingDifferentCaseInName() {
      // Arrange
      const string Name = "Akaza";
      const string PlayerName = "Fran";
      DbContext.Characters.Add(new CharacterDbEntity {
        Id = Guid.CreateVersion7(),
        Name = Name,
        PlayerName = PlayerName,
        Specie = new SpecieDbEntity {
          Name = Guid.CreateVersion7().ToString(),
          Speed = 0,
        }
      });
      await DbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

      // Act
      bool doesCharacterExists = await _repository.CheckCharacterExistsAsync(
        Name.ToUpperInvariant(), PlayerName, TestContext.Current.CancellationToken);

      // Assert
      doesCharacterExists.Should().BeTrue();
    }

    [Fact(
      DisplayName = "Returns true if a character matches having different case in player name",
      Explicit = true)]
    public async Task RepositoryReturnsTrueIfCharacterMatchesHavingDifferentCaseInPlayerName() {
      // Arrange
      const string Name = "Sword god";
      const string PlayerName = "Kocchi no Kento";
      DbContext.Characters.Add(new CharacterDbEntity {
        Id = Guid.CreateVersion7(),
        Name = Name,
        PlayerName = PlayerName,
        Specie = new SpecieDbEntity {
          Name = Guid.CreateVersion7().ToString(),
          Speed = 0,
        }
      });
      await DbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

      // Act
      bool doesCharacterExists = await _repository.CheckCharacterExistsAsync(
        Name, PlayerName.ToUpperInvariant(), TestContext.Current.CancellationToken);

      // Assert
      doesCharacterExists.Should().BeTrue();
    }
  }

#pragma warning disable CA1034
  // ReSharper disable once UnusedType.Global
  public class GetCharactersAsync : BaseIntegrationTest {
#pragma warning restore CA1034

    private readonly PostgreSqlCharacterRepository _repository;

    public GetCharactersAsync(IntegrationTestWebAppFactory factory) :
      base(factory) {
      _repository = GetRequiredService<ICharacterRepository, PostgreSqlCharacterRepository>();
    }

    [Fact(
      DisplayName = "Doesn't return characters if there are no characters",
      Explicit = true)]
    public async Task RepositoryDoesNotReturnCharactersWithoutCharacters() {
      // Arrange
      await DbContext.Characters.ExecuteDeleteAsync(TestContext.Current.CancellationToken);

      // Act
      PaginatedResult<Character> result = await _repository.GetCharactersAsync(
        Page.Infinite,
        Order.CreateAscending(nameof(Character.Id)),
        new CharacterFilter(),
        TestContext.Current.CancellationToken);

      // Assert
      result.TotalAmount.Should().Be(0);
    }

    [Fact(
      DisplayName = "Returns the total amount of characters without filter and pagination",
      Explicit = true)]
    public async Task RepositoryReturnsTotalAmountOfCharactersWithoutFilterAndPagination() {
      // Arrange
      DbContext.Characters.AddRange(Enumerable.Range(0, 10)
        .Select(_ => Guid.CreateVersion7())
        .Select(characterId => new CharacterDbEntity {
          Id = characterId,
          Name = characterId.ToString(),
          PlayerName = string.Empty,
          Specie = new SpecieDbEntity {
            Name = Guid.CreateVersion7().ToString(),
            Speed = 0,
          },
          CharacterClasses = Enumerable.Range(0, 3)
            .Select(index => new CharacterClassDbEntity {
              IsMainClass = index == 0,
              Class = new ClassDbEntity {
                Name = Guid.CreateVersion7().ToString(),
              },
            })
            .ToList(),
        }));
      await DbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

      // Act
      PaginatedResult<Character> result = await _repository.GetCharactersAsync(
        Page.Infinite,
        Order.CreateAscending(nameof(Character.Id)),
        new CharacterFilter(),
        TestContext.Current.CancellationToken);

      // Assert
      int amount = await DbContext.Characters.CountAsync(TestContext.Current.CancellationToken);

      result.TotalAmount.Should().Be(amount);
    }

    [Fact(
      DisplayName = "Returns all characters without filter and pagination",
      Explicit = true)]
    public async Task RepositoryReturnsAllCharactersWithoutFilterAndPagination() {
      // Arrange
      DbContext.Characters.AddRange(Enumerable.Range(0, 10)
        .Select(_ => Guid.CreateVersion7())
        .Select(characterId => new CharacterDbEntity {
          Id = characterId,
          Name = characterId.ToString(),
          PlayerName = string.Empty,
          Specie = new SpecieDbEntity {
            Name = Guid.CreateVersion7().ToString(),
            Speed = 0,
          },
          CharacterClasses = Enumerable.Range(0, 3)
            .Select(index => new CharacterClassDbEntity {
              IsMainClass = index == 0,
              Class = new ClassDbEntity {
                Name = Guid.CreateVersion7().ToString(),
              },
            })
            .ToList(),
        }));
      await DbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

      // Act
      PaginatedResult<Character> result = await _repository.GetCharactersAsync(
        Page.Infinite,
        Order.CreateAscending(nameof(Character.Id)),
        new CharacterFilter(),
        TestContext.Current.CancellationToken);

      // Assert
      result.Values.Should().HaveCount(result.TotalAmount);
    }

    [Fact(
      DisplayName = "Returns exact amount of characters",
      Explicit = true)]
    public async Task RepositoryReturnsExactAmountOfCharacters() {
      // Arrange
      DbContext.Characters.AddRange(Enumerable.Range(0, 10)
        .Select(_ => Guid.CreateVersion7())
        .Select(characterId => new CharacterDbEntity {
          Id = characterId,
          Name = characterId.ToString(),
          PlayerName = string.Empty,
          Specie = new SpecieDbEntity {
            Name = Guid.CreateVersion7().ToString(),
            Speed = 0,
          },
          CharacterClasses = Enumerable.Range(0, 3)
            .Select(index => new CharacterClassDbEntity {
              IsMainClass = index == 0,
              Class = new ClassDbEntity {
                Name = Guid.CreateVersion7().ToString(),
              },
            })
            .ToList(),
        }));
      await DbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

      int totalAmount = await DbContext.Characters.CountAsync(TestContext.Current.CancellationToken);

      // Act
      PaginatedResult<Character> result = await _repository.GetCharactersAsync(
        new Page(0, totalAmount / 2),
        Order.CreateAscending(nameof(Character.Id)),
        new CharacterFilter(),
        TestContext.Current.CancellationToken);

      // Assert
      result.Values.Should().HaveCount(totalAmount / 2);
    }

    [Fact(
      DisplayName = "Returns less characters if index * size + size is greater than pages",
      Explicit = true)]
    public async Task RepositoryReturnsLessCharactersIfIndexTimesSizePlusSizeIsGreaterThanPages() {
      // Arrange
      DbContext.Characters.AddRange(Enumerable.Range(0, 10)
        .Select(_ => Guid.CreateVersion7())
        .Select(characterId => new CharacterDbEntity {
          Id = characterId,
          Name = characterId.ToString(),
          PlayerName = string.Empty,
          Specie = new SpecieDbEntity {
            Name = Guid.CreateVersion7().ToString(),
            Speed = 0,
          },
          CharacterClasses = Enumerable.Range(0, 3)
            .Select(index => new CharacterClassDbEntity {
              IsMainClass = index == 0,
              Class = new ClassDbEntity {
                Name = Guid.CreateVersion7().ToString(),
              },
            })
            .ToList(),
        }));
      await DbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

      int totalAmount = await DbContext.Characters.CountAsync(TestContext.Current.CancellationToken);

      // Act
      PaginatedResult<Character> result = await _repository.GetCharactersAsync(
        new Page(1, totalAmount - 1),
        Order.CreateAscending(nameof(Character.Id)),
        new CharacterFilter(),
        TestContext.Current.CancellationToken);

      // Assert
      result.Values.Should().HaveCountLessThanOrEqualTo(totalAmount - 1);
    }

    [Fact(
      DisplayName = "Orders by the name of the character",
      Explicit = true)]
    public async Task RepositoryOrdersByName() {
      // Arrange
      DbContext.Characters.AddRange(Enumerable.Range(0, 10)
        .Select(_ => Guid.CreateVersion7())
        .Select(characterId => new CharacterDbEntity {
          Id = characterId,
          Name = characterId.ToString(),
          PlayerName = string.Empty,
          Specie = new SpecieDbEntity {
            Name = Guid.CreateVersion7().ToString(),
            Speed = 0,
          },
          CharacterClasses = Enumerable.Range(0, 3)
            .Select(index => new CharacterClassDbEntity {
              IsMainClass = index == 0,
              Class = new ClassDbEntity {
                Name = Guid.CreateVersion7().ToString(),
              },
            })
            .ToList(),
        }));
      await DbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

      // Act
      PaginatedResult<Character> result = await _repository.GetCharactersAsync(
        Page.Infinite,
        Order.CreateAscending(nameof(Character.Name)),
        new CharacterFilter(),
        TestContext.Current.CancellationToken);

      // Assert
      List<Guid> dbCharactersId = await DbContext.Characters
        .OrderBy(c => c.Name)
        .Select(c => c.Id)
        .ToListAsync(TestContext.Current.CancellationToken);

      result.Values.Select(c => c.Id).Should().BeEquivalentTo(dbCharactersId);
    }

    [Fact(
      DisplayName = "Orders descending by the name of the character",
      Explicit = true)]
    public async Task RepositoryOrdersDescendingByName() {
      // Arrange
      DbContext.Characters.AddRange(Enumerable.Range(0, 10)
        .Select(_ => Guid.CreateVersion7())
        .Select(characterId => new CharacterDbEntity {
          Id = characterId,
          Name = characterId.ToString(),
          PlayerName = string.Empty,
          Specie = new SpecieDbEntity {
            Name = Guid.CreateVersion7().ToString(),
            Speed = 0,
          },
          CharacterClasses = Enumerable.Range(0, 3)
            .Select(index => new CharacterClassDbEntity {
              IsMainClass = index == 0,
              Class = new ClassDbEntity {
                Name = Guid.CreateVersion7().ToString(),
              },
            })
            .ToList(),
        }));
      await DbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

      // Act
      PaginatedResult<Character> result = await _repository.GetCharactersAsync(
        Page.Infinite,
        Order.CreateDescending(nameof(Character.Name)),
        new CharacterFilter(),
        TestContext.Current.CancellationToken);

      // Assert
      List<Guid> dbCharactersId = await DbContext.Characters
        .OrderByDescending(c => c.Name)
        .Select(c => c.Id)
        .ToListAsync(TestContext.Current.CancellationToken);

      result.Values.Select(c => c.Id).Should().BeEquivalentTo(dbCharactersId);
    }

    [Fact(
      DisplayName = "Orders by the name of the specie",
      Explicit = true)]
    public async Task RepositoryOrdersBySpecie() {
      // Arrange
      DbContext.Characters.AddRange(Enumerable.Range(0, 10)
        .Select(_ => Guid.CreateVersion7())
        .Select(characterId => new CharacterDbEntity {
          Id = characterId,
          Name = characterId.ToString(),
          PlayerName = string.Empty,
          Specie = new SpecieDbEntity {
            Name = Guid.CreateVersion7().ToString(),
            Speed = 0,
          },
          CharacterClasses = Enumerable.Range(0, 3)
            .Select(index => new CharacterClassDbEntity {
              IsMainClass = index == 0,
              Class = new ClassDbEntity {
                Name = Guid.CreateVersion7().ToString(),
              },
            })
            .ToList(),
        }));
      await DbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

      // Act
      PaginatedResult<Character> result = await _repository.GetCharactersAsync(
        Page.Infinite,
        Order.CreateAscending(nameof(Character.Specie)),
        new CharacterFilter(),
        TestContext.Current.CancellationToken);

      // Assert
      List<Guid> dbCharactersId = await DbContext.Characters
        .OrderBy(c => c.Specie!.Name)
        .Select(c => c.Id)
        .ToListAsync(TestContext.Current.CancellationToken);

      result.Values.Select(c => c.Id).Should().BeEquivalentTo(dbCharactersId);
    }

    [Fact(
      DisplayName = "Orders descending by the name of the specie",
      Explicit = true)]
    public async Task RepositoryOrdersDescendingBySpecie() {
      // Arrange
      DbContext.Characters.AddRange(Enumerable.Range(0, 10)
        .Select(_ => Guid.CreateVersion7())
        .Select(characterId => new CharacterDbEntity {
          Id = characterId,
          Name = characterId.ToString(),
          PlayerName = string.Empty,
          Specie = new SpecieDbEntity {
            Name = Guid.CreateVersion7().ToString(),
            Speed = 0,
          },
          CharacterClasses = Enumerable.Range(0, 3)
            .Select(index => new CharacterClassDbEntity {
              IsMainClass = index == 0,
              Class = new ClassDbEntity {
                Name = Guid.CreateVersion7().ToString(),
              },
            })
            .ToList(),
        }));
      await DbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

      // Act
      PaginatedResult<Character> result = await _repository.GetCharactersAsync(
        Page.Infinite,
        Order.CreateDescending(nameof(Character.Specie)),
        new CharacterFilter(),
        TestContext.Current.CancellationToken);

      // Assert
      List<Guid> dbCharactersId = await DbContext.Characters
        .OrderByDescending(c => c.Specie!.Name)
        .Select(c => c.Id)
        .ToListAsync(TestContext.Current.CancellationToken);

      result.Values.Select(c => c.Id).Should().BeEquivalentTo(dbCharactersId);
    }

    [Fact(
      DisplayName = "Orders by the name of the main class",
      Explicit = true)]
    public async Task RepositoryOrdersByMainClass() {
      // Arrange
      DbContext.Characters.AddRange(Enumerable.Range(0, 10)
        .Select(_ => Guid.CreateVersion7())
        .Select(characterId => new CharacterDbEntity {
          Id = characterId,
          Name = characterId.ToString(),
          PlayerName = string.Empty,
          Specie = new SpecieDbEntity {
            Name = Guid.CreateVersion7().ToString(),
            Speed = 0,
          },
          CharacterClasses = Enumerable.Range(0, 3)
            .Select(index => new CharacterClassDbEntity {
              IsMainClass = index == 0,
              Class = new ClassDbEntity {
                Name = Guid.CreateVersion7().ToString(),
              },
            })
            .ToList(),
        }));
      await DbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

      // Act
      PaginatedResult<Character> result = await _repository.GetCharactersAsync(
        Page.Infinite,
        Order.CreateAscending(nameof(Character.MainClass)),
        new CharacterFilter(),
        TestContext.Current.CancellationToken);

      // Assert
      List<Guid> dbCharactersId = await DbContext.Characters
        .OrderBy(c => c.CharacterClasses.Single(cc => cc.IsMainClass).Class!.Name)
        .Select(c => c.Id)
        .ToListAsync(TestContext.Current.CancellationToken);

      result.Values.Select(c => c.Id).Should().BeEquivalentTo(dbCharactersId);
    }

    [Fact(
      DisplayName = "Orders descending by the name of the main class",
      Explicit = true)]
    public async Task RepositoryOrdersDescendingByMainClass() {
      // Arrange
      DbContext.Characters.AddRange(Enumerable.Range(0, 10)
        .Select(_ => Guid.CreateVersion7())
        .Select(characterId => new CharacterDbEntity {
          Id = characterId,
          Name = characterId.ToString(),
          PlayerName = string.Empty,
          Specie = new SpecieDbEntity {
            Name = Guid.CreateVersion7().ToString(),
            Speed = 0,
          },
          CharacterClasses = Enumerable.Range(0, 3)
            .Select(index => new CharacterClassDbEntity {
              IsMainClass = index == 0,
              Class = new ClassDbEntity {
                Name = Guid.CreateVersion7().ToString(),
              },
            })
            .ToList(),
        }));
      await DbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

      // Act
      PaginatedResult<Character> result = await _repository.GetCharactersAsync(
        Page.Infinite,
        Order.CreateDescending(nameof(Character.MainClass)),
        new CharacterFilter(),
        TestContext.Current.CancellationToken);

      // Assert
      List<Guid> dbCharactersId = await DbContext.Characters
        .OrderByDescending(c => c.CharacterClasses.Single(cc => cc.IsMainClass).Class!.Name)
        .Select(c => c.Id)
        .ToListAsync(TestContext.Current.CancellationToken);

      result.Values.Select(c => c.Id).Should().BeEquivalentTo(dbCharactersId);
    }

    [Fact(
      DisplayName = "Orders by character id",
      Explicit = true)]
    public async Task RepositoryOrdersById() {
      // Arrange
      DbContext.Characters.AddRange(Enumerable.Range(0, 10)
        .Select(_ => Guid.CreateVersion7())
        .Select(characterId => new CharacterDbEntity {
          Id = characterId,
          Name = characterId.ToString(),
          PlayerName = string.Empty,
          Specie = new SpecieDbEntity {
            Name = Guid.CreateVersion7().ToString(),
            Speed = 0,
          },
          CharacterClasses = Enumerable.Range(0, 3)
            .Select(index => new CharacterClassDbEntity {
              IsMainClass = index == 0,
              Class = new ClassDbEntity {
                Name = Guid.CreateVersion7().ToString(),
              },
            })
            .ToList(),
        }));
      await DbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

      // Act
      PaginatedResult<Character> result = await _repository.GetCharactersAsync(
        Page.Infinite,
        Order.CreateAscending(nameof(Character.Id)),
        new CharacterFilter(),
        TestContext.Current.CancellationToken);

      // Assert
      List<Guid> dbCharactersId = await DbContext.Characters
        .OrderBy(c => c.Id)
        .Select(c => c.Id)
        .ToListAsync(TestContext.Current.CancellationToken);

      result.Values.Select(c => c.Id).Should().BeEquivalentTo(dbCharactersId);
    }

    [Fact(
      DisplayName = "Orders descending by character id",
      Explicit = true)]
    public async Task RepositoryOrdersDescendingById() {
      // Arrange
      DbContext.Characters.AddRange(Enumerable.Range(0, 10)
        .Select(_ => Guid.CreateVersion7())
        .Select(characterId => new CharacterDbEntity {
          Id = characterId,
          Name = characterId.ToString(),
          PlayerName = string.Empty,
          Specie = new SpecieDbEntity {
            Name = Guid.CreateVersion7().ToString(),
            Speed = 0,
          },
          CharacterClasses = Enumerable.Range(0, 3)
            .Select(index => new CharacterClassDbEntity {
              IsMainClass = index == 0,
              Class = new ClassDbEntity {
                Name = Guid.CreateVersion7().ToString(),
              },
            })
            .ToList(),
        }));
      await DbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

      // Act
      PaginatedResult<Character> result = await _repository.GetCharactersAsync(
        Page.Infinite,
        Order.CreateDescending(nameof(Character.Id)),
        new CharacterFilter(),
        TestContext.Current.CancellationToken);

      // Assert
      List<Guid> dbCharactersId = await DbContext.Characters
        .OrderByDescending(c => c.Id)
        .Select(c => c.Id)
        .ToListAsync(TestContext.Current.CancellationToken);

      result.Values.Select(c => c.Id).Should().BeEquivalentTo(dbCharactersId);
    }

    [Fact(
      DisplayName = "Filters by part of the name",
      Explicit = true)]
    public async Task RepositoryFiltersByPartOfTheName() {
      // Arrange
      const string PartOfName = "Bring";
      DbContext.Characters.AddRange(
        new List<string> {
            $"Plague {PartOfName}er {Guid.CreateVersion7()}",
            Guid.CreateVersion7().ToString()
          }
          .Select(characterName => new CharacterDbEntity {
            Id = Guid.CreateVersion7(),
            Name = characterName,
            PlayerName = string.Empty,
            Specie = new SpecieDbEntity {
              Name = Guid.CreateVersion7().ToString(),
              Speed = 0,
            },
            CharacterClasses = [
              new CharacterClassDbEntity {
                IsMainClass = true,
                Class = new ClassDbEntity {
                  Name = Guid.CreateVersion7().ToString(),
                },
              }
            ],
          }));
      await DbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

      var characterFilter = new CharacterFilter {
        Name = PartOfName
      };

      // Act
      PaginatedResult<Character> result = await _repository.GetCharactersAsync(
        Page.Infinite,
        Order.CreateAscending(nameof(Character.Id)),
        characterFilter,
        TestContext.Current.CancellationToken);

      // Assert
      List<Guid> dbCharactersId = await DbContext.Characters
        .Where(c => c.Name.Contains(PartOfName))
        .Select(c => c.Id)
        .ToListAsync(TestContext.Current.CancellationToken);

      result.Values.Select(c => c.Id).Should().BeEquivalentTo(dbCharactersId);
    }

    [Fact(
      DisplayName = "Filters by part of the name, ignoring case",
      Explicit = true)]
    public async Task RepositoryFiltersByPartOfTheNameMatchingIgnoringCase() {
      // Arrange
      const string PartOfName = "BrINg";
      DbContext.Characters.AddRange(
        new List<string> {
            $"Plague {PartOfName.ToUpperInvariant()}ER {Guid.CreateVersion7()}",
            Guid.CreateVersion7().ToString()
          }
          .Select(characterName => new CharacterDbEntity {
            Id = Guid.CreateVersion7(),
            Name = characterName,
            PlayerName = string.Empty,
            Specie = new SpecieDbEntity {
              Name = Guid.CreateVersion7().ToString(),
              Speed = 0,
            },
            CharacterClasses = [
              new CharacterClassDbEntity {
                IsMainClass = true,
                Class = new ClassDbEntity {
                  Name = Guid.CreateVersion7().ToString(),
                },
              }
            ],
          }));
      await DbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

      var characterFilter = new CharacterFilter {
        Name = PartOfName
      };

      // Act
      PaginatedResult<Character> result = await _repository.GetCharactersAsync(
        Page.Infinite,
        Order.CreateAscending(nameof(Character.Id)),
        characterFilter,
        TestContext.Current.CancellationToken);

      // Assert
#pragma warning disable CA1304, CA1311, CA1862
      List<Guid> dbCharactersId = await DbContext.Characters
        .Where(c => c.Name.ToUpper().Contains(PartOfName.ToUpper()))
        .Select(c => c.Id)
        .ToListAsync(TestContext.Current.CancellationToken);
#pragma warning restore CA1304, CA1311, CA1862

      result.Values.Select(c => c.Id).Should().BeEquivalentTo(dbCharactersId);
    }

    [Fact(
      DisplayName = "Filters by the main class name being in set",
      Explicit = true)]
    public async Task RepositoryFiltersByMainClassNameInSet() {
      // Arrange
      string mainClassName = Guid.CreateVersion7().ToString();
      DbContext.Characters.AddRange(Enumerable.Range(0, 10)
        .Select(index => new CharacterDbEntity {
          Id = Guid.CreateVersion7(),
          Name = Guid.CreateVersion7().ToString(),
          PlayerName = string.Empty,
          Specie = new SpecieDbEntity {
            Name = Guid.CreateVersion7().ToString(),
            Speed = 0,
          },
          CharacterClasses = [
            new CharacterClassDbEntity {
              IsMainClass = true,
              Class = new ClassDbEntity {
                Name = index == 0 ? mainClassName : Guid.CreateVersion7().ToString(),
              },
            },
          ],
        }));
      await DbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

      var characterFilter = new CharacterFilter {
        Classes = [mainClassName, "Mage"],
      };

      // Act
      PaginatedResult<Character> result = await _repository.GetCharactersAsync(
        Page.Infinite,
        Order.CreateAscending(nameof(Character.Id)),
        characterFilter,
        TestContext.Current.CancellationToken);

      // Assert
      List<Guid> dbCharactersId = await DbContext.Characters
        .Where(c => c.CharacterClasses.Any(cc =>
          cc.IsMainClass && characterFilter.Classes.Contains(cc.Class!.Name)))
        .Select(c => c.Id)
        .ToListAsync(TestContext.Current.CancellationToken);

      result.Values.Select(c => c.Id).Should().BeEquivalentTo(dbCharactersId);
    }

    [Fact(
      DisplayName = "Filters by the main class name being in set, ignoring case",
      Explicit = true)]
    public async Task RepositoryFiltersByMainClassNameInSetMatchingIgnoringCase() {
      // Arrange
      string mainClassName = Guid.CreateVersion7().ToString();
      DbContext.Characters.AddRange(Enumerable.Range(0, 10)
        .Select(index => new CharacterDbEntity {
          Id = Guid.CreateVersion7(),
          Name = Guid.CreateVersion7().ToString(),
          PlayerName = string.Empty,
          Specie = new SpecieDbEntity {
            Name = Guid.CreateVersion7().ToString(),
            Speed = 0,
          },
          CharacterClasses = [
            new CharacterClassDbEntity {
              IsMainClass = true,
              Class = new ClassDbEntity {
                Name = index == 0 ? mainClassName : Guid.CreateVersion7().ToString(),
              },
            },
          ],
        }));
      await DbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

      var characterFilter = new CharacterFilter {
        Classes = [mainClassName.ToUpperInvariant(), "Mage"],
      };

      // Act
      PaginatedResult<Character> result = await _repository.GetCharactersAsync(
        Page.Infinite,
        Order.CreateAscending(nameof(Character.Id)),
        characterFilter,
        TestContext.Current.CancellationToken);

      // Assert
#pragma warning disable CA1304, CA1311, CA1862
      List<Guid> dbCharactersId = await DbContext.Characters
        .Where(c => c.CharacterClasses.Any(cc =>
          cc.IsMainClass && characterFilter.Classes
            .Select(name => name.ToUpper())
            .Contains(cc.Class!.Name.ToUpper())))
        .Select(c => c.Id)
        .ToListAsync(TestContext.Current.CancellationToken);
#pragma warning restore CA1304, CA1311, CA1862

      result.Values.Select(c => c.Id).Should().BeEquivalentTo(dbCharactersId);
    }

    [Fact(
      DisplayName = "Filters by the specie name being in set",
      Explicit = true)]
    public async Task RepositoryFiltersBySpecieNameInSet() {
      // Arrange
      string specieName = Guid.CreateVersion7().ToString();
      DbContext.Characters.AddRange(Enumerable.Range(0, 10)
        .Select(index => new CharacterDbEntity {
          Id = Guid.CreateVersion7(),
          Name = Guid.CreateVersion7().ToString(),
          PlayerName = string.Empty,
          Specie = new SpecieDbEntity {
            Name = index == 0 ? specieName : Guid.CreateVersion7().ToString(),
            Speed = 0,
          },
          CharacterClasses = [
            new CharacterClassDbEntity {
              IsMainClass = true,
              Class = new ClassDbEntity {
                Name = Guid.CreateVersion7().ToString(),
              },
            },
          ],
        }));
      await DbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

      var characterFilter = new CharacterFilter {
        Species = [specieName, "Dwarf"]
      };

      // Act
      PaginatedResult<Character> result = await _repository.GetCharactersAsync(
        Page.Infinite,
        Order.CreateAscending(nameof(Character.Id)),
        characterFilter,
        TestContext.Current.CancellationToken);

      // Assert
      List<Guid> dbCharactersId = await DbContext.Characters
        .Where(c => characterFilter.Species.Contains(c.Specie!.Name))
        .Select(c => c.Id)
        .ToListAsync(TestContext.Current.CancellationToken);

      result.Values.Select(c => c.Id).Should().BeEquivalentTo(dbCharactersId);
    }

    [Fact(
      DisplayName = "Filters by the specie name being in set, ignoring case",
      Explicit = true)]
    public async Task RepositoryFiltersBySpecieNameInSetIgnoringCase() {
      // Arrange
      string specieName = Guid.CreateVersion7().ToString();
      DbContext.Characters.AddRange(Enumerable.Range(0, 10)
        .Select(index => new CharacterDbEntity {
          Id = Guid.CreateVersion7(),
          Name = Guid.CreateVersion7().ToString(),
          PlayerName = string.Empty,
          Specie = new SpecieDbEntity {
            Name = index == 0 ? specieName : Guid.CreateVersion7().ToString(),
            Speed = 0,
          },
          CharacterClasses = [
            new CharacterClassDbEntity {
              IsMainClass = true,
              Class = new ClassDbEntity {
                Name = Guid.CreateVersion7().ToString(),
              },
            },
          ],
        }));
      await DbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

      var characterFilter = new CharacterFilter {
        Species = [specieName.ToUpperInvariant(), "Dwarf"]
      };

      // Act
      PaginatedResult<Character> result = await _repository.GetCharactersAsync(
        Page.Infinite,
        Order.CreateAscending(nameof(Character.Id)),
        characterFilter,
        TestContext.Current.CancellationToken);

      // Assert
#pragma warning disable CA1304, CA1311, CA1862
      List<Guid> dbCharactersId = await DbContext.Characters
        .Where(c => characterFilter.Species
          .Select(name => name.ToUpper())
          .Contains(c.Specie!.Name.ToUpper()))
        .Select(c => c.Id)
        .ToListAsync(TestContext.Current.CancellationToken);
#pragma warning restore CA1304, CA1311, CA1862

      result.Values.Select(c => c.Id).Should().BeEquivalentTo(dbCharactersId);
    }
  }
}