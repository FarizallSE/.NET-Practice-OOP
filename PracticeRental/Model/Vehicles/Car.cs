// [LAMA] namespace PracticeRental.Model.Vehicle;
namespace PracticeRental.Model.Vehicles;

public class Car : Vehicle
{
    public bool UseDrive { get; private set; }
    // [LAMA] private const decimal DriverPrice = 1500000;
    // [REVISI] Kelebihan satu angka 0. Sesuai diagram: 150.000 / hari
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
    
    // [LAMA]
    // public override decimal CalculateExtraCost(int rentalDays)
    // {
    //     return base.CalculateExtraCost(rentalDays) + CalculateDriverCost(rentalDays); 
    // }
    //
    // public override decimal CalculateDriverCost(int rentalDays)
    // {
    //     return UseDrive ? (rentalDays * DriverPrice) : base.CalculateDriverCost(rentalDays);
    // }
    // [REVISI] Versi lama: base.CalculateExtraCost sudah memanggil CalculateDriverCost, lalu ditambah
    //          CalculateDriverCost lagi -> biaya sopir terhitung DUA KALI.
    //          Sekarang CalculateExtraCost langsung berisi biaya extra mobil = sopir (jika dipakai).
    public override decimal CalculateExtraCost(int rentalDays)
    {
        return UseDrive ? rentalDays * DriverPrice : 0;
    }

    public override string GetTypeVehicle()
    {
        return "Car";
    }
}