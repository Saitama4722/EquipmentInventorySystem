namespace EquipmentInventorySystem.Models;

public class InventoryCheck
{
    public int Id { get; set; }
    public DateTime CheckDate { get; set; }
    public int ResponsibleEmployeeId { get; set; }
    public string? Comment { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}
