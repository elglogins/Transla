using System.Collections.Generic;
using System.Threading.Tasks;
using Transla.Contracts;

namespace Transla.Service.Interfaces.Services
{
    public interface ICultureService
    {
        Task Delete(string cultureName);
        Task<CultureContract> Get(string cultureName);
        Task<IEnumerable<CultureContract>> GetAll();
        Task Save(string cultureName);
    }
}