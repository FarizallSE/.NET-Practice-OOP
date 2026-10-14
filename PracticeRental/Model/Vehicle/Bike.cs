namespace PracticeRental.Model.Vehicle;

public class Bike : Vehicle
{
    public bool IsExtraHelmet { get; private set; }
    private const decimal HelmetPrice = 10000;
    public Bike(string lisencePlate, string brand, int year, int dailyRate) : base(lisencePlate, brand, year, dailyRate)
    {
        IsExtraHelmet = false;
    }

    public bool SetIsExtraHelmet(bool isRent)
    {
        return IsExtraHelmet = isRent;
    }

    public override decimal CalculateHelmetCost(int rentalDays)
    {
        return IsExtraHelmet ? (rentalDays * HelmetPrice) : base.CalculateHelmetCost(rentalDays);
    }

    public override string GetTypeVehicle()
    {
        return "Bike";
    }
}