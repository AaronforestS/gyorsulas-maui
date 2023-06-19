using gyorsulas.Persistence;
using gyorsulas_wpf.ViewModel;
using gyorsulas.Model;
using System.IO;
using gyorsulas_maui.ViewModel;
using gyorsulas_maui.View;

namespace gyorsulas_maui;

public partial class AppShell : Shell
{
    private GyorsulasModel model;
    private GyorsulasViewModel vm;
    private GyorsulasFileAccessInterface fileAccess;
    private StorageInterface storageInterface;
    private StorageBrowserModel SBmodel;
    private StorageBrowserViewModel SBvm;

    public AppShell(StorageInterface storageInterface, GyorsulasFileAccessInterface fileAccess, GyorsulasModel model, GyorsulasViewModel vm)
	{
        InitializeComponent();

        this.storageInterface = storageInterface;
        this.fileAccess = fileAccess;
        this.vm = vm;
        this.model = model;

        SBmodel = new StorageBrowserModel(this.storageInterface);
        SBvm = new StorageBrowserViewModel(SBmodel);

        SBvm.Loading += SBvm_Loading;
        SBvm.Saving += SBvm_Saving;

        vm.GameEnd += vm_GameEnd;
        vm.Loading += vm_Loading;
        vm.Saving += vm_Saving;
	}

    private async void SBvm_Saving(object sender, StorageEventArgs e)
    {
        await Navigation.PopAsync();

        try
        {
            await model.SaveGyorsulasGame(e.Name);

            await Navigation.PopAsync();
            await DisplayAlert("Saving", "Successful saved", "OK");
        }
        catch
        {
            await DisplayAlert("Saving", "Saving failed", "OK");
        }
    }

    private async void SBvm_Loading(object sender, StorageEventArgs e)
    {
        await Navigation.PopAsync();

        try
        {
            await model.LoadGyorsulasGame(e.Name);

            await Navigation.PopAsync();
            await DisplayAlert("Loading", "Successful load", "OK");
        }
        catch
        {
            await DisplayAlert("Loading", "Loading failed", "OK");
        }
    }

    private void vm_GameEnd(object sender, GyorsulasArgs e)
    {
        MainThread.BeginInvokeOnMainThread(async () =>
        {
            await DisplayAlert("Game over", $"Your game time was {e._GameTime} second(s)", "OK");
        });  
    }

    private async void vm_Saving(object sender, EventArgs e)
    {
        await SBmodel.Update();
        await Navigation.PushAsync(new SavePage
        {
            BindingContext = SBvm
        });
    }

    private async void vm_Loading(object sender, EventArgs e)
    {
        await SBmodel.Update();
        await Navigation.PushAsync(new LoadPage
        {
            BindingContext = SBvm
        });
    }


}
