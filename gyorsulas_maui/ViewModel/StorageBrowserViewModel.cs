using gyorsulas.Model;
using gyorsulas_wpf.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gyorsulas_maui.ViewModel
{
    public class StorageBrowserViewModel : ViewModelBase
    {
        private StorageBrowserModel model;

        public ObservableCollection<StorageViewModel> Storages { get; set; }

        public event EventHandler<StorageEventArgs> Loading;

        public event EventHandler<StorageEventArgs> Saving;

        public DelegateCommand NewSaveCommand { get; private set; }

        public StorageBrowserViewModel(StorageBrowserModel model)
        {
            if (model == null) throw new ArgumentNullException("Storage browser model is null!");
            this.model = model;

            Storages = new ObservableCollection<StorageViewModel>();

            this.model.StorageChanged += new EventHandler(Storage_Changed);

            NewSaveCommand = new DelegateCommand(p =>
            {
                string? file = Path.GetFileNameWithoutExtension(p?.ToString()?.Trim());
                if (!string.IsNullOrEmpty(file))
                {
                    file += ".gy";
                    OnSaving(file);
                }
            });
            UpdateStorage();
        }

        private void UpdateStorage()
        {
            Storages.Clear();

            foreach (StorageModel storagemodel in model.Storages)
            {
                Storages.Add(new StorageViewModel
                {
                    Name = storagemodel.Name,
                    ModificationDate = storagemodel.ModificationDate,
                    LoadGameCommand = new DelegateCommand(p => OnLoading(p?.ToString() ?? "")),
                    SaveGameCommand = new DelegateCommand(p => OnSaving(p?.ToString() ?? ""))
                });
            }
        }

        private void OnLoading(string v)
        {
            Loading?.Invoke(this, new StorageEventArgs { Name = v });
        }

        private void OnSaving(string file)
        {
            Saving?.Invoke(this, new StorageEventArgs { Name = file });
        }

        private void Storage_Changed(object sender, EventArgs e)
        {
            UpdateStorage();
        }
    }
}
