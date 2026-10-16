using RentalVehicelFix.Interfaces;
using RentalVehicelFix.Models.Vehicles;
using RentalVehicelFix.Repositories;

namespace RentalVehicelFix.Services;

public class Rental
{
    // private Vehicle[] _fleet;
    // private RentalTransaction[] _transactions;
    // private int _transactionCount;
    private Repository<Vehicle> _vehicleRepository;
    private List<RentalTransaction> _transactions = new List<RentalTransaction>();
    private HashSet<string> _blacklistedCustomerIds = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

    // public Rental() : this(new Vehicle[100])
    // {
    // }

    public Rental() : this((Repository<Vehicle>?)null)
    {
        
    }

    public Rental(IEnumerable<Vehicle> initialFleet) : this(new Repository<Vehicle>())
    {
        if (initialFleet != null)
        {
            foreach (var v in initialFleet)
            {
                _vehicleRepository.Add(v);
            }
        }
    }

    // public Rental(Vehicle[] initialFleet, int maxTransactions = 100)
    // {
    //     _fleet = initialFleet ?? new Vehicle[0];
    //     _transactions = new RentalTransaction[maxTransactions];
    //     _transactionCount = 0;
    // }

    public Rental(Repository<Vehicle>? repository = null)
    {
        _vehicleRepository = repository ?? new Repository<Vehicle>();
    }
    
    public void AddVehicle(Vehicle vehicle)
    {
        if (vehicle == null) throw new ArgumentNullException(nameof(vehicle), "Kendaraan tidak boleh null.");
        _vehicleRepository.Add(vehicle);
    }

    public Vehicle? FindVehicle(string plateNumber) => _vehicleRepository.FindByPlate(plateNumber);
    // {
    //     if (string.IsNullOrWhiteSpace(plateNumber)) return null;
    //
    //     for (int i = 0; i < _fleet.Length; i++)
    //     {
    //         if (_fleet[i] != null && string.Equals(_fleet[i].PlateNumber, plateNumber, StringComparison.OrdinalIgnoreCase))
    //         {
    //             return _fleet[i];
    //         }
    //     }
    //     return null;
    // }

    public void Record(RentalTransaction trx)
    {
        if (trx == null) throw new ArgumentNullException(nameof(trx), "Transaksi tidak boleh bernilai null.");
        if (trx.Vehicles.Count == 0) throw new InvalidOperationException("Transaksi tidak dapat dicatat karena belum memilih kendaraan.");
        if (IsBlacklisted(trx.Customer.CustomerID)) throw new InvalidOperationException($"Pelanggan '{trx.Customer.Name}' ({trx.Customer.CustomerID}) masuk dalam daftar hitam (blacklist) dan tidak bisa menyewa lagi.");
        
        _transactions.Add(trx);
        Console.WriteLine($"Transaksi atas nama {trx.Customer?.Name} berhasil dilakukan");

        // if (_transactionCount >= _transactions.Length)
        // {
        //     Array.Resize(ref _transactions, _transactions.Length == 0 ? 10 : _transactions.Length * 2);
        // }
        //
        // _transactions[_transactionCount] = trx;
        // _transactionCount++;
        //
        // string? customerName = trx.customer?.Name;
        // if (!string.IsNullOrEmpty(customerName))
        // {
        //     Console.WriteLine($"[Rental] Transaksi atas nama '{customerName}' berhasil dicatat.");
        // }
        // else
        // {
        //     Console.WriteLine("[Rental] Transaksi berhasil dicatat.");
        // }
    }

    public int ProcessReturn(RentalTransaction trx, int lateHours)
    {
        if (trx == null) throw new ArgumentNullException(nameof(trx), "Transaksi tidak boleh bernilai null.");
        if (lateHours < 0) throw new ArgumentOutOfRangeException(nameof(lateHours), "Jam keterlambatan tidak boleh bernilai negatif.");

        int totalCost = trx.Return(lateHours);

        if (lateHours > 24)
        {
            _blacklistedCustomerIds.Add(trx.Customer.CustomerID);
            Console.WriteLine($"\nPelanggan '{trx.Customer.Name}' terlambat {lateHours} jam (> 24 jam) dan telah dimasukkan ke blacklist!");
        }
        return totalCost;
    }

    public bool IsBlacklisted(string customerId)
    {
        return !string.IsNullOrEmpty(customerId) && _blacklistedCustomerIds.Contains(customerId);
    }

    public HashSet<string> GetBlacklistedCustomerIds() => _blacklistedCustomerIds;

    public void PrintMostProfitableVehicle()
    {
        if (_transactions.Count == 0)
        {
            Console.WriteLine("\nBelum ada transaksi rental yang tercatat.");
            return;
        }
        
        var vehicleEarnings = new Dictionary<string, (Vehicle vehicle, decimal TotalRevenue)>(StringComparer.OrdinalIgnoreCase);
        foreach (var trx in _transactions)
        {
            foreach (var v in trx.Vehicles)
            {
                decimal cost = v.GetCost(trx.RentalDays);
                if (vehicleEarnings.TryGetValue(v.PlateNumber, out var data))
                {
                    vehicleEarnings[v.PlateNumber] = (data.vehicle, data.TotalRevenue + cost);
                }
                else
                {
                    vehicleEarnings[v.PlateNumber] = (v, cost);
                }
            }
        }

        var top = vehicleEarnings.Values.OrderByDescending(v => v.TotalRevenue).FirstOrDefault();
        if (top.vehicle != null)
        {
            Console.WriteLine("\n=============================================");
            Console.WriteLine("        KENDARAAN PALING MENGUNTUNGKAN       ");
            Console.WriteLine("=============================================");
            Console.WriteLine($"Plat nomor       : {top.vehicle.PlateNumber}");
            Console.WriteLine($"Merk / Tipe      : {top.vehicle.Brand} ({top.vehicle.GetType().Name})");
            Console.WriteLine($"Tahun            : {top.vehicle.Year}");
            Console.WriteLine($"Total Pendapatan : Rp {top.TotalRevenue:N0}");
            Console.WriteLine("=============================================\n");
        }
    }

    public void printAvailable()
    {
        Console.WriteLine("\n--- DAFTAR KENDARAAN TERSEDIA (AVAILABLE) ---");
        // bool anyAvailable = false;
        // for (int i = 0; i < _fleet.Length; i++)
        // {
        //     if (_fleet[i] != null && (_fleet[i].Status == "Available" || _fleet[i].Status != "Renting"))
        //     {
        //         _fleet[i].Info();
        //         anyAvailable = true;
        //     }
        // }
        // if (!anyAvailable)
        // {
        //     Console.WriteLine("Tidak ada kendaraan yang tersedia saat ini.");
        // }
        // Console.WriteLine("---------------------------------------------");
        foreach (var v in _vehicleRepository.GetAll())
        {
            if(v.Status != "Renting") v.Info();
        }
    }

    public void PrintRevenueByType()
    {
        decimal[] revenue = new decimal[3];
        string[] typeNames = { "Car", "Bus", "Bike" };

        // for (int i = 0; i < _transactionCount; i++)
        // {
        //     RentalTransaction trx = _transactions[i];
        //     if (trx == null) continue;
        //
        //     int count = 0;
        //     var getCountMethod = trx.GetType().GetMethod("GetVehicleCount");
        //     var getVehicleMethod = trx.GetType().GetMethod("GetVehicleAt");
        //
        //     if (getCountMethod != null && getVehicleMethod != null)
        //     {
        //         count = (int)getCountMethod.Invoke(trx, null)!;
        //         for (int j = 0; j < count; j++)
        //         {
        //             var v = getVehicleMethod.Invoke(trx, new object[] { j }) as Vehicle;
        //             if (v == null) continue;
        //
        //             int typeIndex = v switch
        //             {
        //                 Car => 0,
        //                 Bus => 1,
        //                 Motorcycle => 2,
        //                 _ => v.GetType().Name switch
        //                 {
        //                     "Car" => 0,
        //                     "Bus" => 1,
        //                     "Bike" or "Motorcycle" => 2,
        //                     _ => -1
        //                 }
        //             };
        //
        //             if (typeIndex >= 0)
        //             {
        //                 revenue[typeIndex] += v.GetCost(trx.RentalDays);
        //             }
        //         }
        //     }
        //     else
        //     {
        //         var vehiclesField = trx.GetType().GetField("_vehicles", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        //         var countField = trx.GetType().GetField("_count", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        //
        //         Vehicle[]? vehicles = null;
        //         if (vehiclesField != null)
        //         {
        //             vehicles = vehiclesField.GetValue(trx) as Vehicle[];
        //         }
        //         if (countField != null)
        //         {
        //             count = (int)(countField.GetValue(trx) ?? 0);
        //         }
        //
        //         if (vehicles != null)
        //         {
        //             for (int j = 0; j < count; j++)
        //             {
        //                 var v = vehicles[j];
        //                 if (v == null) continue;
        //
        //                 int typeIndex = v switch
        //                 {
        //                     Car => 0,
        //                     Bus => 1,
        //                     Motorcycle => 2,
        //                     _ => v.GetType().Name switch
        //                     {
        //                         "Car" => 0,
        //                         "Bus" => 1,
        //                         "Bike" or "Motorcycle" => 2,
        //                         _ => -1
        //                     }
        //                 };
        //
        //                 if (typeIndex >= 0)
        //                 {
        //                     revenue[typeIndex] += v.GetCost(trx.RentalDays);
        //                 }
        //             }
        //         }
        //     }
        // }

        foreach (var trx in _transactions)
        {
            if (trx == null) continue;

            foreach (var v in trx.Vehicles)
            {
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