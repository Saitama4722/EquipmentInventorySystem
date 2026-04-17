using System.Windows;
using System.Windows.Controls;
using EquipmentInventorySystem.ViewModels;
using EquipmentInventorySystem.Views;

namespace EquipmentInventorySystem;

public partial class MainWindow : Window
{
    private readonly MainViewModel _viewModel = new();

    private readonly Dictionary<string, UserControl> _pages;

    public MainWindow()
    {
        InitializeComponent();

        _pages = new Dictionary<string, UserControl>
        {
            ["Home"]            = new HomePage(),
            ["Equipment"]       = new EquipmentPage(),
            ["Employees"]       = new EmployeesPage(),
            ["Rooms"]           = new RoomsPage(),
            ["InventoryChecks"] = new InventoryChecksPage(),
            ["MovementHistory"] = new MovementHistoryPage(),
            ["Reports"]         = new ReportsPage()
        };

        DataContext = _viewModel;
        _viewModel.CurrentView = _pages["Home"];
    }

    private void NavButton_Checked(object sender, RoutedEventArgs e)
    {
        if (sender is not RadioButton btn || btn.Tag is not string key) return;
        if (!_pages.TryGetValue(key, out var page)) return;

        _viewModel.CurrentView = page;

        if (key == "MovementHistory" && page is MovementHistoryPage historyPage)
            historyPage.LoadData();
    }
}
