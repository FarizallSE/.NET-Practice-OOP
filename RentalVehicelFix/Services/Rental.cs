using RentalVehicelFix.Interfaces;
using RentalVehicelFix.Models.Vehicles;

namespace RentalVehicelFix.Services;

public class Rental
{
    private Vehicle[] _fleet;
    private RentalTransaction[] _transactions;
    private int _transactionCount;

    public Rental() : this(new Vehicle[100])
    {
    }

    public Rental(Vehicle[] initialFleet, int maxTransactions = 100)
    {
        _fleet = initialFleet ?? new Vehicle[0];
        _transactions = new RentalTransaction[maxTransactions];
        _transactionCount = 0;
    }

    public Vehicle? FindVehicle(string plateNumber)
    {
        if (string.IsNullOrWhiteSpace(plateNumber)) return null;

        for (int i = 0; i < _fleet.Length; i++)
        {
            if (_fleet[i] != null && string.Equals(_fleet[i].PlateNumber, plateNumber, StringComparison.OrdinalIgnoreCase))
            {
                return _fleet[i];
            }
        }
        return null;
    }
    
    public void Record(RentalTransaction trx)
    {
        if (trx == null) return;

        if (_transactionCount >= _transactions.Length)
        {
            Array.Resize(ref _transactions, _transactions.Length == 0 ? 10 : _transactions.Length * 2);
        }

        _transactions[_transactionCount] = trx;
        _transactionCount++;

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
        for (int i = 0; i < _fleet.Length; i++)
        {
            if (_fleet[i] != null && (_fleet[i].Status == "Available" || _fleet[i].Status != "Renting"))
            {
                _fleet[i].Info();
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

        for (int i = 0; i < _transactionCount; i++)
        {
            RentalTransaction trx = _transactions[i];
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