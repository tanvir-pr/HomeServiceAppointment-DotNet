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

    private void CreateCustomer_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var customer = BuildCustomer();
            _repository.AddCustomer(customer);
            ClearCustomerForm();
            RefreshData();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Create Customer Failed", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void UpdateCustomer_Click(object sender, RoutedEventArgs e)
    {
        if (_selectedCustomer is null)
        {
            MessageBox.Show("Select a customer to update.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        try
        {
            var customer = BuildCustomer();
            customer.CustomerId = _selectedCustomer.CustomerId;
            _repository.UpdateCustomer(customer);
            RefreshData();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Update Customer Failed", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void DeleteCustomer_Click(object sender, RoutedEventArgs e)
    {
        if (_selectedCustomer is null)
        {
            MessageBox.Show("Select a customer to delete.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        try
        {
            _repository.DeleteCustomer(_selectedCustomer.CustomerId);
            ClearCustomerForm();
            RefreshData();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Delete Customer Failed", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private Customer BuildCustomer()
    {
        if (string.IsNullOrWhiteSpace(CustomerNameTextBox.Text) || string.IsNullOrWhiteSpace(CustomerEmailTextBox.Text))
        {
            throw new InvalidOperationException("Name and Email are required.");
        }

        return new Customer
        {
            Name = CustomerNameTextBox.Text.Trim(),
            Email = CustomerEmailTextBox.Text.Trim(),
            Phone = ToNullable(CustomerPhoneTextBox.Text),
            DefaultAddress = ToNullable(CustomerAddressTextBox.Text)
        };
    }

    private void ClearCustomerForm()
    {
        _selectedCustomer = null;
        CustomerDataGrid.SelectedItem = null;
        CustomerNameTextBox.Text = string.Empty;
        CustomerEmailTextBox.Text = string.Empty;
        CustomerPhoneTextBox.Text = string.Empty;
        CustomerAddressTextBox.Text = string.Empty;
    }

    private static string? ToNullable(string value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}