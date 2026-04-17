using System.Windows;
using System.Windows.Controls;
using EquipmentInventorySystem.Data.Repositories;
using EquipmentInventorySystem.Models;

namespace EquipmentInventorySystem.Views;

public partial class TransferEquipmentWindow : Window
{
    private readonly Equipment _equipment;

    public TransferEquipmentWindow(EquipmentDisplayRow row)
    {
        InitializeComponent();

        _equipment = new EquipmentRepository().GetById(row.Id)
            ?? throw new InvalidOperationException($"Equipment with Id={row.Id} not found.");

        EquipmentNameText.Text   = row.Name;
        InventoryText.Text       = row.InventoryNumber;
        CurrentRoomText.Text     = row.RoomDisplay;
        CurrentEmployeeText.Text = row.EmployeeDisplay;

        PopulateRoomBox();
        PopulateEmployeeBox();

        MovedAtPicker.SelectedDate = DateTime.Today;
    }

    private void PopulateRoomBox()
    {
        RoomBox.Items.Add(new ComboBoxItem { Content = "Не назначено", Tag = null });
        foreach (var room in new RoomRepository().GetAll())
            RoomBox.Items.Add(new ComboBoxItem { Content = $"{room.Number} – {room.Name}", Tag = room.Id });

        SelectComboByTag(RoomBox, _equipment.RoomId);
    }

    private void PopulateEmployeeBox()
    {
        EmployeeBox.Items.Add(new ComboBoxItem { Content = "Не назначено", Tag = null });
        foreach (var emp in new EmployeeRepository().GetAll())
            EmployeeBox.Items.Add(new ComboBoxItem { Content = emp.FullName, Tag = emp.Id });

        SelectComboByTag(EmployeeBox, _equipment.EmployeeId);
    }

    private static void SelectComboByTag(ComboBox combo, object? tagValue)
    {
        foreach (ComboBoxItem item in combo.Items)
        {
            if (Equals(item.Tag, tagValue))
            {
                combo.SelectedItem = item;
                return;
            }
        }
        combo.SelectedIndex = 0;
    }

    private void Save_Click(object sender, RoutedEventArgs e)
    {
        int? newRoomId     = GetSelectedId(RoomBox);
        int? newEmployeeId = GetSelectedId(EmployeeBox);

        bool roomChanged     = newRoomId     != _equipment.RoomId;
        bool employeeChanged = newEmployeeId != _equipment.EmployeeId;

        if (!roomChanged && !employeeChanged)
        {
            MessageBox.Show(
                "Назначение не изменилось.\nВыберите новое помещение или нового сотрудника.",
                "Нет изменений", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        if (MovedAtPicker.SelectedDate is null)
        {
            MessageBox.Show("Укажите дату перемещения.",
                "Проверка данных", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        try
        {
            int? oldRoomId     = _equipment.RoomId;
            int? oldEmployeeId = _equipment.EmployeeId;

            _equipment.RoomId     = newRoomId;
            _equipment.EmployeeId = newEmployeeId;

            new EquipmentRepository().Update(_equipment);

            new MovementHistoryRepository().Add(new MovementHistory
            {
                EquipmentId    = _equipment.Id,
                FromRoomId     = oldRoomId,
                ToRoomId       = newRoomId,
                FromEmployeeId = oldEmployeeId,
                ToEmployeeId   = newEmployeeId,
                MovedAt        = MovedAtPicker.SelectedDate.Value,
                Comment        = NullIfEmpty(CommentBox.Text)
            });

            DialogResult = true;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка при сохранении:\n{ex.Message}",
                "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private static int? GetSelectedId(ComboBox combo)
    {
        if (combo.SelectedItem is ComboBoxItem { Tag: int id })
            return id;
        return null;
    }

    private static string? NullIfEmpty(string? value)
        => string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    private void Cancel_Click(object sender, RoutedEventArgs e)
        => DialogResult = false;
}
