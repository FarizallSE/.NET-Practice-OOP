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

    // Property helper
    public int CatalogCount => catalogCount;
    public int VehicleCount() => catalogCount; 
    public int TransactionCount => transactionCount;
    public int CustomerCount => customerCount;
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

    public Vehicle[] GetAvailableVehicles()
    {
        int count = 0;
        for (int i = 0; i < catalogCount; i++)
        {
            if (catalog[i].IsAvailable) count++;
        }

        Vehicle[] result = new Vehicle[count];
        int index = 0;
        for (int i = 0; i < catalogCount; i++)
        {
            if (catalog[i].IsAvailable) result[index++] = catalog[i];
        }
        return result;
    }

    public Vehicle[] GetAllVehicles()
    {
        Vehicle[] result = new Vehicle[catalogCount];
        Array.Copy(catalog, result, catalogCount);
        return result;
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

    public Customer[] GetAllCustomers()
    {
        Customer[] result = new Customer[customerCount];
        Array.Copy(customers, result, customerCount);
        return result;
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

    public bool ProcessReturn(int idTransaction, string licensePlate, int hoursLate)
    {
        var trx = FindTransaction(idTransaction);
        if (trx == null) return false;
        return trx.ProcessReturn(licensePlate, hoursLate);
    }

    public bool ProcessReturn(string licensePlate, int hoursLate)
    {
        var trx = FindActiveTransactionByVehicle(licensePlate);
        if (trx == null) return false;
        return trx.ProcessReturn(licensePlate, hoursLate);
    }

    public void ProcessReturnVehicle() => ProcessReturn();

    public void ProcessReturn()
    {
        Console.WriteLine("\n--- Proses Pengembalian Kendaraan ---");
        try
        {
            Console.WriteLine("Cari transaksi berdasarkan:");
            Console.WriteLine("1. ID Transaksi");
            Console.WriteLine("2. Nomor Kendaraan (Plat Nomor)");
            Console.Write("Pilih metode pencarian (1/2): ");
            string? searchChoice = Console.ReadLine()?.Trim();

            RentalTransaction? transaction = null;
            string licensePlate = string.Empty;

            if (searchChoice == "1")
            {
                Console.Write("Masukkan ID Transaksi: ");
                if (!int.TryParse(Console.ReadLine(), out int idTransaction))
                {
                    throw new Exception("ID Transaksi harus berupa angka.");
                }

                transaction = FindTransaction(idTransaction);
                if (transaction == null)
                {
                    throw new Exception($"Transaksi dengan ID {idTransaction} tidak ditemukan.");
                }

                if (transaction.IsCompleted)
                {
                    Console.WriteLine($"Transaksi #{transaction.IdTransaction:D4} sudah selesai (semua kendaraan telah dikembalikan).");
                    return;
                }

                var items = transaction.GetItems();
                var unreturnedItems = new List<RentalItem>();
                foreach (var item in items)
                {
                    if (!item.IsReturned)
                    {
                        unreturnedItems.Add(item);
                    }
                }

                if (unreturnedItems.Count == 0)
                {
                    Console.WriteLine("Tidak ada kendaraan yang perlu dikembalikan pada transaksi ini.");
                    return;
                }

                Console.WriteLine($"\nTransaksi #{transaction.IdTransaction:D4} - Pelanggan: {transaction.Customer.Name}");
                Console.WriteLine("Daftar kendaraan yang sedang disewa:");
                for (int i = 0; i < unreturnedItems.Count; i++)
                {
                    var v = unreturnedItems[i].TargetVehicle;
                    Console.WriteLine($"  {i + 1}. [{v.GetTypeVehicle()}] {v.Brand} - Plat: {v.LicensePlate}");
                }

                if (unreturnedItems.Count == 1)
                {
                    licensePlate = unreturnedItems[0].TargetVehicle.LicensePlate;
                    Console.WriteLine($"Kendaraan yang akan diproses: {licensePlate}");
                }
                else
                {
                    Console.Write("Masukkan nomor plat kendaraan yang ingin dikembalikan: ");
                    licensePlate = Console.ReadLine()?.Trim()!;
                }
            }
            else if (searchChoice == "2")
            {
                Console.Write("Masukkan Nomor Plat Kendaraan: ");
                licensePlate = Console.ReadLine()?.Trim()!;

                if (string.IsNullOrWhiteSpace(licensePlate))
                {
                    throw new Exception("Plat nomor tidak boleh kosong.");
                }

                transaction = FindActiveTransactionByVehicle(licensePlate);
                if (transaction == null)
                {
                    var vehicle = FindVehicle(licensePlate);
                    if (vehicle == null)
                    {
                        throw new Exception($"Kendaraan dengan plat \"{licensePlate}\" tidak terdaftar di sistem.");
                    }
                    if (vehicle.IsAvailable)
                    {
                        Console.WriteLine($"Kendaraan dengan plat \"{licensePlate}\" saat ini tidak sedang disewa (tersedia di garasi).");
                        return;
                    }
                    throw new Exception($"Tidak ditemukan transaksi aktif untuk kendaraan dengan plat \"{licensePlate}\".");
                }

                Console.WriteLine($"\nDitemukan Transaksi #{transaction.IdTransaction:D4} atas nama {transaction.Customer.Name}.");
            }
            else
            {
                throw new Exception("Pilihan metode pencarian tidak valid (harus 1 atau 2).");
            }

            Console.Write("Masukkan keterlambatan pengembalian (dalam jam, ketik 0 jika tepat waktu): ");
            if (!int.TryParse(Console.ReadLine(), out int hoursLate))
            {
                throw new Exception("Keterlambatan harus berupa angka jam.");
            }

            bool success = transaction.ProcessReturn(licensePlate, hoursLate);
            if (success)
            {
                Console.WriteLine("\n========================================================");
                Console.WriteLine(">>> PENGEMBALIAN KENDARAAN BERHASIL DIPROSES <<<");
                Console.WriteLine($"Kendaraan      : {licensePlate.ToUpper()}");
                Console.WriteLine($"Status Armada  : Tersedia kembali di garasi");
                Console.WriteLine($"Keterlambatan  : {(hoursLate > 0 ? $"{hoursLate} jam (Dikenakan denda)" : "Tepat waktu (Tanpa denda)")}");
                Console.WriteLine("========================================================");
                Console.WriteLine("\n--- INVOICE TERKINI TRANSAKSI ---");
                transaction.Print();
            }
            else
            {
                Console.WriteLine($"Gagal memproses pengembalian kendaraan \"{licensePlate}\". Pastikan plat nomor sesuai dan belum dikembalikan.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Error]: {ex.Message}");
        }
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