using Ardent.Project.Application.Common.Behaviours;
using Ardent.Project.Application.Common.Models;
using Ardent.Project.Application.Products;
using Ardent.Project.Application.Products.ListProducts;
using FluentValidation;
using FluentValidation.Results;
using MediatR;

namespace Ardent.Project.Application.UnitTests.Common.Behaviours;

public sealed class ValidationBehaviourTests
{
    private readonly RequestHandlerDelegate<PagedResponse<ProductResponse>> _next =
        Substitute.For<RequestHandlerDelegate<PagedResponse<ProductResponse>>>();

    [Fact]
    public async Task Handle_NoValidationFailures_CallsNext()
    {
        ValidationBehaviour<ListProductsQuery, PagedResponse<ProductResponse>> behaviour = new([new ListProductsQueryValidator()]);

        await behaviour.Handle(new ListProductsQuery(1, 20), _next, TestContext.Current.CancellationToken);

        await _next.Received(1).Invoke(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_FailuresFromSeveralValidators_ThrowsOneExceptionWithAllFailures()
    {
        IValidator<ListProductsQuery> extraValidator = Substitute.For<IValidator<ListProductsQuery>>();
        extraValidator.ValidateAsync(Arg.Any<IValidationContext>(), Arg.Any<CancellationToken>())
            .Returns(new ValidationResult([new ValidationFailure("Page", "Extra rule failed.")]));
        ValidationBehaviour<ListProductsQuery, PagedResponse<ProductResponse>> behaviour =
            new([new ListProductsQueryValidator(), extraValidator]);

        ValidationException exception = await Should.ThrowAsync<ValidationException>(() =>
            behaviour.Handle(new ListProductsQuery(0, 20), _next, TestContext.Current.CancellationToken));

        exception.Errors.Count().ShouldBe(2);
        await _next.DidNotReceive().Invoke(Arg.Any<CancellationToken>());
    }
}
