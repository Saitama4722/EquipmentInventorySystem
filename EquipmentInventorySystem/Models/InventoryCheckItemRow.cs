using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace EquipmentInventorySystem.Models;

public class ResultStatusOption
{
    public string Display { get; init; } = string.Empty;
    public InventoryResultStatus Value { get; init; }
}

public class InventoryCheckItemRow : INotifyPropertyChanged
{
    public static IReadOnlyList<ResultStatusOption> StatusOptions { get; } =
    [
        new() { Display = "Присутствует", Value = InventoryResultStatus.Present },
        new() { Display = "Отсутствует",  Value = InventoryResultStatus.Missing },
        new() { Display = "Повреждено",   Value = InventoryResultStatus.Damaged },
        new() { Display = "Излишек",      Value = InventoryResultStatus.Surplus }
    ];

    public int EquipmentId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string InventoryNumber { get; set; } = string.Empty;
    public string SerialNumber { get; set; } = string.Empty;
    public string StatusDisplay { get; set; } = string.Empty;
    public string RoomDisplay { get; set; } = string.Empty;
    public string EmployeeDisplay { get; set; } = string.Empty;

    private ResultStatusOption _selectedOption = StatusOptions[0];
    public ResultStatusOption SelectedOption
    {
        get => _selectedOption;
        set { _selectedOption = value ?? StatusOptions[0]; OnPropertyChanged(); }
    }

    public InventoryResultStatus ResultStatus => SelectedOption.Value;

    private string? _comment;
    public string? Comment
    {
        get => _comment;
        set { _comment = value; OnPropertyChanged(); }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
