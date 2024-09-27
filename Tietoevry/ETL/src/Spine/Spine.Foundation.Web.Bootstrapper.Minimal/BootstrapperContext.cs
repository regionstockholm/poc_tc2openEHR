public class BootstrapperContext : IBootstrapperContext
{
    private readonly List<ServiceDescription> _services = new();

    public List<ServiceDescription> Services { get => _services; }
}
