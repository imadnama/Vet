using ClinicVets.App.Forms;
using ClinicVets.Core.Interfaces;
using ClinicVets.Core.Session;
using ClinicVets.Data;
using ClinicVets.Data.Repositories;
using ClinicVets.g1.Services;
using ClinicVets.g1.Forms;
using ClinicVets.g2.Services;
using ClinicVets.g3.Services;

namespace ClinicVets.App;

static class Program
{
    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();

        // ── Data layer ──────────────────────────────────────────────────────
        var db = new DatabaseContext("clinicvets.db");

        IEmployeeRepository employeeRepo = new EmployeeRepository(db);
        ICustomerRepository customerRepo = new CustomerRepository(db);
        IAnimalRepository   animalRepo   = new AnimalRepository(db);
        IVisitRepository    visitRepo    = new VisitRepository(db);
        IMedicineRepository medicineRepo = new MedicineRepository(db);

        // ── Service layer ───────────────────────────────────────────────────
        IAuthService     authService     = new AuthService(employeeRepo);
        ICustomerService customerService = new CustomerService(customerRepo, animalRepo);
        IAnimalService   animalService   = new AnimalService(animalRepo, customerRepo);
        IVisitService    visitService    = new VisitService(visitRepo, animalRepo);
        IMedicineService medicineService = new MedicineService(medicineRepo);

        // ── Navigation callback passed to LoginForm ─────────────────────────
        // LoginForm calls this action on successful login, so g1 never needs
        // a direct reference to MainForm (avoids a circular project dependency).
        LoginForm? loginForm = null;
        void OnLoginSuccess()
        {
            var main = new MainForm(
                authService,
                customerService,
                animalService,
                visitService,
                medicineService,
                customerRepo);

            main.FormClosed += (_, _) =>
            {
                CurrentUserSession.Clear();
                loginForm?.Show(); // return to login screen on logout
            };

            loginForm?.Hide();
            main.Show();
        }

        loginForm = new LoginForm(authService, OnLoginSuccess);
        Application.Run(loginForm);
    }
}
