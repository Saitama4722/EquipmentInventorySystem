namespace EquipmentInventorySystem.Models;

public class InventoryCheckDisplayRow
{
    public int Id { get; set; }
    public DateTime CheckDate { get; set; }
    public string CheckDateDisplay => CheckDate.ToString("dd.MM.yyyy");
    public string ResponsibleEmployee { get; set; } = string.Empty;
    public int ItemCount { get; set; }
    public string? Comment { get; set; }
    public DateTime CreatedAt { get; set; }
    public string CreatedAtDisplay => CreatedAt.ToString("dd.MM.yyyy HH:mm");
}
