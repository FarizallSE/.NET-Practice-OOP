namespace PracticeRental.Model.Vehicles;

public class Car : Vehicle
{
    public bool UseDrive { get; private set; }

    private const decimal DriverPrice = 150000;
    
    // [LAMA] public Car(string lisencePlate, string brand, int year, int dailyRate) : base(lisencePlate, brand, year, dailyRate)
    public Car(string licensePlate, string brand, int year, decimal dailyRate) : base(licensePlate, brand, year, dailyRate)
    {
        UseDrive = false;
    }

    public void SetIsUseDrive(bool isUseDrive)
    {
        UseDrive = isUseDrive;
    }

    public override decimal CalculateExtraCost(int rentalDays)
    {
        return UseDrive ? rentalDays * DriverPrice : 0;
    }

    public override string GetTypeVehicle()
    {
        return "Car";
    }
}