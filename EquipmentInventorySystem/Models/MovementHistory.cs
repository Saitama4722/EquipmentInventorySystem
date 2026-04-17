namespace EquipmentInventorySystem.Models;

public class MovementHistory
{
    public int Id { get; set; }
    public int EquipmentId { get; set; }
    public int? FromRoomId { get; set; }
    public int? ToRoomId { get; set; }
    public int? FromEmployeeId { get; set; }
    public int? ToEmployeeId { get; set; }
    public DateTime MovedAt { get; set; } = DateTime.Now;
    public string? Comment { get; set; }
}
