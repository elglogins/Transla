using System.Threading.Tasks;

namespace Transla.Client.Services
{
    public interface IDictionaryService
    {
        string Get(string alias, string cultureName);
        string Get(string alias);
    }
}