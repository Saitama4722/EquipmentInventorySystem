using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using EquipmentInventorySystem.Data.Repositories;
using EquipmentInventorySystem.ViewModels;

namespace EquipmentInventorySystem.Views;

public partial class RoomsPage : UserControl
{
    private readonly RoomViewModel _viewModel = new();

    public RoomsPage()
    {
        InitializeComponent();
        DataContext = _viewModel;
        LoadData();
    }

    private void LoadData() => _viewModel.LoadItems(GetSearchText());

    private string? GetSearchText()
    {
        var text = SearchBox.Text?.Trim();
        return string.IsNullOrEmpty(text) ? null : text;
    }

    private void SearchButton_Click(object sender, RoutedEventArgs e) => LoadData();

    private void ClearButton_Click(object sender, RoutedEventArgs e)
    {
        SearchBox.Text = string.Empty;
        LoadData();
    }

    private void SearchBox_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter) LoadData();
    }

    private void RefreshButton_Click(object sender, RoutedEventArgs e) => LoadData();

    private void AddButton_Click(object sender, RoutedEventArgs e)
    {
        var window = new AddEditRoomWindow { Owner = Window.GetWindow(this) };
        if (window.ShowDialog() == true)
            LoadData();
    }

    private void EditButton_Click(object sender, RoutedEventArgs e)
    {
        if (_viewModel.SelectedItem is null)
        {
            MessageBox.Show("Выберите помещение для редактирования.",
                "Редактирование", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        OpenEditWindow(_viewModel.SelectedItem.Id);
    }

    private void RoomsGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (_viewModel.SelectedItem is not null)
            OpenEditWindow(_viewModel.SelectedItem.Id);
    }

    private void OpenEditWindow(int roomId)
    {
        var room = new RoomRepository().GetById(roomId);
        if (room is null) return;

        var window = new AddEditRoomWindow(room) { Owner = Window.GetWindow(this) };
        if (window.ShowDialog() == true)
            LoadData();
    }

    private void DeleteButton_Click(object sender, RoutedEventArgs e)
    {
        if (_viewModel.SelectedItem is null)
        {
            MessageBox.Show("Выберите помещение для удаления.",
                "Удаление", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        var selected = _viewModel.SelectedItem;
        var confirm = MessageBox.Show(
            $"Удалить помещение «{selected.Number} – {selected.Name}»?\n\nДействие нельзя отменить.",
            "Подтверждение удаления",
            MessageBoxButton.YesNo,
            MessageBoxImage.Warning,
            MessageBoxResult.No);

        if (confirm != MessageBoxResult.Yes) return;

        try
        {
            new RoomRepository().Delete(selected.Id);
            LoadData();
        }
        catch (Exception ex)
        {
            string message = ex.Message.Contains("FOREIGN KEY", StringComparison.OrdinalIgnoreCase)
                ? "Невозможно удалить помещение: оно фигурирует в истории перемещений.\n\n" +
                  "Сначала удалите или измените связанные записи в истории перемещений."
                : $"Ошибка удаления: {ex.Message}";

            MessageBox.Show(message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
