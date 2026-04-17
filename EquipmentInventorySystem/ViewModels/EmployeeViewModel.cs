using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using EquipmentInventorySystem.Data.Repositories;
using EquipmentInventorySystem.Models;

namespace EquipmentInventorySystem.ViewModels;

public class EmployeeViewModel : INotifyPropertyChanged
{
    private readonly EmployeeRepository _repo = new();

    public ObservableCollection<Employee> Items { get; } = [];

    private Employee? _selectedItem;
    public Employee? SelectedItem
    {
        get => _selectedItem;
        set { _selectedItem = value; OnPropertyChanged(); }
    }

    public void LoadItems(string? search = null)
    {
        var all = _repo.GetAll();

        var filtered = string.IsNullOrWhiteSpace(search)
            ? all
            : all.Where(e =>
                e.FullName.Contains(search, StringComparison.OrdinalIgnoreCase)   ||
                e.Position.Contains(search, StringComparison.OrdinalIgnoreCase)   ||
                e.Department.Contains(search, StringComparison.OrdinalIgnoreCase)).ToList();

        Items.Clear();
        foreach (var item in filtered)
            Items.Add(item);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
