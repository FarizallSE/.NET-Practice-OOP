using RentalVehicelFix.Exceptions;
using RentalVehicelFix.Interfaces;
using RentalVehicelFix.Models;
using RentalVehicelFix.Models.Vehicles;

namespace RentalVehicelFix.Services;

public class RentalTransaction
{
    public Customer customer { get; private set; }
    public Customer Customer => customer;

    private List<Vehicle> _vehicles = new List<Vehicle>();
    private int _count = 0;

    private int _rentalDays;
    public int RentalDays
    {
        get => _rentalDays;
        set
        {
            if (value < 1 || value > 30)
            {
                throw new InvalidRentalDurationException("Lama sewa harus antara 1-30 hari.");
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
        if (vehicle == null) return false;

        if (_count >= 5)
        {
            Console.WriteLine("[Gagal] Batas maksimal sewa per transaksi adalah 5 kendaraan.");
            return false;
        }

        if (vehicle.Status == "Renting")
        {
            throw new VehicleNotAvailableException($"Kendaraan {vehicle.PlateNumber} sedang disewa.");
        }

        vehicle.Rent();
        _vehicles.Add(vehicle);
        _count++;
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

        foreach (var item in _vehicles)
        {
            subtotal += item.GetCost(RentalDays);
        }

        if (RentalDays >= 7 || _count >= 3 || RentalDays >= 5)
        {
            return (int)(subtotal * 0.10);
        }

        return 0;
    }

    public int Return(int lateHours)
    {
        int subtotal = 0;
        int totalFine = 0;
        
        foreach (var item in _vehicles) 
        {
            if (item != null)
            {
                subtotal += item.GetCost(RentalDays);
                totalFine += item.CalculateFine(lateHours);
                item.Return();
            }
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
        Console.WriteLine($"Jumlah Unit   : {_count} Unit");
        Console.WriteLine("-------------------------------------------------------");

        int subtotal = 0;
        int i = 1;
        foreach (var v in _vehicles.Take(_count))
        {
            if (v == null) continue;

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
            Console.WriteLine($"{i++}. {v.GetType().Name,-10} | {v.PlateNumber} - {v.Brand} | Rp {itemCost,10:N0}{driverInfo}");
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