using System.Windows;
using EquipmentInventorySystem.Data.Repositories;
using EquipmentInventorySystem.Models;

namespace EquipmentInventorySystem.Views;

public partial class AddEditRoomWindow : Window
{
    private readonly Room? _room;
    private readonly bool _isEditMode;

    public AddEditRoomWindow(Room? room = null)
    {
        InitializeComponent();
        _room       = room;
        _isEditMode = room is not null;

        FormTitle.Text = _isEditMode ? "Редактирование помещения" : "Добавление помещения";
        Title = FormTitle.Text;

        if (_isEditMode)
        {
            NumberBox.Text      = room!.Number;
            NameBox.Text        = room.Name;
            DescriptionBox.Text = room.Description ?? string.Empty;
        }
    }

    private void Save_Click(object sender, RoutedEventArgs e)
    {
        if (!Validate()) return;

        try
        {
            if (_isEditMode)
            {
                _room!.Number      = NumberBox.Text.Trim();
                _room.Name         = NameBox.Text.Trim();
                _room.Description  = NullIfEmpty(DescriptionBox.Text);
                new RoomRepository().Update(_room);
            }
            else
            {
                var room = new Room
                {
                    Number      = NumberBox.Text.Trim(),
                    Name        = NameBox.Text.Trim(),
                    Description = NullIfEmpty(DescriptionBox.Text)
                };
                new RoomRepository().Add(room);
            }

            DialogResult = true;
        }
        catch (Exception ex) when (ex.Message.Contains("UNIQUE"))
        {
            MessageBox.Show("Помещение с таким номером уже существует.",
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
        if (string.IsNullOrWhiteSpace(NumberBox.Text))
        {
            MessageBox.Show("Введите номер помещения.",
                "Проверка данных", MessageBoxButton.OK, MessageBoxImage.Warning);
            NumberBox.Focus();
            return false;
        }

        if (string.IsNullOrWhiteSpace(NameBox.Text))
        {
            MessageBox.Show("Введите наименование помещения.",
                "Проверка данных", MessageBoxButton.OK, MessageBoxImage.Warning);
            NameBox.Focus();
            return false;
        }

        return true;
    }

    private static string? NullIfEmpty(string? value)
        => string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    private void Cancel_Click(object sender, RoutedEventArgs e)
        => DialogResult = false;
}
