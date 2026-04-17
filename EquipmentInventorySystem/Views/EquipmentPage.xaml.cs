using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using EquipmentInventorySystem.Data.Repositories;
using EquipmentInventorySystem.Models;
using EquipmentInventorySystem.ViewModels;

namespace EquipmentInventorySystem.Views;

public partial class EquipmentPage : UserControl
{
    private readonly EquipmentViewModel _viewModel = new();

    public EquipmentPage()
    {
        InitializeComponent();
        DataContext = _viewModel;
        PopulateStatusFilter();
        LoadData();
    }

    private void PopulateStatusFilter()
    {
        StatusFilter.Items.Add(new ComboBoxItem { Content = "Все статусы", Tag = null });
        foreach (var (display, value) in EquipmentStatusHelper.AllStatuses)
            StatusFilter.Items.Add(new ComboBoxItem { Content = display, Tag = value });
        StatusFilter.SelectedIndex = 0;
    }

    private void LoadData()
    {
        _viewModel.LoadItems(GetSearchText(), GetStatusFilter());
    }

    private string? GetSearchText()
    {
        var text = SearchBox.Text?.Trim();
        return string.IsNullOrEmpty(text) ? null : text;
    }

    private EquipmentStatus? GetStatusFilter()
    {
        if (StatusFilter.SelectedItem is ComboBoxItem { Tag: EquipmentStatus status })
            return status;
        return null;
    }

    private void SearchButton_Click(object sender, RoutedEventArgs e) => LoadData();

    private void ClearButton_Click(object sender, RoutedEventArgs e)
    {
        SearchBox.Text = string.Empty;
        StatusFilter.SelectedIndex = 0;
        LoadData();
    }

    private void StatusFilter_SelectionChanged(object sender, SelectionChangedEventArgs e) => LoadData();

    private void SearchBox_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter) LoadData();
    }

    private void RefreshButton_Click(object sender, RoutedEventArgs e) => LoadData();

    private void AddButton_Click(object sender, RoutedEventArgs e)
    {
        var window = new AddEditEquipmentWindow { Owner = Window.GetWindow(this) };
        if (window.ShowDialog() == true)
            LoadData();
    }

    private void TransferButton_Click(object sender, RoutedEventArgs e)
    {
        if (_viewModel.SelectedItem is null)
        {
            MessageBox.Show("Выберите оборудование для перемещения.",
                "Перемещение", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        var window = new TransferEquipmentWindow(_viewModel.SelectedItem) { Owner = Window.GetWindow(this) };
        if (window.ShowDialog() == true)
            LoadData();
    }

    private void EditButton_Click(object sender, RoutedEventArgs e)
    {
        if (_viewModel.SelectedItem is null)
        {
            MessageBox.Show("Выберите оборудование для редактирования.",
                "Редактирование", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        OpenEditWindow(_viewModel.SelectedItem.Id);
    }

    private void EquipmentGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (_viewModel.SelectedItem is not null)
            OpenEditWindow(_viewModel.SelectedItem.Id);
    }

    private void OpenEditWindow(int equipmentId)
    {
        var equipment = new EquipmentRepository().GetById(equipmentId);
        if (equipment is null) return;

        var window = new AddEditEquipmentWindow(equipment) { Owner = Window.GetWindow(this) };
        if (window.ShowDialog() == true)
            LoadData();
    }

    private void DeleteButton_Click(object sender, RoutedEventArgs e)
    {
        if (_viewModel.SelectedItem is null)
        {
            MessageBox.Show("Выберите оборудование для удаления.",
                "Удаление", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        var selected = _viewModel.SelectedItem;
        var confirm = MessageBox.Show(
            $"Удалить оборудование «{selected.Name}» (инв. № {selected.InventoryNumber})?\n\nДействие нельзя отменить.",
            "Подтверждение удаления",
            MessageBoxButton.YesNo,
            MessageBoxImage.Warning,
            MessageBoxResult.No);

        if (confirm != MessageBoxResult.Yes) return;

        try
        {
            new EquipmentRepository().Delete(selected.Id);
            LoadData();
        }
        catch (Exception ex)
        {
            string message = ex.Message.Contains("FOREIGN KEY", StringComparison.OrdinalIgnoreCase)
                ? "Невозможно удалить оборудование: оно участвует в инвентаризациях или истории перемещений.\n\n" +
                  "Сначала удалите связанные записи или измените статус оборудования."
                : $"Ошибка удаления: {ex.Message}";

            MessageBox.Show(message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
