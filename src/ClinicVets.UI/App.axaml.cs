using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using ClinicVets.Data;
using ClinicVets.Data.Repositories;
using ClinicVets.g1.Services;
using ClinicVets.g2.Services;
using ClinicVets.g3.Services;
using ClinicVets.UI.ViewModels;
using ClinicVets.UI.Views;

namespace ClinicVets.UI;

public class App : Application
{
    public override void Initialize() => AvaloniaXamlLoader.Load(this);

    public override void OnFrameworkInitializationCompleted()
    {
        var db             = new DatabaseContext("clinicvets.db");
        var employeeRepo   = new EmployeeRepository(db);
        var customerRepo   = new CustomerRepository(db);
        var animalRepo     = new AnimalRepository(db);
        var visitRepo      = new VisitRepository(db);
        var medicineRepo   = new MedicineRepository(db);

        var authService      = new AuthService(employeeRepo);
        var customerService  = new CustomerService(customerRepo, animalRepo);
        var animalService    = new AnimalService(animalRepo, customerRepo);
        var visitService     = new VisitService(visitRepo, animalRepo, medicineRepo);
        var medicineService  = new MedicineService(medicineRepo);

        var mainVm = new MainViewModel(
            authService, customerService, animalService,
            visitService, medicineService, customerRepo);

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new SplashWindow(mainVm);
        }

        base.OnFrameworkInitializationCompleted();
    }
}
