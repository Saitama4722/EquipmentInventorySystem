namespace EquipmentInventorySystem.Models;

public class EmployeeSummaryRow
{
    public string FullName { get; set; } = string.Empty;
    public string Position { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public int EquipmentCount { get; set; }
}
