namespace PracticeRental.Model.Vehicles;

public class Bus : Vehicle
{
    private const decimal DriverPrice = 300000;
    
    public Bus(string licensePlate, string brand, int year, decimal dailyRate) : base(licensePlate, brand, year, dailyRate)
    {

    }

    public override decimal CalculateExtraCost(int rentalDays)
    {
        return rentalDays * DriverPrice;
    }

    public override string GetTypeVehicle()
    {
        return "Bus";
    }
}