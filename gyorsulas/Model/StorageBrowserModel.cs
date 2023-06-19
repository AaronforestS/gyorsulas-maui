using gyorsulas.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gyorsulas.Model
{
    public class StorageBrowserModel
    {
        public StorageBrowserModel(StorageInterface storage)
        {
            this.storage = storage;

            Storages = new List<StorageModel>();
        }

        private StorageInterface storage;

        public event EventHandler? StorageChanged;

        public List<StorageModel> Storages { get; private set; }
        private void OnStorageChanged()
        {
            StorageChanged?.Invoke(this, EventArgs.Empty);
        }

        public async Task Update()
        {
            if (storage == null) return;

            Storages.Clear();
            foreach (string name in await storage.GetFiles())
            {
                if (name == "unsaved") continue;

                Storages.Add(new StorageModel
                {
                    Name = name,
                    ModificationDate = await storage.GetModificationDate(name)
                });
            }

            Storages = Storages.OrderByDescending(item => item.ModificationDate).ToList();

            OnStorageChanged();
        }

    }
}
