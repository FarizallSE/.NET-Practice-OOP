using RentalVehicelFix.Interfaces;

namespace RentalVehicelFix.Models.Vehicles;

public class Bus : Vehicle, IHasDriver
{
    public int SeatCapacity { get; set; }
    public bool WithDriver { get; set; }
    public Bus(int totalVehicles, string plateNumber, string brand, int year, int dailyRate, int seatCapacity, bool withDriver) : base(totalVehicles, plateNumber, brand, year, dailyRate)
    {
        if (seatCapacity <= 0)
            throw new ArgumentOutOfRangeException(nameof(seatCapacity), "Kapasitas kursi bus harus lebih dari 0.");
        SeatCapacity = seatCapacity;
        WithDriver = withDriver;
    }

    public override int GetCost(int days)
    {
        if (days <= 0)
            throw new ArgumentOutOfRangeException(nameof(days), "Durasi hari sewa harus lebih dari 0.");
        return (base.DailyRate * days) + DriverCost(days);
    }

    bool IHasDriver.WithDriver()
    {
       return WithDriver;
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