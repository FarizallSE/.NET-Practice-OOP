// [LAMA] namespace PracticeRental.Model.Vehicle;
namespace PracticeRental.Model.Vehicles;

public class Bus : Vehicle
{
    // [LAMA] private const decimal DriverPrice = 3000000;
    // [REVISI] Kelebihan satu angka 0. Sesuai diagram: 300.000 / hari (sopir wajib untuk bus)
    private const decimal DriverPrice = 300000;
    
    // [LAMA] public Bus(string lisencePlate, string brand, int year, int dailyRate) : base(lisencePlate, brand, year, dailyRate)
    public Bus(string licensePlate, string brand, int year, decimal dailyRate) : base(licensePlate, brand, year, dailyRate)
    {

    }

    // [LAMA]
    // public override decimal CalculateDriverCost(int rentalDays)
    // {
    //     return base.CalculateDriverCost(rentalDays) + ( DriverPrice *  rentalDays);
    // }
    // [REVISI] CalculateDriverCost sudah tidak ada di base. Biaya extra bus = sopir (wajib).
    public override decimal CalculateExtraCost(int rentalDays)
    {
        return rentalDays * DriverPrice;
    }

    public override string GetTypeVehicle()
    {
        return "Bus";
    }
}