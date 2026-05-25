using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ClinicVets.Core.Interfaces;
using ClinicVets.Core.Models;

namespace ClinicVets.UI.ViewModels;

public record TodayVisitRow(string Time, string AnimalName, string Reason, string Cost);
public record LowStockItem(string Name, int Quantity);
public record VaccinationAlertItem(string AnimalName, string ChipNumber, string AnimalType);

public partial class DashboardViewModel : ViewModelBase
{
    private readonly IVisitService       _visits;
    private readonly IAnimalService      _animals;
    private readonly IMedicineService    _medicines;
    private readonly ICustomerRepository _customerRepo;

    [ObservableProperty] private string _greeting     = string.Empty;
    [ObservableProperty] private string _role         = string.Empty;
    [ObservableProperty] private string _dateStamp    = string.Empty;

    [ObservableProperty] private string _todayEarnings  = "₪0.00";
    [ObservableProperty] private string _monthEarnings  = "₪0.00";
    [ObservableProperty] private int    _todayVisits    = 0;
    [ObservableProperty] private int    _monthVisits    = 0;
    [ObservableProperty] private int    _totalAnimals   = 0;
    [ObservableProperty] private int    _totalCustomers = 0;

    [ObservableProperty] private bool _noTodayVisits    = false;
    [ObservableProperty] private bool _noLowStock       = false;
    [ObservableProperty] private bool _noVaccineAlerts  = false;

    public ObservableCollection<TodayVisitRow>        TodayRows     { get; } = new();
    public ObservableCollection<LowStockItem>         LowStockItems { get; } = new();
    public ObservableCollection<VaccinationAlertItem> VaccineAlerts { get; } = new();

    public DashboardViewModel(
        Employee user,
        IVisitService visits,
        IAnimalService animals,
        IMedicineService medicines,
        ICustomerRepository customerRepo)
    {
        _visits       = visits;
        _animals      = animals;
        _medicines    = medicines;
        _customerRepo = customerRepo;

        var hour = DateTime.Now.Hour;
        var time = hour < 12 ? "Good morning" : hour < 17 ? "Good afternoon" : "Good evening";
        Greeting  = $"{time}, {user.FullName}";
        Role      = user.Role.ToString();
        DateStamp = DateTime.Now.ToString("dddd, MMMM d yyyy");

        Load();
    }

    [RelayCommand]
    private void Refresh() => Load();

    private void Load()
    {
        var today     = DateTime.Today;
        var allVisits = _visits.GetAllVisits().ToList();
        var animalMap = _animals.GetAll().ToDictionary(a => a.Id);

        var todayV = allVisits.Where(v => v.VisitDateTime.Date == today).ToList();
        var monthV = allVisits.Where(v =>
            v.VisitDateTime.Year  == today.Year &&
            v.VisitDateTime.Month == today.Month).ToList();

        TodayVisits    = todayV.Count;
        MonthVisits    = monthV.Count;
        TodayEarnings  = $"₪{todayV.Sum(v => v.TotalCost):N2}";
        MonthEarnings  = $"₪{monthV.Sum(v => v.TotalCost):N2}";
        TotalAnimals   = animalMap.Count;
        TotalCustomers = _customerRepo.GetAll().Count();

        TodayRows.Clear();
        foreach (var v in todayV.OrderByDescending(v => v.VisitDateTime))
        {
            var name   = animalMap.TryGetValue(v.AnimalId, out var a) ? a.Name : $"#{v.AnimalId}";
            var reason = v.Reason.Length > 38 ? v.Reason[..38] + "…" : v.Reason;
            TodayRows.Add(new TodayVisitRow(
                v.VisitDateTime.ToString("HH:mm"),
                name,
                reason,
                $"₪{v.TotalCost:N2}"));
        }

        LowStockItems.Clear();
        foreach (var m in _medicines.GetAll().Where(m => m.Quantity <= 5).OrderBy(m => m.Quantity))
            LowStockItems.Add(new LowStockItem(m.Name, m.Quantity));

        VaccineAlerts.Clear();
        foreach (var a in animalMap.Values.Where(a => _animals.NeedsVaccination(a)).Take(8))
            VaccineAlerts.Add(new VaccinationAlertItem(a.Name, a.ChipNumber, a.Type.ToString()));

        NoTodayVisits   = TodayRows.Count    == 0;
        NoLowStock      = LowStockItems.Count == 0;
        NoVaccineAlerts = VaccineAlerts.Count == 0;
    }
}
