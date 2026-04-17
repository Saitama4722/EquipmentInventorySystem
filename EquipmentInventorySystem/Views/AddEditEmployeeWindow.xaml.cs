using System.Windows;
using EquipmentInventorySystem.Data.Repositories;
using EquipmentInventorySystem.Models;

namespace EquipmentInventorySystem.Views;

public partial class AddEditEmployeeWindow : Window
{
    private readonly Employee? _employee;
    private readonly bool _isEditMode;

    public AddEditEmployeeWindow(Employee? employee = null)
    {
        InitializeComponent();
        _employee   = employee;
        _isEditMode = employee is not null;

        FormTitle.Text = _isEditMode ? "Редактирование сотрудника" : "Добавление сотрудника";
        Title = FormTitle.Text;

        if (_isEditMode)
        {
            FullNameBox.Text   = employee!.FullName;
            PositionBox.Text   = employee.Position;
            DepartmentBox.Text = employee.Department;
        }
    }

    private void Save_Click(object sender, RoutedEventArgs e)
    {
        if (!Validate()) return;

        try
        {
            if (_isEditMode)
            {
                _employee!.FullName   = FullNameBox.Text.Trim();
                _employee.Position    = PositionBox.Text.Trim();
                _employee.Department  = DepartmentBox.Text.Trim();
                new EmployeeRepository().Update(_employee);
            }
            else
            {
                var employee = new Employee
                {
                    FullName   = FullNameBox.Text.Trim(),
                    Position   = PositionBox.Text.Trim(),
                    Department = DepartmentBox.Text.Trim()
                };
                new EmployeeRepository().Add(employee);
            }

            DialogResult = true;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка при сохранении:\n{ex.Message}",
                "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private bool Validate()
    {
        if (string.IsNullOrWhiteSpace(FullNameBox.Text))
        {
            MessageBox.Show("Введите ФИО сотрудника.",
                "Проверка данных", MessageBoxButton.OK, MessageBoxImage.Warning);
            FullNameBox.Focus();
            return false;
        }

        if (string.IsNullOrWhiteSpace(PositionBox.Text))
        {
            MessageBox.Show("Введите должность сотрудника.",
                "Проверка данных", MessageBoxButton.OK, MessageBoxImage.Warning);
            PositionBox.Focus();
            return false;
        }

        if (string.IsNullOrWhiteSpace(DepartmentBox.Text))
        {
            MessageBox.Show("Введите название отдела или подразделения.",
                "Проверка данных", MessageBoxButton.OK, MessageBoxImage.Warning);
            DepartmentBox.Focus();
            return false;
        }

        return true;
    }

    private void Cancel_Click(object sender, RoutedEventArgs e)
        => DialogResult = false;
}
