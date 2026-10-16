using RentalVehicelFix.Interfaces;
using RentalVehicelFix.Models;
using RentalVehicelFix.Models.Vehicles;

namespace RentalVehicelFix.Services;

public class RentalTransaction
{
    public Customer customer { get; private set; }
    public Customer Customer => customer;

    // private Vehicle[] _vehicles = new Vehicle[5];
    // private int _count = 0;

    public List<Vehicle> _vehicles = new List<Vehicle>();
    public IReadOnlyList<Vehicle> Vehicles => _vehicles;

    private int _rentalDays;
    public int RentalDays
    {
        get => _rentalDays;
        set
        {
            // if (value < 1)
            // {
            //     throw new ArgumentOutOfRangeException(nameof(value), "Rental Days must be greater than zero.");
            // }
            if (value < 1 || value > 31)
            {
                throw new ArgumentOutOfRangeException(nameof(value), "Rental Days must be between 1 and 30 days.");
            }
            _rentalDays = value;
        }
    }

    public RentalTransaction(Customer customer, int rentalDays)
    {
        this.customer = customer ?? throw new ArgumentNullException(nameof(customer));
        RentalDays = rentalDays;
    }
    
    public bool Add(Vehicle vehicle)
    {
        if (vehicle == null) throw new ArgumentNullException(nameof(vehicle));

        if (_vehicles.Count >= 5)
        {
            // Console.WriteLine("[Gagal] Batas maksimal sewa per transaksi adalah 5 kendaraan.");
            // return false;
            throw new InvalidOperationException("Jumlah maksimum kendaraan yang disewa per transaksi adalah 5");
        }

        if (vehicle.Status == "Renting")
        {
            // Console.WriteLine($"[Gagal] Kendaraan {vehicle.PlateNumber} sedang tidak tersedia.");
            // return false;
            throw new InvalidOperationException($"Kendaraan denga plat nomor {vehicle.PlateNumber} sedang disewakan");
        }

        vehicle.Rent();
        // _vehicles[Count] = vehicle;
        // _count++;
        _vehicles.Add(vehicle);
        return true;
    }

    public bool Add(Vehicle vehicle, bool driver)
    {
        if (vehicle is Car car)
        {
            car.WithDriver = driver;
        }
        else if (vehicle is Bus bus)
        {
            bus.WithDriver = driver;
        }
        return Add(vehicle);
    }

    public int CalculateDiscount()
    {
        int subtotal = 0;
        // for (int i = 0; i < _count; i++)
        // {
        //     if (_vehicles[i] != null)
        //     {
        //         subtotal += _vehicles[i].GetCost(RentalDays);
        //     }
        // }
        foreach (var v in _vehicles)
        {
            subtotal += v.GetCost(RentalDays);
        }

        // if (RentalDays >= 7 || _count >= 3 || RentalDays >= 5)
        if (RentalDays >= 7 || _vehicles.Count >= 3 || RentalDays >= 5)
        {
            return (int)(subtotal * 0.10);
        }

        return 0;
    }

    public int Return(int lateHours)
    {
        int subtotal = 0;
        int totalFine = 0;

        // for (int i = 0; i < _count; i++)
        // {
        //     if (_vehicles[i] != null)
        //     {
        //         subtotal += _vehicles[i].GetCost(RentalDays);
        //         totalFine += _vehicles[i].CalculateFine(lateHours);
        //         _vehicles[i].Return();
        //     }
        // }
        foreach (var v in _vehicles)
        {
            subtotal += v.GetCost(RentalDays);
            totalFine += v.CalculateFine(lateHours);
            v.Return();
        }

        int discount = CalculateDiscount();
        int grandTotal = (subtotal - discount) + totalFine;
        return grandTotal;
    }

    public void printInvoice()
    {
        Console.WriteLine("\n=======================================================");
        Console.WriteLine("                    FAKTUR SEWA                       ");
        Console.WriteLine("=======================================================");
        customer?.Info();
        Console.WriteLine($"Durasi Sewa   : {RentalDays} Hari");
        // Console.WriteLine($"Jumlah Unit   : {_count} Unit");
        Console.WriteLine($"Jumlah Unit   : {_vehicles.Count} Unit");
        Console.WriteLine("-------------------------------------------------------");

        int subtotal = 0;
        // for (int i = 0; i < _count; i++)
        // {
        //     Vehicle v = _vehicles[i];
        //     if (v == null) continue;
        //
        //     int itemCost = v.GetCost(RentalDays);
        //     subtotal += itemCost;
        //
        //     string driverInfo = "";
        //     if (v is Car car && car.WithDriver)
        //     {
        //         driverInfo = $" (Termasuk Sopir: Rp {car.DriverCost(RentalDays):N0})";
        //     }
        //     else if (v is Bus bus && bus.WithDriver)
        //     {
        //         driverInfo = $" (Termasuk Sopir: Rp {bus.DriverCost(RentalDays):N0})";
        //     }
        //     else if (v is Motorcycle m && m.ExtraHelmet)
        //     {
        //         driverInfo = " (Termasuk Helm Tambahan)";
        //     }
        //
        //     Console.WriteLine($"{i + 1}. {v.GetType().Name,-10} | {v.PlateNumber} - {v.Brand} | Rp {itemCost,10:N0}{driverInfo}");
        // }
        for (int i = 0; i < _vehicles.Count; i++)
        {
            Vehicle v = _vehicles[i];
            int itemCost = v.GetCost(RentalDays);
            subtotal += itemCost;

            string driverInfo = "";
            if (v is Car car && car.WithDriver)
            {
                driverInfo = $" (Termasuk Sopir: Rp {car.DriverCost(RentalDays):N0})";
            }
            else if (v is Bus bus && bus.WithDriver)
            {
                driverInfo = $" (Termasuk Sopir: Rp {bus.DriverCost(RentalDays):N0})";
            }
            else if (v is Motorcycle m && m.ExtraHelmet)
            {
                driverInfo = " (Termasuk Helm Tambahan)";
            }

            Console.WriteLine($"{i + 1}. {v.GetType().Name,-10} | {v.PlateNumber} - {v.Brand} | Rp {itemCost,10:N0}{driverInfo}");
        }

        int discount = CalculateDiscount();
        int netTotal = subtotal - discount;

        Console.WriteLine("-------------------------------------------------------");
        Console.WriteLine($"Subtotal      : Rp {subtotal,14:N0}");
        Console.WriteLine($"Diskon        : Rp {discount,14:N0}");
        Console.WriteLine($"Total Tagihan : Rp {netTotal,14:N0}");
        Console.WriteLine("=======================================================\n");
    }
}