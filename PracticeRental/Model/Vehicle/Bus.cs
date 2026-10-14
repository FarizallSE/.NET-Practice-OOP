namespace PracticeRental.Model.Vehicle;

public class Bus : Vehicle
{
    private const decimal DriverPrice = 3000000;
    
    public Bus(string lisencePlate, string brand, int year, int dailyRate) : base(lisencePlate, brand, year, dailyRate)
    {

    }

    public override decimal CalculateDriverCost(int rentalDays)
    {
        return base.CalculateDriverCost(rentalDays) + ( DriverPrice *  rentalDays);
    }

    public override string GetTypeVehicle()
    {
        return "Bus";
    }
}