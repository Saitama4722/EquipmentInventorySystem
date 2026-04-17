using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using EquipmentInventorySystem.ViewModels;

namespace EquipmentInventorySystem.Views;

public partial class InventoryChecksPage : UserControl
{
    private readonly InventoryCheckViewModel _viewModel = new();

    public InventoryChecksPage()
    {
        InitializeComponent();
        DataContext = _viewModel;
        LoadData();
    }

    private void LoadData()
    {
        _viewModel.LoadItems();
        int count = _viewModel.Items.Count;
        CheckCountText.Text = count > 0
            ? $"Всего инвентаризаций: {count}"
            : "Инвентаризаций пока не проводилось";
    }

    private void RefreshButton_Click(object sender, RoutedEventArgs e) => LoadData();

    private void NewCheckButton_Click(object sender, RoutedEventArgs e)
    {
        var window = new NewInventoryCheckWindow { Owner = Window.GetWindow(this) };
        if (window.ShowDialog() == true)
            LoadData();
    }

    private void ViewButton_Click(object sender, RoutedEventArgs e)
    {
        if (_viewModel.SelectedItem is null)
        {
            MessageBox.Show("Выберите инвентаризацию для просмотра.",
                "Просмотр", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        OpenDetailsWindow(_viewModel.SelectedItem);
    }

    private void ChecksGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (_viewModel.SelectedItem is not null)
            OpenDetailsWindow(_viewModel.SelectedItem);
    }

    private void OpenDetailsWindow(Models.InventoryCheckDisplayRow check)
    {
        var window = new InventoryCheckDetailsWindow(check) { Owner = Window.GetWindow(this) };
        window.ShowDialog();
    }
}
