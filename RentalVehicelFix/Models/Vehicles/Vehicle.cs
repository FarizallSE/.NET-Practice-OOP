using RentalVehicelFix.Exceptions;

namespace RentalVehicelFix.Models.Vehicles;

public abstract class Vehicle
{
    public int TotalVehicles { get; set; }
    public string PlateNumber { get; private set; }
    public string Brand { get; set; }
    public int Year { get; set; }

    public Vehicle(int totalVehicles, string plateNumber, string brand, int year, int dailyRate)
    {
        if (totalVehicles <= 0)
            throw new ArgumentOutOfRangeException(nameof(totalVehicles), "ID/Nomor kendaraan harus lebih dari 0.");
        if (string.IsNullOrWhiteSpace(plateNumber))
            throw new ArgumentException("Plat nomor kendaraan tidak boleh kosong.");
        if (string.IsNullOrWhiteSpace(brand))
            throw new ArgumentException("Merk kendaraan tidak boleh kosong.");
        if (year < 1900 || year > DateTime.Now.Year + 1)
            throw new ArgumentOutOfRangeException(nameof(year), $"Tahun kendaraan harus antara 1900 dan {DateTime.Now.Year + 1}.");

        TotalVehicles = totalVehicles;
        PlateNumber = plateNumber.Trim().ToUpperInvariant();
        Brand = brand.Trim();
        Year = year;
        DailyRate = dailyRate;
        Status = "Available";
    }

    private int _dailyRate;
    public int DailyRate
    {
        get => _dailyRate;
        set
        {
            if (value <= 0)
            {
                // throw new Exception("Cannot Assign Daily Rate To Be zero!");
                throw new ArgumentException("Tarif harian harus lebih besar dari 0 (tidak boleh 0 atau negatif).", nameof(value));
            }
            _dailyRate = value;
        }
    }

    public string Status { get; private set; } = "Available";

    public abstract int GetCost(int days);

    protected int GetBaseRent(int days)
    {
        if (days <= 0)
            throw new ArgumentOutOfRangeException(nameof(days), "Durasi sewa harus lebih dari 0 hari.");
        return DailyRate * days;
    }

    public virtual int CalculateFine(int lateHours)
    {
        if (lateHours < 0)
            throw new ArgumentOutOfRangeException(nameof(lateHours), "Jam keterlambatan tidak boleh bernilai negatif.");
        if (lateHours == 0) return 0;
        int hourlyFineRate = (int)Math.Ceiling(DailyRate * 0.10);
        return hourlyFineRate * lateHours;
    }

    public void Rent()
    {
        if (Status == "Renting")
            throw new VehicleNotAvailableException($"Kendaraan dengan plat {PlateNumber} sedang tidak tersedia untuk disewa.");
        Status = "Renting";
    }

    public void Return()
    {
        Status = "Available";   
    }

    public virtual void Info()
    {
        Console.WriteLine($"Brand       : {Brand}");
        Console.WriteLine($"Year        : {Year}");
        Console.WriteLine($"PlateNumber : {PlateNumber}");
        Console.WriteLine($"DailyRate   : {DailyRate}");
        Console.WriteLine($"Status      : {Status}");
    }
}