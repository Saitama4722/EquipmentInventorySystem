using System.Windows;
using EquipmentInventorySystem.Data.Repositories;
using EquipmentInventorySystem.Models;

namespace EquipmentInventorySystem.Views;

public partial class InventoryCheckDetailsWindow : Window
{
    public InventoryCheckDetailsWindow(InventoryCheckDisplayRow check)
    {
        InitializeComponent();

        WindowTitleText.Text = $"Инвентаризация от {check.CheckDateDisplay}";
        Title                = WindowTitleText.Text;

        DateText.Text      = check.CheckDateDisplay;
        EmployeeText.Text  = check.ResponsibleEmployee;
        CommentText.Text   = string.IsNullOrWhiteSpace(check.Comment) ? "—" : check.Comment;

        var items = new InventoryCheckRepository().GetItemsWithDetails(check.Id);
        ItemCountText.Text = items.Count.ToString();
        ItemsGrid.ItemsSource = items;
    }

    private void Close_Click(object sender, RoutedEventArgs e) => Close();
}
