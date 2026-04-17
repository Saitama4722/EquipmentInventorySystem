using System.Windows;
using System.Windows.Controls;
using EquipmentInventorySystem.Data.Repositories;
using EquipmentInventorySystem.Models;
using Microsoft.Data.Sqlite;

namespace EquipmentInventorySystem.Views;

public partial class AddEditEquipmentWindow : Window
{
    private readonly Equipment? _equipment;
    private readonly bool _isEditMode;

    public AddEditEquipmentWindow(Equipment? equipment = null)
    {
        InitializeComponent();
        _equipment = equipment;
        _isEditMode = equipment is not null;

        FormTitle.Text = _isEditMode ? "Редактирование оборудования" : "Добавление оборудования";
        Title = FormTitle.Text;

        PopulateStaticComboBoxes();
        PopulateDynamicComboBoxes();

        if (_isEditMode)
            PopulateFormFromModel(equipment!);
    }

    private void PopulateStaticComboBoxes()
    {
        foreach (var (display, value) in EquipmentStatusHelper.AllStatuses)
            StatusBox.Items.Add(new ComboBoxItem { Content = display, Tag = value });
        StatusBox.SelectedIndex = 0;
    }

    private void PopulateDynamicComboBoxes()
    {
        RoomBox.Items.Add(new ComboBoxItem { Content = "Не назначено", Tag = null });
        foreach (var room in new RoomRepository().GetAll())
            RoomBox.Items.Add(new ComboBoxItem { Content = $"{room.Number} – {room.Name}", Tag = room.Id });
        RoomBox.SelectedIndex = 0;

        EmployeeBox.Items.Add(new ComboBoxItem { Content = "Не назначено", Tag = null });
        foreach (var emp in new EmployeeRepository().GetAll())
            EmployeeBox.Items.Add(new ComboBoxItem { Content = emp.FullName, Tag = emp.Id });
        EmployeeBox.SelectedIndex = 0;
    }

    private void PopulateFormFromModel(Equipment e)
    {
        NameBox.Text            = e.Name;
        InventoryNumberBox.Text = e.InventoryNumber;
        SerialNumberBox.Text    = e.SerialNumber ?? string.Empty;
        NotesBox.Text           = e.Notes ?? string.Empty;

        SelectComboByTag(StatusBox, e.Status);
        SelectComboByTag(RoomBox, e.RoomId);
        SelectComboByTag(EmployeeBox, e.EmployeeId);
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
        if (!Validate()) return;

        try
        {
            if (_isEditMode)
                SaveEdit();
            else
                SaveNew();

            DialogResult = true;
        }
        catch (SqliteException ex) when (ex.Message.Contains("UNIQUE"))
        {
            MessageBox.Show("Оборудование с таким инвентарным или серийным номером уже существует.",
                "Дублирование", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка при сохранении:\n{ex.Message}",
                "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private bool Validate()
    {
        if (string.IsNullOrWhiteSpace(NameBox.Text))
        {
            MessageBox.Show("Введите наименование оборудования.",
                "Проверка данных", MessageBoxButton.OK, MessageBoxImage.Warning);
            NameBox.Focus();
            return false;
        }

        if (string.IsNullOrWhiteSpace(InventoryNumberBox.Text))
        {
            MessageBox.Show("Введите инвентарный номер.",
                "Проверка данных", MessageBoxButton.OK, MessageBoxImage.Warning);
            InventoryNumberBox.Focus();
            return false;
        }

        return true;
    }

    private void SaveNew()
    {
        var equipment = new Equipment
        {
            Name            = NameBox.Text.Trim(),
            InventoryNumber = InventoryNumberBox.Text.Trim(),
            SerialNumber    = NullIfEmpty(SerialNumberBox.Text),
            Status          = GetSelectedStatus(),
            RoomId          = GetSelectedId(RoomBox),
            EmployeeId      = GetSelectedId(EmployeeBox),
            Notes           = NullIfEmpty(NotesBox.Text),
            CreatedAt       = DateTime.Now,
            UpdatedAt       = DateTime.Now
        };
        new EquipmentRepository().Add(equipment);
    }

    private void SaveEdit()
    {
        int? oldRoomId     = _equipment!.RoomId;
        int? oldEmployeeId = _equipment.EmployeeId;

        _equipment.Name            = NameBox.Text.Trim();
        _equipment.InventoryNumber = InventoryNumberBox.Text.Trim();
        _equipment.SerialNumber    = NullIfEmpty(SerialNumberBox.Text);
        _equipment.Status          = GetSelectedStatus();
        _equipment.RoomId          = GetSelectedId(RoomBox);
        _equipment.EmployeeId      = GetSelectedId(EmployeeBox);
        _equipment.Notes           = NullIfEmpty(NotesBox.Text);

        new EquipmentRepository().Update(_equipment);

        bool roomChanged     = oldRoomId     != _equipment.RoomId;
        bool employeeChanged = oldEmployeeId != _equipment.EmployeeId;

        if (roomChanged || employeeChanged)
        {
            new MovementHistoryRepository().Add(new MovementHistory
            {
                EquipmentId    = _equipment.Id,
                FromRoomId     = oldRoomId,
                ToRoomId       = _equipment.RoomId,
                FromEmployeeId = oldEmployeeId,
                ToEmployeeId   = _equipment.EmployeeId,
                MovedAt        = DateTime.Now,
                Comment        = "Изменено при редактировании"
            });
        }
    }

    private EquipmentStatus GetSelectedStatus()
    {
        if (StatusBox.SelectedItem is ComboBoxItem { Tag: EquipmentStatus s })
            return s;
        return EquipmentStatus.Active;
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
