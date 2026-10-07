using ArchUnitNET.xUnitV3;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using static ArchUnitNET.Fluent.ArchRuleDefinition;

namespace Ardent.Project.ArchitectureTests;

public sealed class ConventionTests
{
    [Fact]
    public void Handlers_AreSealed()
    {
        Classes().That().Are(Layers.Application).And().HaveNameEndingWith("Handler")
            .Should().BeSealed()
            .Check(Layers.Architecture);
    }

    [Fact]
    public void Validators_AreSealed()
    {
        Classes().That().AreAssignableTo(typeof(IValidator))
            .Should().BeSealed()
            .Check(Layers.Architecture);
    }

    [Fact]
    public void CommandsQueriesAndResponses_AreRecords()
    {
        Classes().That().Are(Layers.Application)
            .And().HaveNameEndingWith("Command").Or().HaveNameEndingWith("Query").Or().HaveNameEndingWith("Response")
            .Should().BeRecord()
            .Check(Layers.Architecture);
    }

    [Fact]
    public void ApiContracts_AreRecords()
    {
        Classes().That().Are(Layers.Api).And().HaveNameEndingWith("Request")
            .Should().BeRecord()
            .Check(Layers.Architecture);
    }

    [Fact]
    public void Handlers_DoNotDispatchOtherRequests()
    {
        // Handlers get dependencies only through their constructors, so checking parameters is exact.
        Type[] dispatchers = [typeof(ISender), typeof(IPublisher), typeof(IMediator)];
        string[] offenders = [.. Layers.ApplicationAssembly.GetTypes()
            .Where(t => t.Name.EndsWith("Handler", StringComparison.Ordinal))
            .Where(t => t.GetConstructors().SelectMany(c => c.GetParameters()).Any(p => dispatchers.Contains(p.ParameterType)))
            .Select(t => t.Name)];

        offenders.ShouldBeEmpty("handlers must not send other requests; compose in the controller or extract a domain service");
    }

    [Fact]
    public void CommandsWithInput_EachHaveAValidator()
    {
        // A command whose only property is the route Id has nothing to validate: an unknown Id is a 404.
        Type[] applicationTypes = Layers.ApplicationAssembly.GetTypes();
        string[] commands = [.. applicationTypes
            .Where(t => t.IsAssignableTo(typeof(IBaseRequest)) && t.Name.EndsWith("Command", StringComparison.Ordinal))
            .Where(t => t.GetProperties().Any(p => p.Name != "Id"))
            .Select(t => t.Name)];
        string[] validated = [.. applicationTypes
            .Where(t => t.IsAssignableTo(typeof(IValidator)) && t.BaseType is { IsGenericType: true })
            .Select(t => t.BaseType!.GetGenericArguments()[0].Name)];

        commands.ShouldNotBeEmpty();
        commands.Except(validated).ShouldBeEmpty("every command needs an AbstractValidator<TCommand>");
    }

    [Fact]
    public void Controllers_ResideOnlyInApi()
    {
        Classes().That().AreAssignableTo(typeof(ControllerBase))
            .Should().Be(Layers.Api)
            .Check(Layers.Architecture);
    }
}
