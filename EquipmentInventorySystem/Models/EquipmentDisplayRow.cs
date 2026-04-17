namespace EquipmentInventorySystem.Models;

public class EquipmentDisplayRow
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string InventoryNumber { get; set; } = string.Empty;
    public string SerialNumber { get; set; } = string.Empty;
    public EquipmentStatus Status { get; set; }
    public string StatusDisplay { get; set; } = string.Empty;
    public string RoomDisplay { get; set; } = string.Empty;
    public string EmployeeDisplay { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
}
