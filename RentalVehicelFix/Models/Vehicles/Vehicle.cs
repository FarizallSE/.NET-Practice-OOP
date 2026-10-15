namespace RentalVehicelFix.Models.Vehicles;

public abstract class Vehicle
{
    public int TotalVehicles { get; set; }
    public string PlateNumber { get; private set; }
    public string Brand { get; set; }
    public int Year { get; set; }

    public Vehicle(int totalVehicles, string plateNumber, string brand, int year, int dailyRate)
    {
        TotalVehicles =  totalVehicles;
        PlateNumber = plateNumber;
        Brand = brand;
        Year = year;
        DailyRate = dailyRate;
    }

    public int DailyRate
    {
        get;
        set
        {
            int dailyRateField = value <= 0 ? field : throw new Exception("Cannot Assign Daily Rate To Be zero!");
        }
    }

    public string Status { get; private set; }

    public abstract int GetCost(int days);

    protected int GetBaseRent(int days)
    {
        return DailyRate * days;
    }

    public int CalculateFine(int lateHours)
    {
        return 0;
    }

    public void Rent()
    {
        Status = "Renting";
    }

    public void Return()
    {
        Status = "Returning";   
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