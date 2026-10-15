using RentalVehicelFix.Interfaces;

namespace RentalVehicelFix.Models.Vehicles;

public class Car : Vehicle, IHasDriver
{
    public bool WithDriver { get; set; }
    
    public Car(int totalVehicles, string plateNumber, string brand, int year, int dailyRate, bool withDriver) : base(totalVehicles, plateNumber, brand, year, dailyRate)
    {
        WithDriver = withDriver;
    }

    public override int GetCost(int days)
    {
        if (!WithDriver)
        {
            return DailyRate  * days;
        }
        else
        {
            return (DailyRate * days) + (DriverCost(days));
        }
    }

    bool IHasDriver.WithDriver()
    {
        return WithDriver;
    }

    public int DriverCost(int days)
    {
        return 150000 * days;
    }
}