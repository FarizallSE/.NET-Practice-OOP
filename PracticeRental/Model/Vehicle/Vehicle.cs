using System.Reflection.Emit;

namespace PracticeRental.Model.Vehicle;

public abstract class Vehicle
{
    public string LicensePlate { get; set; }
    public string Brand { get; set; }
    public int Year { get; set; }
    public int DailyRate { get; private set; }
    public bool IsAvailable { get; private set; }

    protected Vehicle(string lisencePlate, string brand, int year, int dailyRate)
    {
        if (dailyRate <= 0)
        {
            throw new ArgumentException("DailyRate must be greater than zero", nameof(dailyRate));
        }

        LicensePlate = lisencePlate;
        Brand = brand;
        Year = year;
        DailyRate = dailyRate;
    }

    public void SetIsAvailable(bool status)
    {
        IsAvailable = status;
    }

    decimal CalculateCost(int rentalDays)
    {
        return rentalDays * DailyRate;
    }
    
    public virtual decimal CalculateExtraCost(int rentalDays)
    {
        return CalculateCost(rentalDays)  + CalculateDriverCost(rentalDays) + CalculateHelmetCost(rentalDays);
    }

    public virtual decimal CalculateDriverCost(int rentalDays)
    {
        return 0;
    }
    
    public virtual decimal CalculateHelmetCost(int rentalDays)
    {
        return 0;
    }

    public abstract string GetTypeVehicle();


}