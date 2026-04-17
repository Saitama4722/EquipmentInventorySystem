using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using EquipmentInventorySystem.Data.Repositories;
using EquipmentInventorySystem.Models;

namespace EquipmentInventorySystem.Views;

public partial class NewInventoryCheckWindow : Window
{
    private readonly ObservableCollection<InventoryCheckItemRow> _items = [];

    public NewInventoryCheckWindow()
    {
        InitializeComponent();
        CheckItemsGrid.ItemsSource = _items;

        CheckDatePicker.SelectedDate = DateTime.Today;
        PopulateEmployeeBox();
        LoadEquipment();
    }

    private void PopulateEmployeeBox()
    {
        foreach (var emp in new EmployeeRepository().GetAll())
            EmployeeBox.Items.Add(new ComboBoxItem { Content = emp.FullName, Tag = emp.Id });

        if (EmployeeBox.Items.Count > 0)
            EmployeeBox.SelectedIndex = 0;
    }

    private void LoadEquipment()
    {
        var equipment = new EquipmentRepository().GetAllForDisplay(null, null);
        _items.Clear();

        foreach (var e in equipment)
        {
            _items.Add(new InventoryCheckItemRow
            {
                EquipmentId     = e.Id,
                Name            = e.Name,
                InventoryNumber = e.InventoryNumber,
                SerialNumber    = e.SerialNumber,
                StatusDisplay   = e.StatusDisplay,
                RoomDisplay     = e.RoomDisplay,
                EmployeeDisplay = e.EmployeeDisplay
            });
        }
    }

    private void Save_Click(object sender, RoutedEventArgs e)
    {
        if (!Validate()) return;

        var check = new InventoryCheck
        {
            CheckDate             = CheckDatePicker.SelectedDate!.Value,
            ResponsibleEmployeeId = GetSelectedEmployeeId(),
            Comment               = NullIfEmpty(CommentBox.Text),
            CreatedAt             = DateTime.Now
        };

        var items = _items.Select(row => new InventoryCheckItem
        {
            EquipmentId  = row.EquipmentId,
            ResultStatus = row.ResultStatus,
            Comment      = NullIfEmpty(row.Comment)
        }).ToList();

        try
        {
            new InventoryCheckRepository().Save(check, items);
            DialogResult = true;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка при сохранении:\n{ex.Message}",
                "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private bool Validate()
    {
        if (_items.Count == 0)
        {
            MessageBox.Show("Нет оборудования для инвентаризации. Сначала добавьте оборудование в систему.",
                "Проверка данных", MessageBoxButton.OK, MessageBoxImage.Warning);
            return false;
        }

        if (CheckDatePicker.SelectedDate is null)
        {
            MessageBox.Show("Укажите дату проверки.",
                "Проверка данных", MessageBoxButton.OK, MessageBoxImage.Warning);
            return false;
        }

        if (EmployeeBox.SelectedIndex < 0)
        {
            MessageBox.Show("Выберите ответственного сотрудника.",
                "Проверка данных", MessageBoxButton.OK, MessageBoxImage.Warning);
            EmployeeBox.Focus();
            return false;
        }

        return true;
    }

    private int GetSelectedEmployeeId()
    {
        if (EmployeeBox.SelectedItem is ComboBoxItem { Tag: int id })
            return id;
        throw new InvalidOperationException("No employee selected.");
    }

    private static string? NullIfEmpty(string? value)
        => string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    private void Cancel_Click(object sender, RoutedEventArgs e)
        => DialogResult = false;
}
