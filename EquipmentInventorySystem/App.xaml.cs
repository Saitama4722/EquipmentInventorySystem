using System.Windows;
using EquipmentInventorySystem.Data;

namespace EquipmentInventorySystem;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        DatabaseInitializer.Initialize();
    }
}
