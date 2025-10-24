using FluentAssertions;
using FluentAssertions.Primitives;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SimplifiedDnd.WebApi.FunctionalTests.Extensions;

internal static class ProblemDetailsAssertionsExtensions {
  internal static void BeValidationError(
    this ObjectAssertions assertions
  ) {
    ProblemDetails? problemDetails = assertions.Subject.Should().BeOfType<ProblemDetails>().Subject;
    problemDetails.Should().NotBeNull();
    problemDetails.Title.Should().Be("Validation.General");
    problemDetails.Status.Should().Be(StatusCodes.Status400BadRequest);
    problemDetails.Detail.Should().Be("One or more validation errors occurred");
    problemDetails.Extensions.Should().NotBeNullOrEmpty();
  }
}