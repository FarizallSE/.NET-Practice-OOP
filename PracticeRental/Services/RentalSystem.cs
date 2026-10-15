using PracticeRental.Interface;
using PracticeRental.Model;
using PracticeRental.Model.Vehicles;

namespace PracticeRental.Services;

public class RentalSystem : IPrintable
{
    private const int MaxFleet = 100;
    private const int MaxTransaction = 500;
    private const int MaxCustomer = 500;

    // Penyimpanan data internal
    private readonly Vehicle[] catalog = new Vehicle[MaxFleet];
    private int catalogCount;

    private readonly RentalTransaction[] transactions = new RentalTransaction[MaxTransaction];
    private int transactionCount;

    private readonly Customer[] customers = new Customer[MaxCustomer];
    private int customerCount;
    
    private bool CanCreateTransaction => transactionCount < MaxTransaction;

    //Kendaraan 
    public bool AddVehicle(Vehicle vehicle)
    {
        if (catalogCount >= MaxFleet || FindVehicle(vehicle.LicensePlate) != null) return false;
        catalog[catalogCount] = vehicle;
        catalogCount++;
        return true;
    }

    public Vehicle? FindVehicle(string licensePlate)
    {
        for (int i = 0; i < catalogCount; i++)
        {
            if (catalog[i].LicensePlate.Equals(licensePlate, StringComparison.OrdinalIgnoreCase))
                return catalog[i];
        }
        return null;
    }

    public void ShowCatalog(bool availableOnly = false)
    {
        Console.WriteLine("\n--- Katalog Kendaraan ---");
        if (catalogCount == 0)
        {
            Console.WriteLine("Katalog masih kosong.");
            return;
        }

        for (int i = 0; i < catalogCount; i++)
        {
            var v = catalog[i];
            if (availableOnly && !v.IsAvailable) continue;
            Console.WriteLine($"[{v.GetTypeVehicle()}] {v.Brand} ({v.Year}) -- {v.LicensePlate} -- Rp {v.DailyRate:N0}/hari -- {(v.IsAvailable ? "Tersedia" : "Disewa")}");
        }
    }

    // Input
    public string ReadVehicleType()
    {
        Console.Write("Jenis (1=Car, 2=Bus, 3=Bike): ");
        return Console.ReadLine()!;
    }

    public void AddVehicle()
    {
        if (catalogCount >= MaxFleet)
        {
            Console.WriteLine("Kapasitas garasi sudah penuh!");
            return;
        }

        try
        {
            var type = ReadVehicleType();
            Console.Write("Plat Nomor: ");
            var plate = Console.ReadLine()?.Trim() ?? string.Empty;
            Console.Write("Merk: ");
            var brand = Console.ReadLine()?.Trim() ?? string.Empty;
            Console.Write("Tahun: ");
            if (!int.TryParse(Console.ReadLine(), out int year))
            {
                throw new Exception("Tahun harus berupa angka yang valid.");
            }
            Console.Write("Tarif Harian: ");
            if (!int.TryParse(Console.ReadLine(), out int rate))
            {
                throw new Exception("Tarif harian harus berupa angka yang valid.");
            }

            var v = CreateVehicleFromInput(type, plate, brand, year, rate);
            if (AddVehicle(v))
            {
                Console.WriteLine("Kendaraan berhasil ditambahkan.");
            }
            else
            {
                Console.WriteLine("Gagal menambahkan kendaraan: Plat nomor sudah terdaftar.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Error]: {ex.Message}");
        }
    }

    public Vehicle CreateVehicleFromInput(string type, string plate, string brand, int year, int dailyRate)
    {
        return type switch
        {
            "1" => new Car(plate, brand, year, dailyRate),
            "2" => new Bus(plate, brand, year, dailyRate),
            "3" => new Bike(plate, brand, year, dailyRate),
            _ => throw new Exception("Jenis kendaraan tidak valid (harus 1=Car, 2=Bus, atau 3=Bike).")
        };
    }

    public void SeedData()
    {
        AddVehicle(new Car("B 1234 AB", "Toyota", 2022, 300000));
        AddVehicle(new Car("B 5678 CD", "Honda", 2023, 350000));
        AddVehicle(new Bus("D 9011 EF", "Mercedes", 2021, 1000000));
        AddVehicle(new Bike("B 2222 GH", "Yamaha", 2023, 100000));
        AddVehicle(new Bike("D 3333 IJ", "Honda", 2022, 80000));

        AddCustomer(new Customer(1, "Budi Santoso", "08123456789"));
        AddCustomer(new Customer(2, "Siti Rahma", "08567891234"));

       var customer1 = FindCustomer(1);
        var vehicle1 = FindVehicle("B 1234 AB");
        if (customer1 != null && vehicle1 != null)
        {
            var trx1 = CreateTransaction(customer1);
            trx1?.AddItem(new RentalItem(vehicle1, 3));
        }
    }

    //Pelanggan 
    public Customer? FindCustomer(int idCustomer)
    {
        for (int i = 0; i < customerCount; i++)
        {
            if (customers[i].IdCustomer == idCustomer) return customers[i];
        }
        return null;
    }

    public bool AddCustomer(Customer customer)
    {
        if (customerCount >= MaxCustomer || FindCustomer(customer.IdCustomer) != null) return false;
        customers[customerCount] = customer;
        customerCount++;
        return true;
    }

    //Transaksi
    public RentalTransaction? CreateTransaction(Customer customer)
    {
        if (!CanCreateTransaction) return null;
        RentalTransaction transaction = new RentalTransaction(transactionCount + 1, customer);
        transactions[transactionCount] = transaction;
        transactionCount++;
        return transaction;
    }

    public RentalTransaction? FindTransaction(int idTransaction)
    {
        for (int i = 0; i < transactionCount; i++)
        {
            if (transactions[i].IdTransaction == idTransaction) return transactions[i];
        }
        return null;
    }

    public RentalTransaction[] GetAllTransactions()
    {
        RentalTransaction[] result = new RentalTransaction[transactionCount];
        Array.Copy(transactions, result, transactionCount);
        return result;
    }

    public void PrintAllTransactions()
    {
        if (transactionCount == 0)
        {
            Console.WriteLine("Belum ada transaksi.");
            return;
        }
        for (int i = 0; i < transactionCount; i++)
        {
            transactions[i].Print();
        }
    }

    //Pengembalian
    public RentalTransaction? FindActiveTransactionByVehicle(string licensePlate)
    {
        for (int i = 0; i < transactionCount; i++)
        {
            if (transactions[i].IsCompleted) continue;
            foreach (var item in transactions[i].GetItems())
            {
                if (!item.IsReturned && item.TargetVehicle.LicensePlate.Equals(licensePlate.Trim(), StringComparison.OrdinalIgnoreCase))
                {
                    return transactions[i];
                }
            }
        }
        return null;
    }

    //Laporan 
    public decimal CalculateTotalRevenue()
    {
        decimal total = 0;
        for (int i = 0; i < transactionCount; i++)
        {
            if (!transactions[i].IsCompleted) continue;
            total += transactions[i].CalculateTotalCost();
        }
        return total;
    }

    public int GetCompletedTransactionCount()
    {
        int completed = 0;
        for (int i = 0; i < transactionCount; i++)
        {
            if (transactions[i].IsCompleted) completed++;
        }
        return completed;
    }

    public void Print()
    {
        Console.WriteLine("====================================================================");
        Console.WriteLine("                      LAPORAN SISTEM RENTAL                         ");
        Console.WriteLine("====================================================================");
        Console.WriteLine($" Total Kendaraan di Garasi : {catalogCount} unit");
        Console.WriteLine($" Total Pelanggan Terdaftar : {customerCount} orang");
        Console.WriteLine($" Total Transaksi           : {transactionCount}");
        Console.WriteLine($" Transaksi Selesai         : {GetCompletedTransactionCount()}");
        Console.WriteLine($" Transaksi Berjalan        : {transactionCount - GetCompletedTransactionCount()}");
        Console.WriteLine("--------------------------------------------------------------------");
        Console.WriteLine($" TOTAL PENDAPATAN RESMI    : Rp {CalculateTotalRevenue(),12:N0}");
        Console.WriteLine("====================================================================");
    }
}