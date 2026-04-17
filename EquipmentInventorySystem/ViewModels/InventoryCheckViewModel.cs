using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using EquipmentInventorySystem.Data.Repositories;
using EquipmentInventorySystem.Models;

namespace EquipmentInventorySystem.ViewModels;

public class InventoryCheckViewModel : INotifyPropertyChanged
{
    private readonly InventoryCheckRepository _repo = new();

    public ObservableCollection<InventoryCheckDisplayRow> Items { get; } = [];

    private InventoryCheckDisplayRow? _selectedItem;
    public InventoryCheckDisplayRow? SelectedItem
    {
        get => _selectedItem;
        set { _selectedItem = value; OnPropertyChanged(); }
    }

    public void LoadItems()
    {
        var rows = _repo.GetAllWithDetails();
        Items.Clear();
        foreach (var row in rows)
            Items.Add(row);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
