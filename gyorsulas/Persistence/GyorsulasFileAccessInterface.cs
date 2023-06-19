using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gyorsulas.Persistence
{
    public interface GyorsulasFileAccessInterface
    {
        Task<GyorsulasData> GyorsulasLoad(string path);
        Task GyorsulasSave(string path, GyorsulasData data);
    }
}
