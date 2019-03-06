using System.Threading.Tasks;

namespace Transla.Client.Services
{
    interface IDictionaryService
    {
        Task<string> GetDictionaryValue(string alias, string cultureName);
    }
}