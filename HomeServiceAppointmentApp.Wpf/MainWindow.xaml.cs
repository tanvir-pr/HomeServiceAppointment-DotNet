using HomeServiceAppointmentApp.Wpf.Data;
using HomeServiceAppointmentApp.Wpf.Models;
using System.Windows;
using System.Windows.Controls;

namespace HomeServiceAppointmentApp.Wpf;

public partial class MainWindow : Window
{
    private readonly HomeServiceRepository _repository = new();
    private Customer? _selectedCustomer;

    public MainWindow()
    {
        InitializeComponent();
        _repository.EnsureDatabase();
        RefreshData();
    }

    private void RefreshData()
    {
        CustomerDataGrid.ItemsSource = _repository.GetCustomers();
    }

    private void RefreshData_Click(object sender, RoutedEventArgs e) => RefreshData();

    private void CustomerDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        _selectedCustomer = CustomerDataGrid.SelectedItem as Customer;
        if (_selectedCustomer is null)
        {
            return;
        }

        CustomerNameTextBox.Text = _selectedCustomer.Name;
        CustomerEmailTextBox.Text = _selectedCustomer.Email;
        CustomerPhoneTextBox.Text = _selectedCustomer.Phone ?? string.Empty;
        CustomerAddressTextBox.Text = _selectedCustomer.DefaultAddress ?? string.Empty;
    }
}