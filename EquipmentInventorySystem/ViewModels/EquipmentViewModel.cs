using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using EquipmentInventorySystem.Data.Repositories;
using EquipmentInventorySystem.Models;

namespace EquipmentInventorySystem.ViewModels;

public class EquipmentViewModel : INotifyPropertyChanged
{
    private readonly EquipmentRepository _repo = new();

    public ObservableCollection<EquipmentDisplayRow> Items { get; } = [];

    private EquipmentDisplayRow? _selectedItem;
    public EquipmentDisplayRow? SelectedItem
    {
        get => _selectedItem;
        set { _selectedItem = value; OnPropertyChanged(); }
    }

    public void LoadItems(string? search = null, EquipmentStatus? status = null)
    {
        var rows = _repo.GetAllForDisplay(search, status);
        Items.Clear();
        foreach (var row in rows)
            Items.Add(row);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
