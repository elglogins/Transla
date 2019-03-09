using Transla.Service.Interfaces.Configurations;

namespace Transla.Service.Configurations
{
    internal class ManagementConfiguration : IManagementConfiguration
    {
        public string AdministrationApiKey { get; set; }
    }
}
