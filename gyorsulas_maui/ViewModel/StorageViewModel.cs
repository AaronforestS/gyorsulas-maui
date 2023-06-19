using gyorsulas_wpf.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gyorsulas_maui.ViewModel
{
    public class StorageViewModel : ViewModelBase
    {
        private string name = string.Empty;
        private DateTime modificationDate;

        public string Name
        { get { return name; } set {if (name != value){name = value;OnPropertyChanged();}}}

        public DateTime ModificationDate{get { return modificationDate; }set{
                if (modificationDate != value)
                {modificationDate = value;OnPropertyChanged();}}}

        public DelegateCommand SaveGameCommand { get; set; }
        public DelegateCommand LoadGameCommand { get; set; }
    }
}
