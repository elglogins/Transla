using System.Collections.Generic;
using System.Threading.Tasks;
using Transla.Contracts;

namespace Transla.Service.Interfaces.Services
{
    public interface IApiKeyService
    {
        Task<bool> IsValid(string apiKey);
        Task<ApiKeyContract> Get(string apiKey);
        Task<IEnumerable<ApiKeyContract>> GetAll();
        Task Save(string apiKey, string applicationAlias);
        Task Delete(string apiKey);

    }
}
