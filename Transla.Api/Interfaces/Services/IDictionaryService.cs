using System.Collections.Generic;
using System.Threading.Tasks;
using Transla.Api.Contracts;

namespace Transla.Api.Services
{
    public interface IDictionaryService
    {
        Task Delete(string cultureName, string service, string alias);
        Task<IEnumerable<DictionaryContract>> Get(string service, string alias);
        Task<DictionaryContract> Get(string service, string alias, string cultureName);
        Task<IEnumerable<DictionaryContract>> GetAll();
        Task<IEnumerable<DictionaryContract>> GetAll(string service);
        Task<IEnumerable<DictionaryContract>> GetAll(string service, string cultureName);
        Task Save(DictionaryContract contract);
    }
}