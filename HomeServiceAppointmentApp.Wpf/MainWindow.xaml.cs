using HomeServiceAppointmentApp.Wpf.Data;
using HomeServiceAppointmentApp.Wpf.Models;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;

namespace HomeServiceAppointmentApp.Wpf;

public partial class MainWindow : Window
{
    private readonly HomeServiceRepository _repository = new();
    private Customer? _selectedCustomer;
    private HomeServiceAppointment? _selectedAppointment;

    public MainWindow()
    {
        InitializeComponent();
        _repository.EnsureDatabase();
        RefreshData();
    }

    private void RefreshData()
    {
        CustomerDataGrid.ItemsSource = _repository.GetCustomers();
        AppointmentDataGrid.ItemsSource = _repository.GetAppointments();
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

    private void AppointmentDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        _selectedAppointment = AppointmentDataGrid.SelectedItem as HomeServiceAppointment;
        if (_selectedAppointment is null)
        {
            return;
        }

        AppointmentCustomerIdTextBox.Text = _selectedAppointment.CustomerId.ToString();
        AppointmentAddressTextBox.Text = _selectedAppointment.AddressLine1;
        AppointmentCityTextBox.Text = _selectedAppointment.City ?? string.Empty;
        AppointmentServiceTypeTextBox.Text = _selectedAppointment.ServiceType;
        AppointmentTechnicianTextBox.Text = _selectedAppointment.TechnicianName;
        AppointmentDateTextBox.Text = _selectedAppointment.ScheduledDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
        AppointmentStartTimeTextBox.Text = _selectedAppointment.StartTime;
        AppointmentStatusTextBox.Text = _selectedAppointment.Status;
        AppointmentFeeTextBox.Text = _selectedAppointment.FeeAmount.ToString(CultureInfo.InvariantCulture);
        AppointmentPaidTextBox.Text = _selectedAppointment.PaidAmount.ToString(CultureInfo.InvariantCulture);
        AppointmentPaymentStatusTextBox.Text = _selectedAppointment.PaymentStatus;
    }

    private void CreateAppointment_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var appointment = BuildAppointment();
            _repository.AddAppointment(appointment);
            ClearAppointmentForm();
            RefreshData();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Create Appointment Failed", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void UpdateAppointment_Click(object sender, RoutedEventArgs e)
    {
        if (_selectedAppointment is null)
        {
            MessageBox.Show("Select an appointment to update.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        try
        {
            var appointment = BuildAppointment();
            appointment.AppointmentId = _selectedAppointment.AppointmentId;
            _repository.UpdateAppointment(appointment);
            RefreshData();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Update Appointment Failed", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void DeleteAppointment_Click(object sender, RoutedEventArgs e)
    {
        if (_selectedAppointment is null)
        {
            MessageBox.Show("Select an appointment to delete.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        try
        {
            _repository.DeleteAppointment(_selectedAppointment.AppointmentId);
            ClearAppointmentForm();
            RefreshData();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Delete Appointment Failed", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private HomeServiceAppointment BuildAppointment()
    {
        if (!int.TryParse(AppointmentCustomerIdTextBox.Text.Trim(), out var customerId))
        {
            throw new InvalidOperationException("Customer ID must be a valid number.");
        }

        if (string.IsNullOrWhiteSpace(AppointmentAddressTextBox.Text) ||
            string.IsNullOrWhiteSpace(AppointmentServiceTypeTextBox.Text) ||
            string.IsNullOrWhiteSpace(AppointmentTechnicianTextBox.Text) ||
            string.IsNullOrWhiteSpace(AppointmentDateTextBox.Text) ||
            string.IsNullOrWhiteSpace(AppointmentStartTimeTextBox.Text))
        {
            throw new InvalidOperationException("Address, Service Type, Technician, Date, and Start Time are required.");
        }

        if (!DateTime.TryParseExact(AppointmentDateTextBox.Text.Trim(), "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var scheduledDate))
        {
            throw new InvalidOperationException("Date must be in yyyy-MM-dd format.");
        }

        if (!decimal.TryParse(AppointmentFeeTextBox.Text.Trim(), NumberStyles.Number, CultureInfo.InvariantCulture, out var feeAmount))
        {
            throw new InvalidOperationException("Fee Amount must be a number.");
        }

        if (!decimal.TryParse(AppointmentPaidTextBox.Text.Trim(), NumberStyles.Number, CultureInfo.InvariantCulture, out var paidAmount))
        {
            throw new InvalidOperationException("Paid Amount must be a number.");
        }

        return new HomeServiceAppointment
        {
            CustomerId = customerId,
            AddressLine1 = AppointmentAddressTextBox.Text.Trim(),
            City = ToNullable(AppointmentCityTextBox.Text),
            ServiceType = AppointmentServiceTypeTextBox.Text.Trim(),
            TechnicianName = AppointmentTechnicianTextBox.Text.Trim(),
            ScheduledDate = scheduledDate,
            StartTime = AppointmentStartTimeTextBox.Text.Trim(),
            Status = string.IsNullOrWhiteSpace(AppointmentStatusTextBox.Text) ? "scheduled" : AppointmentStatusTextBox.Text.Trim(),
            FeeAmount = feeAmount,
            PaidAmount = paidAmount,
            PaymentStatus = string.IsNullOrWhiteSpace(AppointmentPaymentStatusTextBox.Text) ? "unpaid" : AppointmentPaymentStatusTextBox.Text.Trim()
        };
    }

    private void ClearAppointmentForm()
    {
        _selectedAppointment = null;
        AppointmentDataGrid.SelectedItem = null;
        AppointmentCustomerIdTextBox.Text = string.Empty;
        AppointmentAddressTextBox.Text = string.Empty;
        AppointmentCityTextBox.Text = string.Empty;
        AppointmentServiceTypeTextBox.Text = string.Empty;
        AppointmentTechnicianTextBox.Text = string.Empty;
        AppointmentDateTextBox.Text = string.Empty;
        AppointmentStartTimeTextBox.Text = string.Empty;
        AppointmentStatusTextBox.Text = "scheduled";
        AppointmentFeeTextBox.Text = "0";
        AppointmentPaidTextBox.Text = "0";
        AppointmentPaymentStatusTextBox.Text = "unpaid";
    }

    private static string? ToNullable(string value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}