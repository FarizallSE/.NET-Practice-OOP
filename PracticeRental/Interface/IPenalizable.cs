namespace PracticeRental.Interface;

// Dipakai kembali. Diimplementasikan oleh RentalItem:
// denda 10% tarif harian per jam terlambat, maksimal 24 jam (1 hari).
public interface IPenalizable
{
    decimal CalculateFeeLate();
}