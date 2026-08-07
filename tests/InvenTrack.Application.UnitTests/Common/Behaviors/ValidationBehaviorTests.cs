namespace InvenTrack.Application.UnitTests.Common.Behaviors;

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using InvenTrack.Application.Common.Behaviors;
using MediatR;
using Moq;
using Xunit;

public class ValidationBehaviorTests
{
    private readonly Mock<IValidator<TestRequest>> _validator = new();
    private readonly Mock<RequestHandlerDelegate<TestResponse>> _next = new();
    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    private ValidationBehavior<TestRequest, TestResponse> CreateBehavior(IEnumerable<IValidator<TestRequest>> validators)
        => new(validators);

    [Fact]
    public async Task Handle_ShouldCallNext_WhenNoValidators()
    {
        var behavior = CreateBehavior(Enumerable.Empty<IValidator<TestRequest>>());
        _next.Setup(n => n()).ReturnsAsync(new TestResponse());

        await behavior.Handle(new TestRequest(), _next.Object, _cancellationToken);

        _next.Verify(n => n(), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldCallNext_WhenValidationSucceeds()
    {
        _validator
            .Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<TestRequest>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        var behavior = CreateBehavior(new[] { _validator.Object });
        _next.Setup(n => n()).ReturnsAsync(new TestResponse());

        await behavior.Handle(new TestRequest(), _next.Object, _cancellationToken);

        _next.Verify(n => n(), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldThrowValidationException_WhenValidationFails()
    {
        var failure = new ValidationFailure("Name", "Name is required.");
        _validator
            .Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<TestRequest>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(new[] { failure }));

        var behavior = CreateBehavior(new[] { _validator.Object });

        Func<Task> act = async () => await behavior.Handle(new TestRequest(), _next.Object, _cancellationToken);

        await act.Should().ThrowAsync<ValidationException>();
        _next.Verify(n => n(), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldAggregateFailures_FromMultipleValidators()
    {
        var validator2 = new Mock<IValidator<TestRequest>>();
        var failures = new[]
        {
            new ValidationFailure("Name", "Name is required."),
            new ValidationFailure("Age", "Age is required.")
        };
        _validator
            .Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<TestRequest>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(failures.Take(1)));
        validator2
            .Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<TestRequest>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(failures.Skip(1)));

        var behavior = CreateBehavior(new[] { _validator.Object, validator2.Object });

        Func<Task> act = async () => await behavior.Handle(new TestRequest(), _next.Object, _cancellationToken);

        var exception = await act.Should().ThrowAsync<ValidationException>();
        exception.And.Errors.Should().HaveCount(2);
        _next.Verify(n => n(), Times.Never);
    }

    public class TestRequest : IRequest<TestResponse>;

    public class TestResponse;
}
