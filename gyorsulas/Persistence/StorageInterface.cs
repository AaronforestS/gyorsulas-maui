using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gyorsulas.Persistence
{
    public interface StorageInterface
    {
        Task<IEnumerable<string>> GetFiles();

        Task<DateTime> GetModificationDate(string name);
    }
}
