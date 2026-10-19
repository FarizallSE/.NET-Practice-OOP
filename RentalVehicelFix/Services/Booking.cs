using RentalVehicelFix.Models;
using RentalVehicelFix.Models.Vehicles;

namespace RentalVehicelFix.Services;

public class Booking
{
    private static int _nextId = 1;

    public int BookingId { get; }
    public Customer Customer { get; }
    public List<Vehicle> Vehicles { get; }
    public int RentalDays { get; }
    public string Status { get; private set; } // "Pending" | "Paid" | "Cancelled"

    public Booking(Customer customer, List<Vehicle> vehicles, int rentalDays)
    {
        BookingId = _nextId++;
        Customer = customer;
        Vehicles = vehicles;
        RentalDays = rentalDays;
        Status = "Pending";
    }

    public void MarkAsPaid() => Status = "Paid";
    public void MarkAsCancelled() => Status = "Cancelled";

    public void Print()
    {
        Console.WriteLine($"\n--- BOOKING #{BookingId} ---");
        Console.WriteLine($"Pelanggan : {Customer.Name}");
        Console.WriteLine($"Durasi    : {RentalDays} hari");
        Console.WriteLine($"Status    : {Status}");
        Console.WriteLine("Kendaraan :");
        foreach (var v in Vehicles)
        {
            Console.WriteLine($"  - {v.GetType().Name} {v.PlateNumber} ({v.Brand})");
        }
    }
}
