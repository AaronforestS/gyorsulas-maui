using gyorsulas.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gyorsulas_maui.Persistence
{
    public  class GyorsulasStorage : StorageInterface
    {
        public async Task<IEnumerable<string>> GetFiles()
        {
            return await Task.Run(() => Directory.GetFiles(FileSystem.AppDataDirectory)
                .Select(Path.GetFileName)
                .Where(name => name?.EndsWith(".gy") ?? false)
                .OfType<string>());
        }

        public async Task<DateTime> GetModificationDate(string name)
        {
            var info = new FileInfo(Path.Combine(FileSystem.AppDataDirectory, name));

            return await Task.Run(() => info.LastWriteTime);
        }
    }
}
