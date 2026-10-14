namespace PracticeRental.Model.Vehicle;

public class Car : Vehicle
{
    public bool UseDrive { get; private set; }
    private const decimal DriverPrice = 1500000;
    
    public Car(string lisencePlate, string brand, int year, int dailyRate) : base(lisencePlate, brand, year, dailyRate)
    {
        UseDrive = false;
    }

    public void SetIsUseDrive(bool isUseDrive)
    {
        UseDrive = isUseDrive;
    }
    
    public override decimal CalculateExtraCost(int rentalDays)
    {
        return base.CalculateExtraCost(rentalDays) + CalculateDriverCost(rentalDays); 
    }

    public override decimal CalculateDriverCost(int rentalDays)
    {
        return UseDrive ? (rentalDays * DriverPrice) : base.CalculateDriverCost(rentalDays);
    }

    public override string GetTypeVehicle()
    {
        return "Car";
    }
}