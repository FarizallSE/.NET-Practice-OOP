using RentalVehicelFix.Interfaces;

namespace RentalVehicelFix.Models.Vehicles;

public class Bus : Vehicle, IHasDriver
{
    public int SeatCapacity { get; set; }
    public bool WithDriver { get; set; }
    public Bus(int totalVehicles, string plateNumber, string brand, int year, int dailyRate, int seatCapacity, bool withDriver) : base(totalVehicles, plateNumber, brand, year, dailyRate)
    {
        SeatCapacity = seatCapacity;
        WithDriver = withDriver;
    }

    public override int GetCost(int days)
    {
        return (base.DailyRate * days) + DriverCost(days);
    }

    bool IHasDriver.WithDriver()
    {
       return WithDriver = true;
    }

    public int DriverCost(int days)
    {
        return 300000 * days;
    }

    public override void Info()
    {
        base.Info();
    }
}