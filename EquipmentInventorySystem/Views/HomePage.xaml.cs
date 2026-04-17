using System.Windows.Controls;
using EquipmentInventorySystem.Data.Repositories;

namespace EquipmentInventorySystem.Views;

public partial class HomePage : UserControl
{
    public HomePage()
    {
        InitializeComponent();
        Loaded += (_, _) => LoadStats();
    }

    private void LoadStats()
    {
        try
        {
            CountEquipment.Text = new EquipmentRepository().GetAll().Count.ToString();
            CountEmployees.Text = new EmployeeRepository().GetAll().Count.ToString();
            CountRooms.Text     = new RoomRepository().GetAll().Count.ToString();
            CountChecks.Text    = new InventoryCheckRepository().GetAll().Count.ToString();
        }
        catch
        {
            // Stats are decorative; silently keep "—" on failure.
        }
    }
}
