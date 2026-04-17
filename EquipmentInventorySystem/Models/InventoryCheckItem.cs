namespace EquipmentInventorySystem.Models;

public class InventoryCheckItem
{
    public int Id { get; set; }
    public int InventoryCheckId { get; set; }
    public int EquipmentId { get; set; }
    public InventoryResultStatus ResultStatus { get; set; }
    public string? Comment { get; set; }
}
