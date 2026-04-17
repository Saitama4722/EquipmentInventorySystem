using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using EquipmentInventorySystem.Data.Repositories;
using EquipmentInventorySystem.Models;

namespace EquipmentInventorySystem.ViewModels;

public class RoomViewModel : INotifyPropertyChanged
{
    private readonly RoomRepository _repo = new();

    public ObservableCollection<Room> Items { get; } = [];

    private Room? _selectedItem;
    public Room? SelectedItem
    {
        get => _selectedItem;
        set { _selectedItem = value; OnPropertyChanged(); }
    }

    public void LoadItems(string? search = null)
    {
        var all = _repo.GetAll();

        var filtered = string.IsNullOrWhiteSpace(search)
            ? all
            : all.Where(r =>
                r.Number.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                r.Name.Contains(search, StringComparison.OrdinalIgnoreCase)   ||
                (r.Description ?? string.Empty).Contains(search, StringComparison.OrdinalIgnoreCase)).ToList();

        Items.Clear();
        foreach (var item in filtered)
            Items.Add(item);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
