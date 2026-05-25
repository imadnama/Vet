using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.VisualTree;
using ClinicVets.Core.Models;
using ClinicVets.UI.ViewModels;

namespace ClinicVets.UI.Views;

public partial class OpenVisitView : UserControl
{
    public OpenVisitView()
    {
        InitializeComponent();
        MedicineItemsControl.ContainerPrepared += OnMedicineContainerPrepared;
    }

    private void OnMedicineContainerPrepared(object? sender, ContainerPreparedEventArgs e)
    {
        var cb = e.Container.GetVisualDescendants()
                            .OfType<CheckBox>()
                            .FirstOrDefault();
        if (cb is null) return;
        cb.IsCheckedChanged += OnMedicineChecked;
    }

    private void OnMedicineChecked(object? sender, RoutedEventArgs e)
    {
        if (sender is not CheckBox cb) return;
        if (cb.Tag is not Medicine med) return;
        if (DataContext is not OpenVisitViewModel vm) return;
        vm.ToggleMedicine(med, cb.IsChecked == true);
    }
}
