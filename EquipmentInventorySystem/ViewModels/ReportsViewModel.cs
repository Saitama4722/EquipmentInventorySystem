using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using EquipmentInventorySystem.Data.Repositories;
using EquipmentInventorySystem.Models;

namespace EquipmentInventorySystem.ViewModels;

public class ReportsViewModel : INotifyPropertyChanged
{
    private readonly EquipmentRepository      _equipmentRepo = new();
    private readonly RoomRepository           _roomRepo      = new();
    private readonly EmployeeRepository       _employeeRepo  = new();
    private readonly InventoryCheckRepository _checkRepo     = new();

    public ObservableCollection<EquipmentDisplayRow>       AllEquipmentRows { get; } = [];
    public ObservableCollection<RoomSummaryRow>            RoomRows         { get; } = [];
    public ObservableCollection<EmployeeSummaryRow>        EmployeeRows     { get; } = [];
    public ObservableCollection<InventoryCheckDisplayRow>  InventoryChecks  { get; } = [];
    public ObservableCollection<InventoryCheckItemDetailRow> CheckItems     { get; } = [];

    private InventoryCheckDisplayRow? _selectedCheck;
    public InventoryCheckDisplayRow? SelectedCheck
    {
        get => _selectedCheck;
        set
        {
            _selectedCheck = value;
            OnPropertyChanged();
            LoadCheckItems();
        }
    }

    public void LoadAllEquipment()
    {
        AllEquipmentRows.Clear();
        foreach (var row in _equipmentRepo.GetAllForDisplay(null, null))
            AllEquipmentRows.Add(row);
    }

    public void LoadRooms()
    {
        RoomRows.Clear();
        foreach (var row in _roomRepo.GetSummaryWithEquipmentCount())
            RoomRows.Add(row);
    }

    public void LoadEmployees()
    {
        EmployeeRows.Clear();
        foreach (var row in _employeeRepo.GetSummaryWithEquipmentCount())
            EmployeeRows.Add(row);
    }

    public void LoadInventoryChecks()
    {
        InventoryChecks.Clear();
        CheckItems.Clear();
        _selectedCheck = null;
        OnPropertyChanged(nameof(SelectedCheck));

        foreach (var row in _checkRepo.GetAllWithDetails())
            InventoryChecks.Add(row);
    }

    public void LoadCheckItems()
    {
        CheckItems.Clear();
        if (_selectedCheck == null) return;

        foreach (var item in _checkRepo.GetItemsWithDetails(_selectedCheck.Id))
            CheckItems.Add(item);
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
