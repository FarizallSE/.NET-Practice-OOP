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

    public void printAvailable()
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

    public void PrintRevenueByType()
    {
        decimal[] revenue = new decimal[3];
        string[] typeNames = { "Car", "Bus", "Bike" };

        foreach (var trx in _transactions)
        {
            if (trx == null) continue;

            int count = 0;
            var getCountMethod = trx.GetType().GetMethod("GetVehicleCount");
            var getVehicleMethod = trx.GetType().GetMethod("GetVehicleAt");

            if (getCountMethod != null && getVehicleMethod != null)
            {
                count = (int)getCountMethod.Invoke(trx, null)!;
                for (int j = 0; j < count; j++)
                {
                    var v = getVehicleMethod.Invoke(trx, new object[] { j }) as Vehicle;
                    if (v == null) continue;

                    int typeIndex = v switch
                    {
                        Car => 0,
                        Bus => 1,
                        Motorcycle => 2,
                        _ => v.GetType().Name switch
                        {
                            "Car" => 0,
                            "Bus" => 1,
                            "Bike" or "Motorcycle" => 2,
                            _ => -1
                        }
                    };

                    if (typeIndex >= 0)
                    {
                        revenue[typeIndex] += v.GetCost(trx.RentalDays);
                    }
                }
            }
            else
            {
                var vehiclesField = trx.GetType().GetField("_vehicles", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                var countField = trx.GetType().GetField("_count", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

                Vehicle[]? vehicles = null;
                if (vehiclesField != null)
                {
                    vehicles = vehiclesField.GetValue(trx) as Vehicle[];
                }
                if (countField != null)
                {
                    count = (int)(countField.GetValue(trx) ?? 0);
                }

                if (vehicles != null)
                {
                    for (int j = 0; j < count; j++)
                    {
                        var v = vehicles[j];
                        if (v == null) continue;

                        int typeIndex = v switch
                        {
                            Car => 0,
                            Bus => 1,
                            Motorcycle => 2,
                            _ => v.GetType().Name switch
                            {
                                "Car" => 0,
                                "Bus" => 1,
                                "Bike" or "Motorcycle" => 2,
                                _ => -1
                            }
                        };

                        if (typeIndex >= 0)
                        {
                            revenue[typeIndex] += v.GetCost(trx.RentalDays);
                        }
                    }
                }
            }
        }

        Console.WriteLine("\n--- PENDAPATAN PER JENIS KENDARAAN ---");
        decimal total = 0;
        for (int i = 0; i < revenue.Length; i++)
        {
            Console.WriteLine($"{typeNames[i],-5}: Rp {revenue[i],12:N0}");
            total += revenue[i];
        }
        Console.WriteLine("------------------------------");
        Console.WriteLine($"{"Total",-5}: Rp {total,12:N0}");
    }
}
