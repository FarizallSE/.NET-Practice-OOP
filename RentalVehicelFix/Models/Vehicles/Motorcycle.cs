namespace RentalVehicelFix.Models.Vehicles;

public class Motorcycle : Vehicle
{
    public bool ExtraHelmet { get; set; }
    public Motorcycle(int totalVehicles, string plateNumber, string brand, int year, int dailyRate, bool extraHelmet) : base(totalVehicles, plateNumber, brand, year, dailyRate)
    {
        ExtraHelmet = extraHelmet;
    }

    public override int GetCost(int days)
    {
        if (ExtraHelmet)
        {
            return (150000 * days) + (10000 * days);
        }
        return 150000 * days;
    }

    public override void Info()
    {
        base.Info();
        Console.WriteLine($"Extra Helmet : {ExtraHelmet}");
    }
}