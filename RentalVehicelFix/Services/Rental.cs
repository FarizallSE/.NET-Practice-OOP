using System.Diagnostics;
using RentalVehicelFix.Models;
using RentalVehicelFix.Models.Vehicles;
using RentalVehicelFix.Repositories;

namespace RentalVehicelFix.Services;

public class Rental
{
    private readonly Repository<Vehicle> _vehicleRepo;
    private readonly List<RentalTransaction> _transactions = new();
    private readonly HashSet<string> _blacklist = new();

    public Rental() : this(new List<Vehicle>())
    {
    }

    public Rental(List<Vehicle> initialFleet)
    {
        _vehicleRepo = new Repository<Vehicle>();
        foreach (var v in initialFleet)
        {
            _vehicleRepo.Add(v);
        }
    }

    public bool IsBlacklisted(string customerID) => _blacklist.Contains(customerID);

    public void AddToBlacklist(string customerID) => _blacklist.Add(customerID);

    public Vehicle? FindVehicle(string plateNumber) => _vehicleRepo.FindByPlate(plateNumber);

    public void Record(RentalTransaction trx)
    {
        if (trx == null) return;

        _transactions.Add(trx);

        string? customerName = trx.customer?.Name;
        if (!string.IsNullOrEmpty(customerName))
        {
            Console.WriteLine($"[Rental] Transaksi atas nama '{customerName}' berhasil dicatat.");
        }
        else
        {
            Console.WriteLine("[Rental] Transaksi berhasil dicatat.");
        }
    }

    public void printAvailable2()
    {
        Console.WriteLine("\n--- DAFTAR KENDARAAN TERSEDIA (AVAILABLE) ---");
        bool anyAvailable = false;
        foreach (var v in _vehicleRepo.GetAll())
        {
            if (v.Status == "Available")
            {
                v.Info();
                anyAvailable = true;
            }
        }
        if (!anyAvailable)
        {
            Console.WriteLine("Tidak ada kendaraan yang tersedia saat ini.");
        }
        Console.WriteLine("---------------------------------------------");
    }

    public void printAvailable()
    {
        Console.WriteLine("\n--- DAFTAR KENDARAAN TERSEDIA (AVAILABLE) ---");
        var vehicles = _vehicleRepo.GetAvalaiable();
        foreach (var v in vehicles)
        {
            v.Info();
        }
        if (!vehicles.Any())
        {
            Console.WriteLine("Tidak ada kendaraan yang tersedia saat ini.");
        }
    }

    public void printOrderByTarif()
    {
        Console.WriteLine("\n--- ORDER BY TARIF ---");
        var vehicles = _vehicleRepo.GetAvailableOrderByTarif();

        foreach (var v in vehicles)
        {
            v.Info();
        }
        if (!vehicles.Any())
        {
            Console.WriteLine("Tidak ada kendaraan yang tersedia saat ini.");
        }
    }

    public void PrintNeverRented()
    {
        var vehicles = _vehicleRepo.GetNeverRented();
        Console.WriteLine("\n--- DAFTAR KENDARAAN BELUM PERNAH DISEWA ---");
        if (vehicles.Any())
        {
            foreach (var vehicle in vehicles)
            {
                Console.WriteLine($"Jenis          : {vehicle.GetType().Name}");
                vehicle.Info();
                Console.WriteLine("----------------------------------");
            }
        }
        else
        {
            Console.WriteLine("Tidak ada kendaraan yang belum pernah disewa!");
        }
    }

    public void PrintRevenueByType()
    {
        var revenueByType = GetRevenueType();
        Console.WriteLine("\n--- PENDAPATAN PER JENIS KENDARAAN ---");
        string[] typeNames = { "Car", "Bus", "Bike" };
        decimal total = 0;
        foreach (var name in typeNames)
        {
            decimal value = revenueByType.TryGetValue(name, out var revenue) ? revenue : 0;
            Console.WriteLine($"{name,-5}: Rp {value,12:N0}");
            total += value;
        }
        Console.WriteLine("------------------------------");
        Console.WriteLine($"{"Total",-5}: Rp {total,12:N0}");
    }

    public Dictionary<string, decimal> GetRevenueType()
    {
        var revenueByType = _transactions
            .Where(t => t != null)
            .SelectMany(t => t.Vehicles.Select(v => new { Vehicle = v, Days = t.RentalDays }))
            .GroupBy(x => x.Vehicle switch
            {
                Car => "Car",
                Bus => "Bus",
                Motorcycle => "Bike",
                _ => "Other"
            })
            .ToDictionary(g => g.Key, g => g.Sum(x => (decimal)x.Vehicle.GetCost(x.Days)));
        return revenueByType;
    }

    public async Task CheckAllVehicleServiceAsync()
    {
        var vehicles = _vehicleRepo.GetAll();
        Console.WriteLine("--- CEK SERVIS SEMUA KENDARAAN ----");
        
        Stopwatch stopwatch = Stopwatch.StartNew();
        
        //membuat task untuk semua kendaraan, jalankan bersamaan dengan whenAll
        var tasks = vehicles.Select(v => v.CheckServiceAsync()).ToArray();
        await Task.WhenAll(tasks);
        
        stopwatch.Stop();

        Console.WriteLine("--- HASIL CEK SERVIS---");

        int ok = 0, needsService = 0;
        foreach (var v in vehicles)
        {
            Console.WriteLine($"{v.PlateNumber, -12} {v.GetType().Name, -12} : {v.ServiceStatus}");
            if (v.ServiceStatus == "OK") ok++;
            else needsService++;
        }

        Console.WriteLine("-------------------------------------");
        Console.WriteLine($"Total : {vehicles.Count} | OK : {ok} | Perlu Services : {needsService}");
        Console.WriteLine($"Waktu total : {stopwatch.ElapsedMilliseconds} ms");
    }

    public async Task<Booking> CreateBookingAsync(Customer customer, List<Vehicle> vehicle, int rentalDays)
    {
        //1. Rent semua kendaraan
        foreach (var v in vehicle)
        {
            v.Rent();
        }

        var booking = new Booking(customer, vehicle, rentalDays);
        booking.Print();
        
        //Pembayaran dengan batas waktu
        bool paid = await ProcessPaymentAsync(booking, TimeSpan.FromSeconds(15));

        if (paid)
        {
            booking.MarkAsPaid();
            Console.WriteLine($"OK #{booking.BookingId} LUNAS, kendaraan siap diambil");
            return booking;
        }
        else
        {
            CancelBooking(booking);
            return null;
        }
    }

    private void CancelBooking(Booking booking)
    {
        booking.MarkAsCancelled();
        foreach (var v in booking.Vehicles)
        {
            v.Return();
        }

        Console.WriteLine($"[BATAL] Booking #{booking.BookingId} DIBATALKAN");
        Console.WriteLine("Pembayaran tidak selesai dalam batas waktu.");
        Console.WriteLine("Semua kendaraan dikembalikan ke status Available");
    }

    private async Task<bool> ProcessPaymentAsync(Booking booking, TimeSpan timeout)
    {
        var cts = new CancellationTokenSource();
        cts.CancelAfter(timeout);

        var paymentTask = Task.Run(() =>
        {
            Console.Write("Ketik 'y' lalu enter untuk membayar");
            var input = Console.ReadLine();
            return input?.Trim().ToLower() == "y";
        });
        
        var timeOutTask = Task.Delay(Timeout.Infinite, cts.Token);
        
        var completedTask = await Task.WhenAny(paymentTask, timeOutTask);

        if (completedTask == paymentTask)
        {
            cts.Cancel();
            return await paymentTask;
        }

        return false;
    }
}
