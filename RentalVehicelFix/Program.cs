using RentalVehicelFix.Exceptions;
using RentalVehicelFix.Models;
using RentalVehicelFix.Models.Vehicles;
using RentalVehicelFix.Services;

namespace RentalVehicelFix;

class Program
{
    // private static Vehicle[] fleetData = new Vehicle[]
    private static List<Vehicle> fleetData = new List<Vehicle>
    {
        new Motorcycle(1, "B 1111 MOT", "Honda Vario", 2022, 80000, extraHelmet: true),
        new Motorcycle(2, "B 2222 MOT", "Yamaha NMAX", 2023, 100000, extraHelmet: false),
        new Car(3, "B 3333 MOB", "Toyota Avanza", 2021, 350000, withDriver: false),
        new Car(4, "B 4444 MOB", "Innova Zenix", 2024, 650000, withDriver: false),
        new Bus(5, "B 5555 BUS", "Isuzu Elf Long", 2020, 1200000, seatCapacity: 19, withDriver: false)
    };

    // private static Customer[] customerData = new Customer[50];
    // private static int customerCount = 0;
    private static List<Customer> customerData = new List<Customer>();

    // private static RentalTransaction[] transactionData = new RentalTransaction[100];
    // private static int transactionCount = 0;
    private static List<RentalTransaction> transactionData = new List<RentalTransaction>();

    private static Rental rentalApp = new Rental(fleetData);

    static void Main(string[] args)
    {
        // Data awal pelanggan
        // customerData[customerCount++] = new Customer("CUST001", "Budi Santoso", "08123456789");
        // customerData[customerCount++] = new Customer("CUST002", "Siti Rahma", "08567891234");
        customerData.Add(new Customer("CUST001", "Budi Santoso", "08123456789"));
        customerData.Add(new Customer("CUST002", "Siti Rahma", "08567891234"));

        while (true)
        {
            Console.Clear();
            Console.WriteLine("===== SISTEM RENTAL KENDARAAN =====");
            Console.WriteLine("1. Lihat Katalog Kendaraan");
            Console.WriteLine("2. Buat Transaksi Baru");
            Console.WriteLine("3. Proses Pengembalian");
            Console.WriteLine("4. Tambah Pelanggan");
            Console.WriteLine("5. Tambah Kendaraan Baru");
            Console.WriteLine("6. Laporan Pendapatan per Jenis Kendaraan");
            Console.WriteLine("7. Kendaraan Paling Menguntungkan");
            Console.WriteLine("0. Keluar");
            Console.Write("Pilih: ");
            var choice = Console.ReadLine()?.Trim();

            try
            {
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
                        AddNewVehicle();
                        break;
                    case "6":
                        rentalApp.PrintRevenueByType();
                        break;
                    case "7":
                        rentalApp.PrintMostProfitableVehicle();
                        break;
                    case "0":
                        return;
                    default:
                        Console.WriteLine("Pilihan tidak valid.");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n[Terjadi Kesalahan]: {ex.Message}");
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

        // Cek apakah pelanggan masuk daftar hitam (blacklist)
        if (rentalApp.IsBlacklisted(customer.CustomerID))
        {
            Console.WriteLine($"\n[TRANSAKSI DITOLAK] Pelanggan {customer.Name} ({customer.CustomerID}) masuk daftar hitam (blacklist) karena riwayat keterlambatan > 24 jam dan TIDAK BISA menyewa lagi!");
            return;
        }

        Console.Write("\nDurasi Sewa (hari, 1-30): ");
        if (!int.TryParse(Console.ReadLine(), out int days))
        {
            Console.WriteLine("[Validasi Gagal]: Input durasi sewa harus berupa angka.");
            return;
        }

        RentalTransaction trx;
        try
        {
            trx = new RentalTransaction(customer, days);
        }
        catch (InvalidRentalDurationException ex)
        {
            Console.WriteLine($"\n[Validasi Durasi Gagal]: {ex.Message}");
            return;
        }
        catch (ArgumentOutOfRangeException ex)
        {
            Console.WriteLine($"\n[Validasi Gagal]: {ex.Message}");
            return;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\n[Error]: {ex.Message}");
            return;
        }

        while (true)
        {
            Console.Write("\nMasukkan Plat Nomor Kendaraan: ");
            var plate = Console.ReadLine()?.Trim() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(plate))
            {
                Console.WriteLine("[Validasi Gagal]: Plat nomor kendaraan tidak boleh kosong.");
            }
            else
            {
                var vehicle = rentalApp.FindVehicle(plate);
                if (vehicle == null)
                {
                    Console.WriteLine($"Kendaraan dengan plat '{plate}' tidak ditemukan.");
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

                    try
                    {
                        if (trx.Add(vehicle, withDriver))
                        {
                            Console.WriteLine($"Kendaraan {vehicle.PlateNumber} berhasil ditambahkan ke transaksi.");
                        }
                    }
                    catch (VehicleNotAvailableException ex)
                    {
                        Console.WriteLine($"\n[Kendaraan Tidak Tersedia]: {ex.Message}");
                    }
                    catch (InvalidOperationException ex)
                    {
                        Console.WriteLine($"\n[Gagal Menambah Kendaraan]: {ex.Message}");
                    }
                    catch (ArgumentException ex)
                    {
                        Console.WriteLine($"\n[Validasi Gagal]: {ex.Message}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"\n[Error]: {ex.Message}");
                    }
                }
            }

            if (trx.Vehicles.Count >= 5)
            {
                Console.WriteLine("\n[Info] Jumlah sewa telah mencapai batas maksimal 5 kendaraan.");
                break;
            }

            Console.Write("Tambah kendaraan lagi? (y/n): ");
            if (Console.ReadLine()?.Trim().ToLower() != "y") break;
        }

        if (trx.Vehicles.Count == 0)
        {
            Console.WriteLine("\n[Batal] Transaksi dibatalkan karena tidak ada kendaraan yang berhasil ditambahkan.");
            return;
        }

        try
        {
            trx.printInvoice();
            rentalApp.Record(trx);
            transactionData.Add(trx);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\n[Error Pencatatan Transaksi]: {ex.Message}");
        }
    }

    static void ProcessReturn()
    {
        Console.WriteLine("\n--- Proses Pengembalian Kendaraan ---");
        try
        {
            if (transactionData.Count == 0)
            {
                Console.WriteLine("Belum ada transaksi.");
                return;
            }

            Console.WriteLine("Daftar Transaksi:");
            for (int i = 0; i < transactionData.Count; i++)
            {
                var t = transactionData[i];
                string status = t.IsReturned ? "[SUDAH KEMBALI]" : "[SEDANG DISEWA]";
                Console.WriteLine($"{i + 1}. {status} Transaksi Pelanggan: {t.Customer?.Name} ({t.RentalDays} hari, {t.Vehicles.Count} unit)");
            }

            Console.Write("Pilih nomor transaksi yang ingin dikembalikan: ");
            var choiceInput = Console.ReadLine()?.Trim();
            if (!int.TryParse(choiceInput, out int idx) || idx < 1 || idx > transactionData.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(idx), $"Pilihan transaksi tidak valid (harus antara 1 dan {transactionData.Count}).");
            }

            var trx = transactionData[idx - 1];

            if (trx.IsReturned)
            {
                throw new InvalidOperationException("Transaksi ini sudah pernah diproses pengembaliannya sebelumnya.");
            }

            Console.Write("Masukkan keterlambatan pengembalian (dalam jam, ketik 0 jika tepat waktu): ");
            var lateInput = Console.ReadLine()?.Trim();
            if (!int.TryParse(lateInput, out int lateHours) || lateHours < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(lateHours), "Jam keterlambatan harus berupa angka 0 atau lebih.");
            }

            int totalCost = rentalApp.ProcessReturn(trx, lateHours);
            Console.WriteLine("\n========================================================");
            Console.WriteLine(">>> PENGEMBALIAN KENDARAAN BERHASIL DIPROSES <<<");
            Console.WriteLine($"Keterlambatan : {(lateHours > 0 ? $"{lateHours} jam (Dikenakan denda)" : "Tepat waktu (Tanpa denda)")}");
            Console.WriteLine($"Total Biaya   : Rp {totalCost:N0}");
            Console.WriteLine("=======================================================");
        }
        catch (ArgumentOutOfRangeException ex)
        {
            Console.WriteLine($"\n[Validasi Nilai Gagal]: {ex.Message}");
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine($"\n[Gagal Pengembalian]: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\n[Error]: {ex.Message}");
        }
    }

    static void AddNewCustomer()
    {
        Console.WriteLine("\n--- Tambah Pelanggan Baru ---");
        try
        {
            Console.Write("ID Pelanggan : ");
            var id = Console.ReadLine()?.Trim() ?? string.Empty;
            if (customerData.Any(c => string.Equals(c.CustomerID, id, StringComparison.OrdinalIgnoreCase)))
            {
                throw new InvalidOperationException($"Pelanggan dengan ID '{id}' sudah terdaftar.");
            }

            Console.Write("Nama         : ");
            var name = Console.ReadLine()?.Trim() ?? string.Empty;

            Console.Write("Nomor Telepon: ");
            var phone = Console.ReadLine()?.Trim() ?? string.Empty;

            var newCustomer = new Customer(id, name, phone);
            customerData.Add(newCustomer);
            Console.WriteLine($"\n[Sukses] Pelanggan '{newCustomer.Name}' berhasil ditambahkan.");
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine($"\n[Validasi Gagal]: {ex.Message}");
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine($"\n[Operasi Gagal]: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\n[Error]: {ex.Message}");
        }
    }

    static Customer? GetOrCreateCustomer()
    {
        try
        {
            if (customerData.Count > 0)
            {
                Console.WriteLine("\nPilih Pelanggan Terdaftar:");
                for (int i = 0; i < customerData.Count; i++)
                {
                    Console.WriteLine($"{i + 1}. [{customerData[i].CustomerID}] {customerData[i].Name} ({customerData[i].PhoneNumber})");
                }
                Console.Write("Pilih nomor pelanggan (atau ketik 0 untuk buat baru): ");
                var input = Console.ReadLine()?.Trim();
                if (!int.TryParse(input, out int sel) || sel < 0 || sel > customerData.Count)
                {
                    throw new ArgumentOutOfRangeException(nameof(sel), $"Pilihan harus berupa angka antara 0 dan {customerData.Count}.");
                }

                if (sel > 0)
                {
                    return customerData[sel - 1];
                }
            }

            Console.WriteLine("\n--- Buat Data Pelanggan Baru ---");
            Console.Write("ID Pelanggan : ");
            var id = Console.ReadLine()?.Trim() ?? string.Empty;
            if (customerData.Any(c => string.Equals(c.CustomerID, id, StringComparison.OrdinalIgnoreCase)))
            {
                throw new InvalidOperationException($"Pelanggan dengan ID '{id}' sudah terdaftar.");
            }

            Console.Write("Nama         : ");
            var name = Console.ReadLine()?.Trim() ?? string.Empty;

            Console.Write("Nomor Telepon: ");
            var phone = Console.ReadLine()?.Trim() ?? string.Empty;

            var newCust = new Customer(id, name, phone);
            customerData.Add(newCust);
            return newCust;
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine($"\n[Validasi Gagal]: {ex.Message}");
            return null;
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine($"\n[Operasi Gagal]: {ex.Message}");
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\n[Error]: {ex.Message}");
            return null;
        }
    }

    static void AddNewVehicle()
    {
        Console.WriteLine("\n--- Tambah Kendaraan Baru ke Armada ---");
        try
        {
            Console.WriteLine("Pilih Tipe Kendaraan:");
            Console.WriteLine("1. Mobil (Car)");
            Console.WriteLine("2. Sepeda Motor (Motorcycle)");
            Console.WriteLine("3. Bus");
            Console.Write("Pilih (1-3): ");
            var typeChoice = Console.ReadLine()?.Trim();

            if (typeChoice != "1" && typeChoice != "2" && typeChoice != "3")
            {
                throw new ArgumentException("Pilihan tipe kendaraan tidak valid (harus 1, 2, atau 3).");
            }

            Console.Write("Plat Nomor       : ");
            var plate = Console.ReadLine()?.Trim() ?? string.Empty;

            if (rentalApp.FindVehicle(plate) != null)
            {
                throw new InvalidOperationException($"Kendaraan dengan plat '{plate}' sudah terdaftar dalam sistem.");
            }

            Console.Write("Merk / Model     : ");
            var brand = Console.ReadLine()?.Trim() ?? string.Empty;

            Console.Write("Tahun            : ");
            if (!int.TryParse(Console.ReadLine(), out int year))
            {
                throw new FormatException("Tahun kendaraan harus berupa angka.");
            }

            Console.Write("Tarif Harian (Rp): ");
            if (!int.TryParse(Console.ReadLine(), out int rate))
            {
                throw new FormatException("Tarif harian harus berupa angka.");
            }

            int nextId = fleetData.Count + 1;
            Vehicle newVehicle;

            switch (typeChoice)
            {
                case "1":
                    Console.Write("Sedia sopir? (y/n): ");
                    bool withDriver = Console.ReadLine()?.Trim().ToLower() == "y";
                    newVehicle = new Car(nextId, plate, brand, year, rate, withDriver);
                    break;
                case "2":
                    Console.Write("Extra helm? (y/n): ");
                    bool extraHelmet = Console.ReadLine()?.Trim().ToLower() == "y";
                    newVehicle = new Motorcycle(nextId, plate, brand, year, rate, extraHelmet);
                    break;
                case "3":
                    Console.Write("Kapasitas Kursi  : ");
                    if (!int.TryParse(Console.ReadLine(), out int seatCap))
                    {
                        throw new FormatException("Kapasitas kursi bus harus berupa angka.");
                    }
                    Console.Write("Sedia sopir? (y/n): ");
                    bool busDriver = Console.ReadLine()?.Trim().ToLower() == "y";
                    newVehicle = new Bus(nextId, plate, brand, year, rate, seatCap, busDriver);
                    break;
                default:
                    throw new ArgumentException("Tipe kendaraan tidak dikenal.");
            }

            fleetData.Add(newVehicle);
            rentalApp.AddVehicle(newVehicle);
            Console.WriteLine($"\n[Sukses] Kendaraan {newVehicle.Brand} [{newVehicle.PlateNumber}] berhasil ditambahkan ke armada!");
        }
        catch (ArgumentOutOfRangeException ex)
        {
            Console.WriteLine($"\n[Validasi Rentang Gagal]: {ex.Message}");
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine($"\n[Validasi Input Gagal]: {ex.Message}");
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine($"\n[Operasi Gagal]: {ex.Message}");
        }
        catch (FormatException ex)
        {
            Console.WriteLine($"\n[Format Tidak Valid]: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\n[Error]: {ex.Message}");
        }
    }
}