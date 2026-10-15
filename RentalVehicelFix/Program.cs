using RentalVehicelFix.Models;
using RentalVehicelFix.Models.Vehicles;
using RentalVehicelFix.Services;

namespace RentalVehicelFix;

class Program
{
    private static Vehicle[] fleetData = new Vehicle[]
    {
        new Motorcycle(1, "B 1111 MOT", "Honda Vario", 2022, 80000, extraHelmet: true),
        new Motorcycle(2, "B 2222 MOT", "Yamaha NMAX", 2023, 100000, extraHelmet: false),
        new Car(3, "B 3333 MOB", "Toyota Avanza", 2021, 350000, withDriver: false),
        new Car(4, "B 4444 MOB", "Innova Zenix", 2024, 650000, withDriver: false),
        new Bus(5, "B 5555 BUS", "Isuzu Elf Long", 2020, 1200000, seatCapacity: 19, withDriver: false)
    };

    private static Customer[] customerData = new Customer[50];
    private static int customerCount = 0;

    private static RentalTransaction[] transactionData = new RentalTransaction[100];
    private static int transactionCount = 0;

    private static Rental rentalApp = new Rental(fleetData);

    static void Main(string[] args)
    {
        // Data awal pelanggan
        customerData[customerCount++] = new Customer("CUST001", "Budi Santoso", "08123456789");
        customerData[customerCount++] = new Customer("CUST002", "Siti Rahma", "08567891234");

        while (true)
        {
            Console.Clear();
            Console.WriteLine("===== SISTEM RENTAL KENDARAAN =====");
            Console.WriteLine("1. Lihat Katalog Kendaraan");
            Console.WriteLine("2. Buat Transaksi Baru");
            Console.WriteLine("3. Proses Pengembalian");
            Console.WriteLine("4. Tambah Pelanggan");
            Console.WriteLine("5. Laporan Pendapatan per Jenis Kendaraan");
            Console.WriteLine("0. Keluar");
            Console.Write("Pilih: ");
            var choice = Console.ReadLine()?.Trim();

            switch (choice)
            {
                case "1":
                    rentalApp.printAvailable();
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
                case "7":
                    rentalApp.PrintRevenueByType();
                    break;
                case "0":
                    return;
                default:
                    Console.WriteLine("Pilihan tidak valid.");
                    break;
            }

            Console.WriteLine("\nEnter untuk melanjutkan...");
            Console.ReadLine();
        }
    }

    static void CreateNewTransaction()
    {
        Console.WriteLine("\n--- Buat Transaksi Baru ---");
        rentalApp.printAvailable();

        var customer = GetOrCreateCustomer();
        if (customer == null)
        {
            Console.WriteLine("Gagal mendapatkan data pelanggan.");
            return;
        }

        Console.Write("\nDurasi Sewa (hari): ");
        if (!int.TryParse(Console.ReadLine(), out int days) || days <= 0)
        {
            Console.WriteLine("Durasi sewa minimal 1 hari.");
            return;
        }

        var trx = new RentalTransaction(customer, days);

        while (true)
        {
            Console.Write("\nMasukkan Plat Nomor Kendaraan: ");
            var plate = Console.ReadLine()?.Trim() ?? string.Empty;
            var vehicle = rentalApp.FindVehicle(plate);

            if (vehicle == null)
            {
                Console.WriteLine("Kendaraan dengan plat tersebut tidak ditemukan.");
            }
            else if (vehicle.Status == "Renting")
            {
                Console.WriteLine($"Kendaraan {vehicle.PlateNumber} sedang tidak tersedia.");
            }
            else
            {
                bool withDriver = false;
                if (vehicle is Car || vehicle is Bus)
                {
                    Console.Write("Pakai sopir? (y/n): ");
                    withDriver = Console.ReadLine()?.Trim().ToLower() == "y";
                }
                else if (vehicle is Motorcycle motor)
                {
                    Console.Write("Extra helm? (y/n): ");
                    motor.ExtraHelmet = Console.ReadLine()?.Trim().ToLower() == "y";
                }

                if (trx.Add(vehicle, withDriver))
                {
                    Console.WriteLine($"Kendaraan {vehicle.PlateNumber} berhasil ditambahkan ke transaksi.");
                }
            }

            Console.Write("Tambah kendaraan lagi? (y/n): ");
            if (Console.ReadLine()?.Trim().ToLower() != "y") break;
        }

        trx.printInvoice();
        rentalApp.Record(trx);

        if (transactionCount < transactionData.Length)
        {
            transactionData[transactionCount++] = trx;
        }
    }

    static void ProcessReturn()
    {
        Console.WriteLine("\n--- Proses Pengembalian Kendaraan ---");
        if (transactionCount == 0)
        {
            Console.WriteLine("Belum ada transaksi.");
            return;
        }

        Console.WriteLine("Daftar Transaksi:");
        for (int i = 0; i < transactionCount; i++)
        {
            Console.WriteLine($"{i + 1}. Transaksi Pelanggan: {transactionData[i].customer?.Name} ({transactionData[i].RentalDays} hari)");
        }

        Console.Write("Pilih nomor transaksi yang ingin dikembalikan: ");
        if (!int.TryParse(Console.ReadLine(), out int idx) || idx < 1 || idx > transactionCount)
        {
            Console.WriteLine("Pilihan transaksi tidak valid.");
            return;
        }

        var trx = transactionData[idx - 1];

        Console.Write("Masukkan keterlambatan pengembalian (dalam jam, ketik 0 jika tepat waktu): ");
        if (!int.TryParse(Console.ReadLine(), out int lateHours) || lateHours < 0)
        {
            Console.WriteLine("Jam keterlambatan tidak valid.");
            return;
        }

        int totalCost = trx.Return(lateHours);
        Console.WriteLine("\n========================================================");
        Console.WriteLine(">>> PENGEMBALIAN KENDARAAN BERHASIL DIPROSES <<<");
        Console.WriteLine($"Keterlambatan : {(lateHours > 0 ? $"{lateHours} jam (Dikenakan denda)" : "Tepat waktu (Tanpa denda)")}");
        Console.WriteLine($"Total Biaya   : Rp {totalCost:N0}");
        Console.WriteLine("=======================================================");
    }

    static void AddNewCustomer()
    {
        Console.WriteLine("\n--- Tambah Pelanggan Baru ---");
        Console.Write("ID Pelanggan : ");
        var id = Console.ReadLine()?.Trim() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(id))
        {
            Console.WriteLine("ID Pelanggan tidak boleh kosong.");
            return;
        }

        Console.Write("Nama         : ");
        var name = Console.ReadLine()?.Trim() ?? string.Empty;
        Console.Write("Nomor Telepon: ");
        var phone = Console.ReadLine()?.Trim() ?? string.Empty;

        if (customerCount < customerData.Length)
        {
            customerData[customerCount++] = new Customer(id, name, phone);
            Console.WriteLine("Pelanggan berhasil ditambahkan.");
        }
        else
        {
            Console.WriteLine("Kapasitas penyimpanan pelanggan penuh.");
        }
    }

    static Customer? GetOrCreateCustomer()
    {
        if (customerCount > 0)
        {
            Console.WriteLine("\nPilih Pelanggan Terdaftar:");
            for (int i = 0; i < customerCount; i++)
            {
                Console.WriteLine($"{i + 1}. [{customerData[i].CustomerID}] {customerData[i].Name} ({customerData[i].PhoneNumber})");
            }
            Console.Write("Pilih nomor pelanggan (atau ketik 0 untuk buat baru): ");
            if (int.TryParse(Console.ReadLine(), out int sel) && sel >= 1 && sel <= customerCount)
            {
                return customerData[sel - 1];
            }
        }

        Console.WriteLine("\n--- Buat Data Pelanggan Baru ---");
        Console.Write("ID Pelanggan : ");
        var id = Console.ReadLine()?.Trim() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(id)) return null;

        Console.Write("Nama         : ");
        var name = Console.ReadLine()?.Trim() ?? string.Empty;
        Console.Write("Nomor Telepon: ");
        var phone = Console.ReadLine()?.Trim() ?? string.Empty;

        var newCust = new Customer(id, name, phone);
        if (customerCount < customerData.Length)
        {
            customerData[customerCount++] = newCust;
        }
        return newCust;
    }
}