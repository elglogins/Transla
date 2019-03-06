using System.Collections.Generic;
using System.Threading.Tasks;
using Transla.Contracts;

namespace Transla.Service.Interfaces.Services
{
    public interface IApplicationService
    {
        Task Delete(string alias);
        Task<ApplicationContract> Get(string alias);
        Task<IEnumerable<ApplicationContract>> GetAll();
        Task Save(string alias);
    }
}
