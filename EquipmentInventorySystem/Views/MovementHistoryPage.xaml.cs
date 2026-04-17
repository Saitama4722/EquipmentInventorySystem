using System.Windows.Controls;
using EquipmentInventorySystem.ViewModels;

namespace EquipmentInventorySystem.Views;

public partial class MovementHistoryPage : UserControl
{
    private readonly MovementHistoryViewModel _viewModel = new();

    public MovementHistoryPage()
    {
        InitializeComponent();
        DataContext = _viewModel;
        LoadData();
    }

    public void LoadData() => _viewModel.LoadItems();

    private void RefreshButton_Click(object sender, System.Windows.RoutedEventArgs e)
        => LoadData();
}
