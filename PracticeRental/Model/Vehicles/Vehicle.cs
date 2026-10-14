// [LAMA] using System.Reflection.Emit;
// [REVISI] using di atas tidak terpakai, dihapus.

// [LAMA] namespace PracticeRental.Model.Vehicle;
// [REVISI] Namespace sama dengan nama class 'Vehicle' -> error ambigu
//          ("'Vehicle' is a namespace but is used like a type"). Diganti sesuai nama folder baru.
namespace PracticeRental.Model.Vehicles;

// [REVISI] Status kendaraan memakai enum (sesuai diagram StatusKendaraan), menggantikan bool IsAvailable.
public enum VehicleStatus
{
    Available, // TERSEDIA
    Rented     // DISEWA
}

// [REVISI] Method biaya sewa (CalculateCost, CalculateExtraCost) langsung didefinisikan di abstract class ini,
//          jadi interface IRentable tidak dipakai. Diskon & denda memakai interface IDicountable & IPenalizable
//          (diimplementasikan oleh RentalItem).
public abstract class Vehicle
{

    // [LAMA]
    // public string LicensePlate { get; set; }
    // public string Brand { get; set; }
    // public int Year { get; set; }
    // public int DailyRate { get; private set; }
    // public bool IsAvailable { get; private set; }
    // [REVISI] - Identitas kendaraan read-only (tidak boleh diubah setelah dibuat)
    //          - DailyRate memakai decimal (uang), sebelumnya int
    //          - IsAvailable (default false = semua kendaraan baru "tidak tersedia") diganti enum
    //            Status dengan default Available
    public string LicensePlate { get; }
    public string Brand { get; }
    public int Year { get; }
    public decimal DailyRate { get; private set; }
    public VehicleStatus Status { get; private set; } = VehicleStatus.Available;
    public bool IsAvailable => Status == VehicleStatus.Available;

    // [LAMA] protected Vehicle(string lisencePlate, string brand, int year, int dailyRate)
    protected Vehicle(string licensePlate, string brand, int year, decimal dailyRate)
    {
        if (string.IsNullOrWhiteSpace(licensePlate))
        {
            throw new ArgumentException("LicensePlate must not be empty", nameof(licensePlate));
        }

        if (dailyRate <= 0)
        {
            throw new ArgumentException("DailyRate must be greater than zero", nameof(dailyRate));
        }

        LicensePlate = licensePlate;
        Brand = brand;
        Year = year;
        DailyRate = dailyRate;
    }

    // [LAMA]
    // public void SetIsAvailable(bool status)
    // {
    //     IsAvailable = status;
    // }
    // [REVISI] Diganti SetStatus dengan enum
    public void SetStatus(VehicleStatus status)
    {
        Status = status;
    }

    // [REVISI] Tambahan sesuai diagram: setTarifHarian
    public void SetDailyRate(decimal dailyRate)
    {
        if (dailyRate <= 0)
        {
            throw new ArgumentException("DailyRate must be greater than zero", nameof(dailyRate));
        }

        DailyRate = dailyRate;
    }

    // [LAMA]
    // decimal CalculateCost(int rentalDays)
    // {
    //     return rentalDays * DailyRate;
    // }
    // [REVISI] Tanpa access modifier = private, tidak bisa dipanggil dari luar. Dijadikan public.
    public decimal CalculateCost(int rentalDays)
    {
        return rentalDays * DailyRate;
    }

    // [LAMA]
    // public virtual decimal CalculateExtraCost(int rentalDays)
    // {
    //     return CalculateCost(rentalDays)  + CalculateDriverCost(rentalDays) + CalculateHelmetCost(rentalDays);
    // }
    // [REVISI] - Biaya tambahan HANYA extra (sopir / helm). Biaya dasar (CalculateCost) tidak boleh ikut,
    //            kalau tidak, biaya sewa akan terhitung dua kali di invoice.
    //          - Dijadikan abstract (sesuai diagram: {abstract} hitungBiayaTambahan), jadi SETIAP subclass
    //            wajib menentukan biaya tambahannya sendiri:
    //              Car  -> sopir (opsional)   Bus -> sopir (wajib)   Bike -> helm extra (opsional)
    public abstract decimal CalculateExtraCost(int rentalDays);

    // [LAMA]
    // public virtual decimal CalculateDriverCost(int rentalDays)
    // {
    //     return 0;
    // }
    //
    // public virtual decimal CalculateHelmetCost(int rentalDays)
    // {
    //     return 0;
    // }
    // [REVISI] Tidak diperlukan lagi di base class. Helm tidak relevan untuk Car/Bus dan sopir tidak relevan
    //          untuk Bike. Perhitungannya dipindah ke CalculateExtraCost di masing-masing subclass.

    // [REVISI] Perhitungan denda TIDAK di Vehicle, tapi lewat interface IPenalizable
    //          yang diimplementasikan RentalItem (RentalItem.CalculateFeeLate).

    public abstract string GetTypeVehicle();


}