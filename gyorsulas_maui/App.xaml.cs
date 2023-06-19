using gyorsulas.Persistence;
using gyorsulas.Model;
using gyorsulas_wpf.ViewModel;
using gyorsulas_maui.Persistence;

namespace gyorsulas_maui;

public partial class App : Application
{
    private AppShell appShell;
    private GyorsulasModel model;
    private GyorsulasViewModel vm;
    private GyorsulasFileAccessInterface fileAccess;
    private StorageInterface storageInterface;


    public App()
	{
		InitializeComponent();

        fileAccess = new GyorsulasFileAccess(FileSystem.AppDataDirectory);
        model = new GyorsulasModel(fileAccess);
        vm = new GyorsulasViewModel(model);
        storageInterface = new GyorsulasStorage();

        appShell = new AppShell(storageInterface, fileAccess, model, vm)
        {
            BindingContext = vm
        };

		MainPage = appShell;
	}

    protected override Window CreateWindow(IActivationState activationState)
    {
        Window window = base.CreateWindow(activationState);

        window.Created += (s, e) =>
        {
            model.NewGame();
        };

        return window;
    }
}
