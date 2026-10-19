using HomeServiceAppointmentApp.Wpf.Models;
using Xunit;

namespace HomeServiceAppointmentApp.Tests;

public class ModelChecks
{
    [Fact]
    public void Customer_starts_with_empty_name_and_email()
    {
        var customer = new Customer();

        Assert.Equal(string.Empty, customer.Name);
        Assert.Equal(string.Empty, customer.Email);
        Assert.Null(customer.Phone);
        Assert.Null(customer.DefaultAddress);
    }

    [Fact]
    public void Appointment_starts_scheduled_and_unpaid()
    {
        var appointment = new HomeServiceAppointment();

        Assert.Equal("scheduled", appointment.Status);
        Assert.Equal("unpaid", appointment.PaymentStatus);
        Assert.Equal(0m, appointment.FeeAmount);
        Assert.Equal(0m, appointment.PaidAmount);
    }
}
