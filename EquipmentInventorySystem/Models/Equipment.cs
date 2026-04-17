namespace EquipmentInventorySystem.Models;

public class Equipment
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string InventoryNumber { get; set; } = string.Empty;
    public string? SerialNumber { get; set; }
    public EquipmentStatus Status { get; set; } = EquipmentStatus.Active;
    public int? RoomId { get; set; }
    public int? EmployeeId { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}
