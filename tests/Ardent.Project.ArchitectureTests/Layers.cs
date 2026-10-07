using ArchUnitNET.Domain;
using ArchUnitNET.Loader;
using Ardent.Project.Api.Controllers;
using Ardent.Project.Domain.Products;
using Ardent.Project.Infrastructure.Persistence;
using static ArchUnitNET.Fluent.ArchRuleDefinition;
using Assembly = System.Reflection.Assembly;

namespace Ardent.Project.ArchitectureTests;

/// <summary>The four layers, loaded once and shared by every architecture test.</summary>
internal static class Layers
{
    public static readonly Assembly DomainAssembly = typeof(Product).Assembly;
    public static readonly Assembly ApplicationAssembly = typeof(Application.DependencyInjection).Assembly;
    public static readonly Assembly InfrastructureAssembly = typeof(ApplicationDbContext).Assembly;
    public static readonly Assembly ApiAssembly = typeof(ProductsController).Assembly;

    public static readonly Architecture Architecture = new ArchLoader()
        .LoadAssemblies(DomainAssembly, ApplicationAssembly, InfrastructureAssembly, ApiAssembly)
        .Build();

    public static readonly IObjectProvider<IType> Domain = Types().That().ResideInAssembly(DomainAssembly).As("Domain");
    public static readonly IObjectProvider<IType> Application = Types().That().ResideInAssembly(ApplicationAssembly).As("Application");
    public static readonly IObjectProvider<IType> Infrastructure = Types().That().ResideInAssembly(InfrastructureAssembly).As("Infrastructure");
    public static readonly IObjectProvider<IType> Api = Types().That().ResideInAssembly(ApiAssembly).As("Api");
}
