using PracticeRental.Interface;
using PracticeRental.Model.Vehicles;


namespace PracticeRental.Model;

// [REVISI] Mengimplementasikan 3 interface:
//          IPrintable   -> Print()               : cetak baris item di invoice
//          IDicountable -> CalculateFeeDiscount(): diskon 10% jika sewa > 7 hari
//          IPenalizable -> CalculateFeeLate()    : denda 10% tarif harian / jam, maks 24 jam
public class RentalItem : IPrintable, IDicountable, IPenalizable
{
    // Aturan diskon
    public const int MinDaysForDiscount = 7;
    public const decimal DiscountRate = 0.10m;

    // Aturan denda 
    public const decimal PenaltyRatePerHour = 0.10m;
    public const int MaxPenaltyHours = 24;

    public Vehicle TargetVehicle { get; }
    public int DurationDays { get; }
    public int HoursLate { get; private set; }
    public bool IsReturned { get; private set; }


    private readonly decimal _baseSubtotal;
    private readonly decimal _extraCost;
    private readonly decimal _dailyRate;   // tarif saat disewa (snapshot) untuk hitung denda
    private readonly string _extraInfo;

    public RentalItem(Vehicle vehicle, int durationDays)
    {
        TargetVehicle = vehicle ?? throw new ArgumentNullException(nameof(vehicle));
        if (durationDays <= 0)
            throw new ArgumentException("Durasi sewa minimal 1 hari", nameof(durationDays));

        DurationDays = durationDays;
        HoursLate = 0;

        _baseSubtotal = vehicle.CalculateCost(durationDays);
        _extraCost = vehicle.CalculateExtraCost(durationDays);
        _dailyRate = vehicle.DailyRate;
        _extraInfo = vehicle switch
        {
            Car car   => car.UseDrive ? "Pakai sopir" : "Tanpa sopir",
            Bus       => "Sopir (wajib)",
            Bike bike => bike.IsExtraHelmet ? $"{bike.HelmetCount} helm (1 extra)" : "1 helm (gratis)",
            _         => "-"
        };
    }
    
    public void SetHoursLate(int hoursLate)
    {
        if (hoursLate < 0)
            throw new ArgumentException("Jam terlambat tidak boleh negatif", nameof(hoursLate));

        HoursLate = hoursLate;
        IsReturned = true;
    }

    public decimal CalculateSubTotal() => _baseSubtotal;   // biaya sewa dasar
    public decimal CalculateExtraCost() => _extraCost;     // sopir / helm extra

    public decimal CalculateFeeDiscount()
    {
        return DurationDays > MinDaysForDiscount ? _baseSubtotal * DiscountRate : 0;
    }

    public decimal CalculateFeeLate()
    {
        if (HoursLate <= 0) return 0;

        int chargedHours = Math.Min(HoursLate, MaxPenaltyHours);
        return chargedHours * _dailyRate * PenaltyRatePerHour;
    }

    public decimal CalculateTotal()
    {
        return CalculateSubTotal() + CalculateExtraCost() - CalculateFeeDiscount() + CalculateFeeLate();
    }

    public void Print()
    {
        Console.WriteLine($"  {TargetVehicle.GetTypeVehicle()} | {TargetVehicle.LicensePlate} | {TargetVehicle.Brand} ({TargetVehicle.Year})"
                          + $" | {DurationDays} hari | {_extraInfo} | {(IsReturned ? "Sudah kembali" : "Belum kembali")}");
        Console.WriteLine($"    Biaya sewa ({DurationDays} x {_dailyRate:N0}) : Rp {CalculateSubTotal(),12:N0}");
        Console.WriteLine($"    Biaya tambahan             : Rp {CalculateExtraCost(),12:N0}");
        Console.WriteLine($"    Diskon                     : Rp {-CalculateFeeDiscount(),12:N0}");
        Console.WriteLine($"    Denda ({HoursLate} jam)              : Rp {CalculateFeeLate(),12:N0}");
        Console.WriteLine($"    Subtotal item              : Rp {CalculateTotal(),12:N0}");
    }
}