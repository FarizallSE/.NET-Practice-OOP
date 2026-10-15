namespace PracticeRental.Model.Vehicles;

public enum VehicleStatus
{
    Available, 
    Rented     
}

public abstract class Vehicle
{
    public string LicensePlate { get; }
    public string Brand { get; }
    public int Year { get; }
    public decimal DailyRate { get; private set; }
    public VehicleStatus Status { get; private set; } = VehicleStatus.Available;
    public bool IsAvailable => Status == VehicleStatus.Available;

    protected Vehicle(string licensePlate, string brand, int year, decimal dailyRate)
    {
        if (string.IsNullOrWhiteSpace(licensePlate))
        {
            throw new Exception("Plat nomor tidak boleh kosong.");
        }

        if (dailyRate <= 0)
        {
            throw new Exception("Tarif harian harus lebih dari 0.");
        }

        LicensePlate = licensePlate;
        Brand = brand;
        Year = year;
        DailyRate = dailyRate;
    }

    public void SetStatus(VehicleStatus status)
    {
        Status = status;
    }

    public void SetDailyRate(decimal dailyRate)
    {
        if (dailyRate <= 0)
        {
            throw new Exception("Tarif harian harus lebih dari 0.");
        }

        DailyRate = dailyRate;
    }

    public decimal CalculateCost(int rentalDays)
    {
        return rentalDays * DailyRate;
    }

    public abstract decimal CalculateExtraCost(int rentalDays);

    public abstract string GetTypeVehicle();


}