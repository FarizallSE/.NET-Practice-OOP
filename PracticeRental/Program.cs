using PracticeRental.Model;
using PracticeRental.Model.Vehicles;
using PracticeRental.Services;

namespace PracticeRental;

class Program
{
    private RentalSystem _system = new();

    static void Main(string[] args)
    {
        var program = new Program();
        program.Run();
    }

    void Run()
    {
        _system.SeedData();

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
                    _system.ShowCatalog(true);
                    break;
                case "2":
                    CreateNewTransaction();
                    break;
                case "3":
                    ProcessReturn();
                    break;
                case "4":
                    AddNewCustomer();
                    break;
                case "5":
                    _system.AddVehicle();
                    break;
                case "6":
                    _system.PrintAllTransactions();
                    _system.Print();
                    break;
                case "0":
                    return;
            }
            Console.WriteLine("\nEnter untuk melanjutkan...");
            Console.ReadLine();
        }
    }

    void CreateNewTransaction()
    {
        _system.ShowCatalog(true);

        var customer = GetOrCreateCustomer();
        if (customer == null) return;

        var transaction = _system.CreateTransaction(customer);
        if (transaction == null)
        {
            Console.WriteLine("Gagal membuat transaksi.");
            return;
        }

        while (true)
        {
            Console.Write("\nPlat Nomor: ");
            var plate = Console.ReadLine()!;
            var vehicle = _system.FindVehicle(plate);

            if (vehicle == null)
            {
                Console.WriteLine("Kendaraan tidak ditemukan.");
                continue;
            }
            if (!vehicle.IsAvailable)
            {
                Console.WriteLine("Kendaraan sedang disewa.");
                continue;
            }

            Console.Write("Durasi (hari): ");
            if (!int.TryParse(Console.ReadLine(), out int days) || days <= 0)
            {
                Console.WriteLine("Durasi tidak valid.");
                continue;
            }

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

            if (transaction.AddItem(item))
                Console.WriteLine("Item ditambahkan.");
            else
                Console.WriteLine("Gagal menambahkan item.");

            Console.Write("Tambah item lagi? (y/n): ");
            if (Console.ReadLine()!.Trim().ToLower() != "y") break;
        }

        transaction.Print();
    }

    void ProcessReturn()
    {
        Console.Write("ID Transaksi: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        {
            Console.WriteLine("ID tidak valid.");
            return;
        }

        var transaction = _system.FindTransaction(id);
        if (transaction == null)
        {
            Console.WriteLine("Transaksi tidak ditemukan.");
            return;
        }

        if (transaction.IsCompleted)
        {
            Console.WriteLine("Transaksi sudah selesai.");
            return;
        }

        Console.WriteLine("\nItem dalam transaksi:");
        foreach (var item in transaction.GetItems())
        {
            Console.WriteLine($"  {item.TargetVehicle.LicensePlate} - {item.TargetVehicle.Brand} ({(item.IsReturned ? "Sudah kembali" : "Belum kembali")})");
        }

        Console.Write("\nPlat Nomor yang dikembalikan: ");
        var plate = Console.ReadLine()!;

        Console.Write("Jam terlambat: ");
        if (!int.TryParse(Console.ReadLine(), out int hoursLate) || hoursLate < 0)
        {
            Console.WriteLine("Input tidak valid.");
            return;
        }

        if (transaction.ProcessReturn(plate, hoursLate))
        {
            Console.WriteLine("Pengembalian berhasil diproses.");
            transaction.Print();
        }
        else
        {
            Console.WriteLine("Kendaraan tidak ditemukan dalam transaksi ini.");
        }
    }

    void AddNewCustomer()
    {
        Console.Write("ID Pelanggan: ");
        if (!int.TryParse(Console.ReadLine(), out int id) || id <= 0)
        {
            Console.WriteLine("ID tidak valid.");
            return;
        }

        Console.Write("Nama: ");
        var name = Console.ReadLine()!;
        if (string.IsNullOrWhiteSpace(name))
        {
            Console.WriteLine("Nama tidak boleh kosong.");
            return;
        }

        Console.Write("Nomor Kontak: ");
        var contact = Console.ReadLine()!;
        if (string.IsNullOrWhiteSpace(contact))
        {
            Console.WriteLine("Kontak tidak boleh kosong.");
            return;
        }

        if (_system.AddCustomer(new Customer(id, name, contact)))
            Console.WriteLine("Pelanggan berhasil ditambahkan.");
        else
            Console.WriteLine("Gagal menambahkan pelanggan (ID sudah terdaftar).");
    }

    Customer? GetOrCreateCustomer()
    {
        Console.Write("ID Pelanggan: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        {
            Console.WriteLine("ID tidak valid.");
            return null;
        }

        var customer = _system.FindCustomer(id);
        if (customer != null) return customer;

        Console.WriteLine("Pelanggan tidak ditemukan. Buat baru? (y/n): ");
        if (Console.ReadLine()!.Trim().ToLower() != "y") return null;

        Console.Write("Nama: ");
        var name = Console.ReadLine()!;
        Console.Write("Nomor Kontak: ");
        var contact = Console.ReadLine()!;

        if (_system.AddCustomer(new Customer(id, name, contact)))
        {
            Console.WriteLine("Pelanggan baru ditambahkan.");
            return _system.FindCustomer(id);
        }

        Console.WriteLine("Gagal menambahkan pelanggan.");
        return null;
    }
}
