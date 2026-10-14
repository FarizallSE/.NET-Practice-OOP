// [LAMA] namespace PracticeRental.Model.Vehicle;
namespace PracticeRental.Model.Vehicles;

public class Bike : Vehicle
{
    // [REVISI] Setiap sewa motor otomatis dapat 1 helm GRATIS.
    //          IsExtraHelmet = true berarti pesan 1 helm tambahan (kena biaya HelmetPrice / hari).
    public const int FreeHelmet = 1;

    public bool IsExtraHelmet { get; private set; }
    private const decimal HelmetPrice = 10000;

    // [REVISI] Info jumlah helm yang diterima pelanggan (untuk invoice)
    public int HelmetCount => IsExtraHelmet ? FreeHelmet + 1 : FreeHelmet;

    // [LAMA] public Bike(string lisencePlate, string brand, int year, int dailyRate) : base(lisencePlate, brand, year, dailyRate)
    public Bike(string licensePlate, string brand, int year, decimal dailyRate) : base(licensePlate, brand, year, dailyRate)
    {
        IsExtraHelmet = false;
    }

    public bool SetIsExtraHelmet(bool isRent)
    {
        return IsExtraHelmet = isRent;
    }

    // [LAMA]
    // public override decimal CalculateHelmetCost(int rentalDays)
    // {
    //     return IsExtraHelmet ? (rentalDays * HelmetPrice) : base.CalculateHelmetCost(rentalDays);
    // }
    // [REVISI] CalculateHelmetCost sudah tidak ada di base. Biaya extra motor = helm tambahan (opsional).
    //          Helm pertama gratis, jadi hanya dikenakan biaya jika IsExtraHelmet = true.
    public override decimal CalculateExtraCost(int rentalDays)
    {
        return IsExtraHelmet ? rentalDays * HelmetPrice : 0;
    }

    public override string GetTypeVehicle()
    {
        return "Bike";
    }
}