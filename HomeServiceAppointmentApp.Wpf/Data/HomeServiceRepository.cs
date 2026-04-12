using HomeServiceAppointmentApp.Wpf.Models;
using Microsoft.Data.SqlClient;

namespace HomeServiceAppointmentApp.Wpf.Data;

public class HomeServiceRepository
{
    private const string MasterConnectionString =
        "Server=(localdb)\\MSSQLLocalDB;Database=master;Trusted_Connection=True;TrustServerCertificate=True;";

    private const string ConnectionString =
        "Server=(localdb)\\MSSQLLocalDB;Database=HomeServiceDB;Trusted_Connection=True;TrustServerCertificate=True;";

    public void EnsureDatabase()
    {
        // Must create the database on `master` first; connecting directly to HomeServiceDB fails if it does not exist yet.
        const string createDbSql = """
                                   IF NOT EXISTS (SELECT 1 FROM sys.databases WHERE name = N'HomeServiceDB')
                                       CREATE DATABASE HomeServiceDB;
                                   """;

        using (var masterConnection = new SqlConnection(MasterConnectionString))
        using (var createDbCmd = new SqlCommand(createDbSql, masterConnection))
        {
            masterConnection.Open();
            createDbCmd.ExecuteNonQuery();
        }

        const string sql = """
                           IF OBJECT_ID('dbo.Customer', 'U') IS NULL
                           BEGIN
                               CREATE TABLE dbo.Customer (
                                   customer_id INT IDENTITY(1,1) PRIMARY KEY,
                                   name NVARCHAR(100) NOT NULL,
                                   email NVARCHAR(120) NOT NULL UNIQUE,
                                   phone NVARCHAR(30) NULL,
                                   default_address NVARCHAR(250) NULL,
                                   created_at DATETIME2 NOT NULL DEFAULT SYSDATETIME()
                               );
                           END;

                           IF OBJECT_ID('dbo.HomeServiceAppointment', 'U') IS NULL
                           BEGIN
                               CREATE TABLE dbo.HomeServiceAppointment (
                                   appointment_id INT IDENTITY(1,1) PRIMARY KEY,
                                   customer_id INT NOT NULL,
                                   address_line1 NVARCHAR(180) NOT NULL,
                                   city NVARCHAR(80) NULL,
                                   service_type NVARCHAR(80) NOT NULL,
                                   technician_name NVARCHAR(100) NOT NULL,
                                   scheduled_date DATE NOT NULL,
                                   start_time NVARCHAR(20) NOT NULL,
                                   status NVARCHAR(30) NOT NULL DEFAULT 'scheduled',
                                   fee_amount DECIMAL(10,2) NOT NULL DEFAULT 0,
                                   paid_amount DECIMAL(10,2) NOT NULL DEFAULT 0,
                                   payment_status NVARCHAR(30) NOT NULL DEFAULT 'unpaid',
                                   created_at DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
                                   CONSTRAINT FK_HomeServiceAppointment_Customer
                                       FOREIGN KEY (customer_id) REFERENCES dbo.Customer(customer_id)
                               );
                           END;
                           """;

        using var connection = new SqlConnection(ConnectionString);
        using var command = new SqlCommand(sql, connection);
        connection.Open();
        command.ExecuteNonQuery();
    }

    public List<Customer> GetCustomers()
    {
        const string sql = """
                           SELECT customer_id, name, email, phone, default_address
                           FROM dbo.Customer
                           ORDER BY customer_id DESC;
                           """;
        var items = new List<Customer>();
        using var connection = new SqlConnection(ConnectionString);
        using var command = new SqlCommand(sql, connection);
        connection.Open();
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            items.Add(new Customer
            {
                CustomerId = reader.GetInt32(0),
                Name = reader.GetString(1),
                Email = reader.GetString(2),
                Phone = reader.IsDBNull(3) ? null : reader.GetString(3),
                DefaultAddress = reader.IsDBNull(4) ? null : reader.GetString(4)
            });
        }
        return items;
    }

    public void AddCustomer(Customer customer)
    {
        const string sql = """
                           INSERT INTO dbo.Customer (name, email, phone, default_address)
                           VALUES (@name, @email, @phone, @default_address);
                           """;
        using var connection = new SqlConnection(ConnectionString);
        using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@name", customer.Name);
        command.Parameters.AddWithValue("@email", customer.Email);
        command.Parameters.AddWithValue("@phone", (object?)customer.Phone ?? DBNull.Value);
        command.Parameters.AddWithValue("@default_address", (object?)customer.DefaultAddress ?? DBNull.Value);
        connection.Open();
        command.ExecuteNonQuery();
    }

    public void UpdateCustomer(Customer customer)
    {
        const string sql = """
                           UPDATE dbo.Customer
                           SET name = @name, email = @email, phone = @phone, default_address = @default_address
                           WHERE customer_id = @customer_id;
                           """;
        using var connection = new SqlConnection(ConnectionString);
        using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@customer_id", customer.CustomerId);
        command.Parameters.AddWithValue("@name", customer.Name);
        command.Parameters.AddWithValue("@email", customer.Email);
        command.Parameters.AddWithValue("@phone", (object?)customer.Phone ?? DBNull.Value);
        command.Parameters.AddWithValue("@default_address", (object?)customer.DefaultAddress ?? DBNull.Value);
        connection.Open();
        command.ExecuteNonQuery();
    }

    public void DeleteCustomer(int customerId)
    {
        const string sql = "DELETE FROM dbo.Customer WHERE customer_id = @customer_id;";
        using var connection = new SqlConnection(ConnectionString);
        using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@customer_id", customerId);
        connection.Open();
        command.ExecuteNonQuery();
    }
}
