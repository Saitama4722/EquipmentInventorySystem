namespace EquipmentInventorySystem.Models;

public static class InventoryResultStatusHelper
{
    public static string GetDisplay(InventoryResultStatus status) => status switch
    {
        InventoryResultStatus.Present => "Присутствует",
        InventoryResultStatus.Missing => "Отсутствует",
        InventoryResultStatus.Damaged => "Повреждено",
        InventoryResultStatus.Surplus => "Излишек",
        _                             => status.ToString()
    };

    public static IReadOnlyList<(string Display, InventoryResultStatus Value)> AllStatuses { get; } =
    [
        ("Присутствует", InventoryResultStatus.Present),
        ("Отсутствует",  InventoryResultStatus.Missing),
        ("Повреждено",   InventoryResultStatus.Damaged),
        ("Излишек",      InventoryResultStatus.Surplus)
    ];
}
