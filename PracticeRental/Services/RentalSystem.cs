using PracticeRental.Interface;
using PracticeRental.Model.Vehicle;

namespace PracticeRental.Services;

public class RentalSystem : IPrintable
{
    private const int MAX_VEHICLES = 100;
    private static int currentCount = 0;

    public int VehicleCount()
    {
        return currentCount;
    }
    
    public Vehicle[]  vehicles = new Vehicle[MAX_VEHICLES];
    
    
    public void ShowCatalog(bool availableOnly)
    {
        Console.WriteLine("\n--- Katalog Kendaraan ---");
        for (int i = 0; i < VehicleCount(); i++)
        {
            var v = vehicles[i];
            if (availableOnly && !v.IsAvailable) continue;
            Console.WriteLine($"[{v.GetTypeVehicle()}] {v.Brand} {v.Year} -- {v.LicensePlate} -- Rp {v.DailyRate:N0}/hari -- {(v.IsAvailable ? "Tersedia" : "Disewa")}");
        }
    }
    
    public Vehicle? FindVehicle(string plate)
    {
        for (int i = 0; i < VehicleCount(); i++)
        {
            if (vehicles[i].LicensePlate == plate)
                return vehicles[i];
        }
        return null;
    }
    
    public string ReadVehicleType()
    {
        Console.Write("Jenis (1=Car, 2=Bus, 3=Motorcycle): ");
        return Console.ReadLine()!;
    }
    
    public void AddVehicle()
    {
        var type = ReadVehicleType();
        Console.Write("Plat Nomor: ");
        var plate = Console.ReadLine()!;
        Console.Write("Merk: ");
        var brand = Console.ReadLine()!;
        Console.Write("Tahun: ");
        var year = int.Parse(Console.ReadLine()!);
        Console.Write("Tarif Harian: ");
        var rate = int.Parse(Console.ReadLine()!);

        var v = CreateVehicleFromInput(type, plate, brand, year, rate);
        AddVehicle(v);
        Console.WriteLine("Kendaraan ditambahkan.");
    }
    
    public void AddVehicle(Vehicle v)
    {
        if (currentCount < MAX_VEHICLES)
        {
            vehicles[currentCount] = v;
            currentCount++; // Tambah jumlah setiap kali ada kendaraan masuk
        }
        else
        {
            Console.WriteLine("Kapasitas garasi sudah penuh!");
        }
    }
    
    public void SeedData()
    {
        AddVehicle(new Car("B 1234 AB", "Toyota", 2022, 300000));
        AddVehicle(new Car("B 5678 CD", "Honda", 2023, 350000));
        AddVehicle(new Bus("D 9011 EF", "Mercedes", 2021, 1000000));
        AddVehicle(new Bike("B 2222 GH", "Yamaha", 2023, 100000));
        AddVehicle(new Bike("D 3333 IJ", "Honda", 2022, 80000));
    }
    
    
    public Vehicle CreateVehicleFromInput(string type, string plate, string brand, int year, int dailyRate)
    {
        return type switch
        {
            "1" => new Car(plate, brand, year, dailyRate),
            "2" => new Bus(plate, brand, year, dailyRate),
            "3" => new Bike(plate, brand, year, dailyRate),
            _ => throw new ArgumentException("Jenis tidak valid.")
        };
    }
    
    public void PrintReceipt()
    {
        throw new NotImplementedException();
    }
}