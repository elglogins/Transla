using System.Collections.Generic;
using System.Threading.Tasks;
using Transla.Contracts;

namespace Transla.Core.Interfaces.Services
{
    public interface IDictionaryService
    {
        Task Delete(string cultureName, string application, string alias);
        Task<IEnumerable<DictionaryContract>> Get(string application, string alias);
        Task<DictionaryContract> Get(string application, string alias, string cultureName);
        Task<IEnumerable<DictionaryContract>> GetAll();
        Task<IEnumerable<DictionaryContract>> GetAll(string application);
        Task<IEnumerable<DictionaryContract>> GetAll(string application, string cultureName);
        Task Save(DictionaryContract contract);
    }
}