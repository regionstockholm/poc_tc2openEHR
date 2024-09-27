using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

public class BootstrapperBuilder
{
    private static BootstrapperContext BootstrapperContext { get; } = new BootstrapperContext();

    public static BootstrapperBuilder Instance = new();

    private BootstrapperBuilder() { }

    public static void Build(IServiceCollection services, string searchPattern)
    {
        var bootStrappers = GetBootStrappers(searchPattern);
        ActivateBootstrappers(bootStrappers, BootstrapperContext);
        MergeServices(services, BootstrapperContext.Services);
        CleanContext(BootstrapperContext);
    }

    private static void CleanContext(BootstrapperContext bootstrapperContext)
    {
        bootstrapperContext.Services.Clear();
    }

    private static void MergeServices(IServiceCollection services, IList<ServiceDescription> srcServices)
    {
        foreach (var item in srcServices)
        {
            services.Add(new ServiceDescriptor(item.Contract, item.Implmentaion,
                (ServiceLifetime)item.Lifetime));
        }
    }

    private static void ActivateBootstrappers(List<Type> bootStrappers, BootstrapperContext bootstrapperContext)
    {
        foreach (var bootStrapper in bootStrappers)
        {
            var bInstance = Activator.CreateInstance(bootStrapper) as IBootStrapper;
            bInstance?.Register(new ServiceRegister(bootstrapperContext));
        }
    }

    private static List<Type> GetBootStrappers(string searchPattern)
    {
        var assemblyNames = SearchAssemblies(searchPattern);
        return FindBootstrappers(assemblyNames);
    }

    private static List<Type> FindBootstrappers(List<string> assemblyNames)
    {
        var bootStrapperTypes = new List<Type>();
        _ = assemblyNames.Where(dll =>
        {
            var assembly = Assembly.LoadFrom(dll);
            var type = typeof(IBootStrapper);
            var types = assembly.GetTypes()
                .Where(p => type.IsAssignableFrom(p) && p.IsClass).ToList();

            if (types.Any()) bootStrapperTypes.AddRange(types);

            return true;
        }).ToList();
        return bootStrapperTypes;
    }

    private static List<string> SearchAssemblies(string searchPattern)
    {
        string path = Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory);
        var searchPatterns = $"TakeCare.*.dll;Spine.*.dll;{searchPattern}";
        var allAssemblies = new List<string>();
        foreach (var pattern in searchPatterns.Split(";", StringSplitOptions.RemoveEmptyEntries))
        {
            var searchedAssemblies = Directory.GetFiles(path, pattern, SearchOption.AllDirectories)
                .AsEnumerable();
            allAssemblies.AddRange(searchedAssemblies);
        }

        var assemblyNames = allAssemblies.Distinct().ToList();
        return assemblyNames;
    }
}
