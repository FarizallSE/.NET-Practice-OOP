using PracticeRental.Model;
using PracticeRental.Model.Vehicles;
using PracticeRental.Services;

namespace PracticeRental;

class Program
{
    private RentalSystem system = new();

    static void Main(string[] args)
    {
        var program = new Program();
        program.Run();
    }

    void Run()
    {
        system.SeedData();

        while (true)
        {
            Console.Clear();
            Console.WriteLine("===== SISTEM RENTAL KENDARAAN =====");
            Console.WriteLine("1. Lihat Katalog");
            Console.WriteLine("2. Buat Transaksi Baru");
            Console.WriteLine("3. Proses Pengembalian");
            Console.WriteLine("4. Tambah Pelanggan");
            Console.WriteLine("5. Tambah Kendaraan");
            Console.WriteLine("6. Laporan");
            Console.WriteLine("0. Keluar");
            Console.Write("Pilih: ");
            var choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    system.ShowCatalog(true);
                    break;
                case "2":
                    var newTransaction = CreateNewTransaction();
                    if (newTransaction != null)
                        PrintTransaction(newTransaction);
                    break;
                case "3":
                    var returnedTransaction = ProcessReturn();
                    if (returnedTransaction != null)
                        PrintTransaction(returnedTransaction);
                    break;
                case "4":
                    var customerAdded = AddNewCustomer();
                    PrintAddCustomerResult(customerAdded);
                    break;
                case "5":
                    system.AddVehicle();
                    break;
                case "6":
                    system.PrintAllTransactions();
                    system.Print();
                    break;
                case "0":
                    return;
            }
            Console.WriteLine("\nEnter untuk melanjutkan...");
            Console.ReadLine();
        }
    }

    RentalTransaction? CreateNewTransaction()
    {
        system.ShowCatalog(true);

        var customer = GetOrCreateCustomer();
        if (customer == null) return null;

        var transaction = system.CreateTransaction(customer);
        if (transaction == null) return null;

        while (true)
        {
            Console.Write("\nPlat Nomor: ");
            var plate = Console.ReadLine()!;
            var vehicle = system.FindVehicle(plate);

            if (vehicle == null || !vehicle.IsAvailable)
                continue;

            Console.Write("Durasi (hari): ");
            if (!int.TryParse(Console.ReadLine(), out int days) || days <= 0)
                continue;

            var item = new RentalItem(vehicle, days);

            if (vehicle is Car car)
            {
                Console.Write("Pakai sopir? (y/n): ");
                car.SetIsUseDrive(Console.ReadLine()!.Trim().ToLower() == "y");
            }
            else if (vehicle is Bike bike)
            {
                Console.Write("Extra helm? (y/n): ");
                bike.SetIsExtraHelmet(Console.ReadLine()!.Trim().ToLower() == "y");
            }

            transaction.AddItem(item);

            Console.Write("Tambah item lagi? (y/n): ");
            if (Console.ReadLine()!.Trim().ToLower() != "y") break;
        }

        return transaction;
    }

    RentalTransaction? ProcessReturn()
    {
        Console.Write("ID Transaksi: ");
        if (!int.TryParse(Console.ReadLine(), out int id)) return null;

        var transaction = system.FindTransaction(id);
        if (transaction == null || transaction.IsCompleted) return null;

        Console.WriteLine("\nItem dalam transaksi:");
        foreach (var item in transaction.GetItems())
        {
            Console.WriteLine($"  {item.TargetVehicle.LicensePlate} - {item.TargetVehicle.Brand} ({(item.IsReturned ? "Sudah kembali" : "Belum kembali")})");
        }

        Console.Write("\nPlat Nomor yang dikembalikan: ");
        var plate = Console.ReadLine()!;

        Console.Write("Jam terlambat: ");
        if (!int.TryParse(Console.ReadLine(), out int hoursLate) || hoursLate < 0) return null;

        transaction.ProcessReturn(plate, hoursLate);
        return transaction;
    }

    bool AddNewCustomer()
    {
        Console.Write("ID Pelanggan: ");
        if (!int.TryParse(Console.ReadLine(), out int id) || id <= 0) return false;

        Console.Write("Nama: ");
        var name = Console.ReadLine()!;
        if (string.IsNullOrWhiteSpace(name)) return false;

        Console.Write("Nomor Kontak: ");
        var contact = Console.ReadLine()!;
        if (string.IsNullOrWhiteSpace(contact)) return false;

        return system.AddCustomer(new Customer(id, name, contact));
    }

    Customer? GetOrCreateCustomer()
    {
        Console.Write("ID Pelanggan: ");
        if (!int.TryParse(Console.ReadLine(), out int id)) return null;

        var customer = system.FindCustomer(id);
        if (customer != null) return customer;

        Console.Write("Buat pelanggan baru? (y/n): ");
        if (Console.ReadLine()!.Trim().ToLower() != "y") return null;

        Console.Write("Nama: ");
        var name = Console.ReadLine()!;
        Console.Write("Nomor Kontak: ");
        var contact = Console.ReadLine()!;

        if (system.AddCustomer(new Customer(id, name, contact)))
            return system.FindCustomer(id);

        return null;
    }

    void PrintTransaction(RentalTransaction transaction)
    {
        transaction.Print();
    }

    void PrintAddCustomerResult(bool success)
    {
        Console.WriteLine(success ? "Pelanggan berhasil ditambahkan." : "Gagal menambahkan pelanggan.");
    }

    void PrintCustomerResult(Customer? customer)
    {
        if (customer != null)
            Console.WriteLine($"Pelanggan: {customer}");
        else
            Console.WriteLine("Pelanggan tidak ditemukan.");
    }
}
