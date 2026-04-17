namespace EquipmentInventorySystem.Models;

public class MovementHistoryDisplayRow
{
    public int      Id              { get; init; }
    public DateTime MovedAt         { get; init; }
    public string   EquipmentName   { get; init; } = string.Empty;
    public string   InventoryNumber { get; init; } = string.Empty;
    public string?  FromRoomDisplay     { get; init; }
    public string?  ToRoomDisplay       { get; init; }
    public string?  FromEmployeeDisplay { get; init; }
    public string?  ToEmployeeDisplay   { get; init; }
    public string?  Comment         { get; init; }

    public string MovedAtDisplay => MovedAt.ToString("dd.MM.yyyy HH:mm");

    public string FromRoomText     => FromRoomDisplay     ?? "—";
    public string ToRoomText       => ToRoomDisplay       ?? "—";
    public string FromEmployeeText => FromEmployeeDisplay ?? "—";
    public string ToEmployeeText   => ToEmployeeDisplay   ?? "—";
    public string CommentText      => string.IsNullOrWhiteSpace(Comment) ? "—" : Comment;

    public string ChangeDescription
    {
        get
        {
            bool roomChanged     = FromRoomDisplay     != ToRoomDisplay;
            bool employeeChanged = FromEmployeeDisplay != ToEmployeeDisplay;
            return (roomChanged, employeeChanged) switch
            {
                (true,  true)  => "Помещение и сотрудник",
                (true,  false) => "Помещение",
                (false, true)  => "Сотрудник",
                _              => "—"
            };
        }
    }
}
