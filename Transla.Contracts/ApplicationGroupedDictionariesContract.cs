using System.Collections.Generic;

namespace Transla.Contracts
{
    public class ApplicationGroupedDictionariesContract
    {
        public IEnumerable<GroupedCultureDictionariesContract> Dictionaries { get; set; }

        public string Alias { get; set; }

        public class GroupedCultureDictionariesContract
        {
            public string Alias { get; set; }
            public IEnumerable<DictionaryContract> Dictionaries { get; set; }
        }
    }
}
