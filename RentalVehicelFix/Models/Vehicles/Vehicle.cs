namespace RentalVehicelFix.Models.Vehicles;

public abstract class Vehicle
{
    public int TotalVehicles { get; set; }
    public string PlateNumber { get; private set; }
    public string Brand { get; set; }
    public int Year { get; set; }

    public Vehicle(int totalVehicles, string plateNumber, string brand, int year, int dailyRate)
    {
        TotalVehicles = totalVehicles;
        PlateNumber = plateNumber;
        Brand = brand;
        Year = year;
        DailyRate = dailyRate;
        Status = "Available";
        RentalCount = 0;
    }

    private int _dailyRate;
    public int DailyRate
    {
        get => _dailyRate;
        set
        {
            if (value <= 0)
            {
                throw new ArgumentException("Tarif harian harus lebih dari 0.", nameof(value));
            }
            _dailyRate = value;
        }
    }

    public string Status { get; private set; } = "Available";
    public int RentalCount { get; private set; }
    public bool HasEverBeenRented => RentalCount > 0;

    public string ServiceStatus { get; private set; } =  "Not Checked"; 

    public abstract int GetCost(int days);

    protected int GetBaseRent(int days)
    {
        return DailyRate * days;
    }

    public virtual int CalculateFine(int lateHours)
    {
        if (lateHours <= 0) return 0;
        int hourlyFineRate = (int)Math.Ceiling(DailyRate * 0.10);
        return hourlyFineRate * lateHours;
    }

    public void Rent()
    {
        Status = "Renting";
        RentalCount++;
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

    public async Task CheckServiceAsync()
    {
        int delay = 500 + (PlateNumber.Sum(c => c) % 1500);

        Console.WriteLine($"[Servis] Mengecek {Brand} ({PlateNumber})");
        await Task.Delay(delay);
        
        ServiceStatus = Random.Shared.Next(0, 2) == 0 ? "OK" : "Needs Service";

        Console.WriteLine($"[Servis] {Brand} ({PlateNumber} selesai dalam {delay} ms - Status : {ServiceStatus}) ");
    }
}