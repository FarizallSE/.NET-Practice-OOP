namespace PracticeRental.Model.Vehicles;

public class Bike : Vehicle
{
    public const int FreeHelmet = 1;

    public bool IsExtraHelmet { get; private set; }
    private const decimal HelmetPrice = 10000;

    public int HelmetCount => IsExtraHelmet ? FreeHelmet + 1 : FreeHelmet;

    public Bike(string licensePlate, string brand, int year, decimal dailyRate) : base(licensePlate, brand, year, dailyRate)
    {
        IsExtraHelmet = false;
    }

    public bool SetIsExtraHelmet(bool isRent)
    {
        return IsExtraHelmet = isRent;
    }

    public override decimal CalculateExtraCost(int rentalDays)
    {
        return IsExtraHelmet ? rentalDays * HelmetPrice : 0;
    }

    public override string GetTypeVehicle()
    {
        return "Bike";
    }
}