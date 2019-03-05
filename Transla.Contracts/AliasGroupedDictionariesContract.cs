using System.Collections.Generic;

namespace Transla.Contracts
{
    public class AliasGroupedDictionariesContract
    {
        public string Key => $"{Service}.{Alias}";
        public string Alias { get; set; }
        public string Service { get; set; }
        public IEnumerable<DictionaryContract> Dictionaries { get; set; }
    }
}
