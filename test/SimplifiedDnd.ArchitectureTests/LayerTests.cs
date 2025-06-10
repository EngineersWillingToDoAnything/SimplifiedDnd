using FluentAssertions;
using NetArchTest.Rules;
using SimplifiedDnd.Application.Abstractions.Core;
using SimplifiedDnd.DataBase;
using SimplifiedDnd.Domain.Characters;
using SimplifiedDnd.MigrationService;
using System.Reflection;
using TestResult = NetArchTest.Rules.TestResult;

namespace SimplifiedDnd.ArchitectureTests;

public sealed class LayerTests {
  private static readonly Assembly DomainAssembly = typeof(Character).Assembly;
  private static readonly Assembly ApplicationAssembly = typeof(DomainError).Assembly;
  private static readonly Assembly DataBaseAssembly = typeof(DataBaseDependencyInjection).Assembly;
  private static readonly Assembly ApiAssembly = typeof(Program).Assembly;
  private static readonly Assembly MigrationAssembly = typeof(IMigrationAssemblyMarker).Assembly;

  private sealed class UnexpectedDomainDependencies() : TheoryData<string>(
    ApplicationAssembly.GetName().Name!,
    DataBaseAssembly.GetName().Name!,
    ApiAssembly.GetName().Name!,
    MigrationAssembly.GetName().Name!);

  [Theory(DisplayName = "Domain layer doesn't depend on layer")]
  [ClassData(typeof(UnexpectedDomainDependencies))]
  public void DomainDoesNotHaveAnyDependencies(string assemblyName) {
    // Arrange & Act
    TestResult? result = Types.InAssembly(DomainAssembly)
      .ShouldNot()
      .HaveDependencyOn(assemblyName)
      .GetResult();

    // Assert
    result.IsSuccessful.Should().BeTrue();
  }
  
  private sealed class UnexpectedApplicationDependencies() : TheoryData<string>(
    DataBaseAssembly.GetName().Name!,
    ApiAssembly.GetName().Name!,
    MigrationAssembly.GetName().Name!);
  
  [Theory(DisplayName = "Application doesn't depend on layer")]
  [ClassData(typeof(UnexpectedApplicationDependencies))]
  public void ApplicationDoesNotHaveSpecifiedDependency(string assemblyName) {
    // Arrange & Act
    TestResult? result = Types.InAssembly(ApplicationAssembly)
      .ShouldNot()
      .HaveDependencyOn(assemblyName)
      .GetResult();

    // Assert
    result.IsSuccessful.Should().BeTrue();
  }
  
  private sealed class UnexpectedDataBaseDependencies() : TheoryData<string>(
    ApiAssembly.GetName().Name!,
    MigrationAssembly.GetName().Name!);
  
  [Theory(DisplayName = "Data base layer doesn't depends on layer")]
  [ClassData(typeof(UnexpectedDataBaseDependencies))]
  public void DataBaseDoesNotHaveSpecifiedDependency(string assemblyName) {
    // Arrange & Act
    TestResult? result = Types.InAssembly(DataBaseAssembly)
      .ShouldNot()
      .HaveDependencyOn(assemblyName)
      .GetResult();

    // Assert
    result.IsSuccessful.Should().BeTrue();
  }
  
  private sealed class UnexpectedApiDependencies() : TheoryData<string>(
    MigrationAssembly.GetName().Name!);
  
  [Theory(DisplayName = "API doesn't depends on layer")]
  [ClassData(typeof(UnexpectedApiDependencies))]
  public void ApiDoesNotHaveSpecifiedDependency(string assemblyName) {
    // Arrange & Act
    TestResult? result = Types.InAssembly(ApiAssembly)
      .ShouldNot()
      .HaveDependencyOn(assemblyName)
      .GetResult();

    // Assert
    result.IsSuccessful.Should().BeTrue();
  }
  
  
  private sealed class UnexpectedMigrationDependencies() : TheoryData<string>(
    MigrationAssembly.GetName().Name!);
  
  [Theory(DisplayName = "Migration background service doesn't depends on layer")]
  [ClassData(typeof(UnexpectedMigrationDependencies))]
  public void MigrationDoesNotHaveSpecifiedDependency(string assemblyName) {
    // Arrange & Act
    TestResult? result = Types.InAssembly(MigrationAssembly)
      .ShouldNot()
      .HaveDependencyOn(assemblyName)
      .GetResult();

    // Assert
    result.IsSuccessful.Should().BeTrue();
  }
}