namespace EquipmentInventorySystem.Models;

public static class EquipmentStatusHelper
{
    public static string GetDisplay(EquipmentStatus status) => status switch
    {
        EquipmentStatus.Active      => "В использовании",
        EquipmentStatus.UnderRepair => "На ремонте",
        EquipmentStatus.WrittenOff  => "Списано",
        EquipmentStatus.Reserved    => "Зарезервировано",
        _                           => status.ToString()
    };

    public static IReadOnlyList<(string Display, EquipmentStatus Value)> AllStatuses { get; } =
    [
        ("В использовании", EquipmentStatus.Active),
        ("На ремонте",      EquipmentStatus.UnderRepair),
        ("Списано",         EquipmentStatus.WrittenOff),
        ("Зарезервировано", EquipmentStatus.Reserved)
    ];
}
