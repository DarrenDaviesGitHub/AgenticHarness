using System.Reflection;
using ArchUnitNET.xUnitV3;
using static ArchUnitNET.Fluent.ArchRuleDefinition;

namespace Ardent.Project.ArchitectureTests;

/// <summary>Domain depends on nothing; Application on Domain; Infrastructure on Application; Api on Application (+ Infrastructure for DI).</summary>
public sealed class DependencyRuleTests
{
    [Fact]
    public void Domain_DoesNotDependOnAnyOtherLayer()
    {
        Types().That().Are(Layers.Domain)
            .Should().NotDependOnAny(Types().That().Are(Layers.Application).Or().Are(Layers.Infrastructure).Or().Are(Layers.Api))
            .Check(Layers.Architecture);
    }

    [Fact]
    public void Domain_ReferencesNoMicrosoftOrProjectAssemblies()
    {
        string[] references = [.. Layers.DomainAssembly.GetReferencedAssemblies().Select(a => a.Name!)];

        references.ShouldNotContain(name => name.StartsWith("Microsoft.", StringComparison.Ordinal));
        references.ShouldNotContain(name => name.StartsWith(RootNamespace, StringComparison.Ordinal));
    }

    [Fact]
    public void Application_DoesNotDependOnInfrastructureOrApi()
    {
        Types().That().Are(Layers.Application)
            .Should().NotDependOnAny(Types().That().Are(Layers.Infrastructure).Or().Are(Layers.Api))
            .Check(Layers.Architecture);
    }

    [Theory]
    [InlineData("Microsoft.EntityFrameworkCore")]
    [InlineData("Microsoft.AspNetCore")]
    public void Application_DoesNotReferenceFrameworkAssemblies(string forbiddenPrefix)
    {
        AssemblyName[] references = Layers.ApplicationAssembly.GetReferencedAssemblies();

        references.ShouldNotContain(a => a.Name!.StartsWith(forbiddenPrefix, StringComparison.Ordinal));
    }

    [Fact]
    public void Infrastructure_DoesNotReferenceAspNetCore()
    {
        // The project has a FrameworkReference for options/health checks; this keeps HTTP types out of it.
        AssemblyName[] references = Layers.InfrastructureAssembly.GetReferencedAssemblies();

        references.ShouldNotContain(a => a.Name!.StartsWith("Microsoft.AspNetCore", StringComparison.Ordinal));
    }

    [Fact]
    public void Infrastructure_DoesNotDependOnApi()
    {
        Types().That().Are(Layers.Infrastructure)
            .Should().NotDependOnAny(Layers.Api)
            .Check(Layers.Architecture);
    }

    [Fact]
    public void Api_UsesInfrastructureOnlyInCompositionRoot()
    {
        Types().That().Are(Layers.Api).And().DoNotHaveFullName("Program")
            .Should().NotDependOnAny(Layers.Infrastructure)
            .Check(Layers.Architecture);
    }

    private static string RootNamespace => typeof(DependencyRuleTests).Namespace!.Split('.')[0];
}
