namespace HomeServiceAppointmentApp.Wpf.Models;

public class HomeServiceAppointment
{
    public int AppointmentId { get; set; }
    public int CustomerId { get; set; }
    public string AddressLine1 { get; set; } = string.Empty;
    public string? City { get; set; }
    public string ServiceType { get; set; } = string.Empty;
    public string TechnicianName { get; set; } = string.Empty;
    public DateTime ScheduledDate { get; set; }
    public string StartTime { get; set; } = string.Empty;
    public string Status { get; set; } = "scheduled";
    public decimal FeeAmount { get; set; }
    public decimal PaidAmount { get; set; }
    public string PaymentStatus { get; set; } = "unpaid";
}
