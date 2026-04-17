using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using EquipmentInventorySystem.Models;
using EquipmentInventorySystem.ViewModels;
using Microsoft.Win32;

namespace EquipmentInventorySystem.Views;

public partial class ReportsPage : UserControl
{
    private readonly ReportsViewModel _vm = new();
    private string _currentTag = "AllEquipment";

    public ReportsPage()
    {
        InitializeComponent();
        DataContext = _vm;
        LoadReport("AllEquipment");
    }

    private void ReportButton_Checked(object sender, RoutedEventArgs e)
    {
        if (sender is not RadioButton btn || btn.Tag is not string tag) return;
        LoadReport(tag);
    }

    private void LoadReport(string tag)
    {
        _currentTag = tag;

        PanelAllEquipment.Visibility = Visibility.Collapsed;
        PanelRooms.Visibility        = Visibility.Collapsed;
        PanelEmployees.Visibility    = Visibility.Collapsed;
        PanelInventory.Visibility    = Visibility.Collapsed;

        switch (tag)
        {
            case "AllEquipment":
                _vm.LoadAllEquipment();
                PanelAllEquipment.Visibility = Visibility.Visible;
                UpdateSubtitle(TxtAllEquipmentCount, _vm.AllEquipmentRows.Count, "позиций", TxtNoEquipment, GridAllEquipment);
                break;

            case "Rooms":
                _vm.LoadRooms();
                PanelRooms.Visibility = Visibility.Visible;
                UpdateSubtitle(TxtRoomsCount, _vm.RoomRows.Count, "помещений", TxtNoRooms, GridRooms);
                break;

            case "Employees":
                _vm.LoadEmployees();
                PanelEmployees.Visibility = Visibility.Visible;
                UpdateSubtitle(TxtEmployeesCount, _vm.EmployeeRows.Count, "сотрудников", TxtNoEmployees, GridEmployees);
                break;

            case "InventoryChecks":
                _vm.LoadInventoryChecks();
                PanelInventory.Visibility = Visibility.Visible;
                ComboChecks.SelectedIndex = -1;
                ResetCheckItemsPanel();
                UpdateInventorySubtitle();
                break;
        }
    }

    private static void UpdateSubtitle(TextBlock subtitle, int count, string unit,
                                       TextBlock noDataLabel, DataGrid grid)
    {
        subtitle.Text = count > 0 ? $"Всего записей: {count} {unit}" : string.Empty;
        noDataLabel.Visibility = count == 0 ? Visibility.Visible  : Visibility.Collapsed;
        grid.Visibility        = count >  0 ? Visibility.Visible  : Visibility.Collapsed;
    }

    private void UpdateInventorySubtitle()
    {
        int count = _vm.InventoryChecks.Count;
        TxtInventoryCount.Text = count > 0
            ? $"Всего инвентаризаций: {count}"
            : "Нет проведённых инвентаризаций";
    }

    private void ResetCheckItemsPanel()
    {
        TxtNoCheckSelected.Visibility = Visibility.Visible;
        TxtCheckNoItems.Visibility    = Visibility.Collapsed;
        GridCheckItems.Visibility     = Visibility.Collapsed;
        TxtCheckInfo.Text             = string.Empty;
    }

    private void ComboChecks_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (ComboChecks.SelectedItem is not InventoryCheckDisplayRow check)
        {
            _vm.SelectedCheck = null;
            ResetCheckItemsPanel();
            return;
        }

        _vm.SelectedCheck = check;

        TxtCheckInfo.Text = $"Ответственный: {check.ResponsibleEmployee}  •  Позиций: {check.ItemCount}";

        bool hasItems = _vm.CheckItems.Count > 0;
        TxtNoCheckSelected.Visibility = Visibility.Collapsed;
        TxtCheckNoItems.Visibility    = hasItems ? Visibility.Collapsed : Visibility.Visible;
        GridCheckItems.Visibility     = hasItems ? Visibility.Visible   : Visibility.Collapsed;
    }

    private void BtnExport_Click(object sender, RoutedEventArgs e)
    {
        var dlg = new SaveFileDialog
        {
            Title      = "Экспорт отчёта",
            Filter     = "CSV файл (*.csv)|*.csv",
            FileName   = GetExportFileName()
        };

        if (dlg.ShowDialog() != true) return;

        try
        {
            var lines = BuildCsvLines();
            File.WriteAllLines(dlg.FileName, lines, new UTF8Encoding(true));
            MessageBox.Show("Экспорт выполнен успешно.", "Экспорт", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка при экспорте:\n{ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private string GetExportFileName() => _currentTag switch
    {
        "AllEquipment"   => "report_equipment",
        "Rooms"          => "report_rooms",
        "Employees"      => "report_employees",
        "InventoryChecks" => "report_inventory",
        _                => "report"
    };

    private List<string> BuildCsvLines()
    {
        var lines = new List<string>();

        switch (_currentTag)
        {
            case "AllEquipment":
                lines.Add(CsvRow("Наименование", "Инв. номер", "Серийный №", "Статус", "Помещение", "Сотрудник"));
                foreach (var r in _vm.AllEquipmentRows)
                    lines.Add(CsvRow(r.Name, r.InventoryNumber, r.SerialNumber, r.StatusDisplay, r.RoomDisplay, r.EmployeeDisplay));
                break;

            case "Rooms":
                lines.Add(CsvRow("Номер", "Наименование", "Описание", "Кол-во оборудования"));
                foreach (var r in _vm.RoomRows)
                    lines.Add(CsvRow(r.Number, r.Name, r.Description, r.EquipmentCount.ToString()));
                break;

            case "Employees":
                lines.Add(CsvRow("ФИО", "Должность", "Отдел", "Кол-во оборудования"));
                foreach (var r in _vm.EmployeeRows)
                    lines.Add(CsvRow(r.FullName, r.Position, r.Department, r.EquipmentCount.ToString()));
                break;

            case "InventoryChecks":
                if (_vm.SelectedCheck == null)
                {
                    lines.Add(CsvRow("Дата", "Ответственный", "Наименование", "Инв. номер", "Результат", "Помещение", "Сотрудник", "Примечание"));
                }
                else
                {
                    lines.Add($"Инвентаризация: {_vm.SelectedCheck.CheckDateDisplay}  |  Ответственный: {_vm.SelectedCheck.ResponsibleEmployee}");
                    lines.Add(string.Empty);
                    lines.Add(CsvRow("Наименование", "Инв. номер", "Результат", "Помещение", "Сотрудник", "Примечание"));
                    foreach (var r in _vm.CheckItems)
                        lines.Add(CsvRow(r.EquipmentName, r.InventoryNumber, r.ResultStatusDisplay, r.RoomDisplay, r.EmployeeDisplay, r.Comment));
                }
                break;
        }

        return lines;
    }

    private static string CsvRow(params string[] fields)
    {
        var escaped = fields.Select(f => $"\"{(f ?? string.Empty).Replace("\"", "\"\"")}\"");
        return string.Join(";", escaped);
    }
}
