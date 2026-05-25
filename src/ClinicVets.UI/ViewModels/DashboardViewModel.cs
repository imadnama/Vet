using CommunityToolkit.Mvvm.ComponentModel;
using ClinicVets.Core.Models;

namespace ClinicVets.UI.ViewModels;

public partial class DashboardViewModel : ViewModelBase
{
    [ObservableProperty] private string _greeting     = string.Empty;
    [ObservableProperty] private string _role         = string.Empty;
    [ObservableProperty] private string _dateStamp    = string.Empty;

    public DashboardViewModel(Employee user)
    {
        var hour = DateTime.Now.Hour;
        var time = hour < 12 ? "Good morning" : hour < 17 ? "Good afternoon" : "Good evening";
        Greeting   = $"{time}, {user.FullName}";
        Role       = user.Role.ToString();
        DateStamp  = DateTime.Now.ToString("dddd, MMMM d yyyy");
    }
}
