using System.Collections.Generic;

namespace Transla.Contracts
{
    public class ExportContract
    {
        public IEnumerable<CultureContract> Cultures { get; set; }
        public IEnumerable<ApplicationContract> Applications { get; set; }
        public IEnumerable<DictionaryContract> Dictionaries { get; set; }
    }
}
