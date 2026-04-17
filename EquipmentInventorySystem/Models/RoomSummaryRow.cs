namespace EquipmentInventorySystem.Models;

public class RoomSummaryRow
{
    public string Number { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int EquipmentCount { get; set; }
}
