namespace PracticeRental.Interface;

// Diimplementasikan oleh RentalItem:
// diskon 10% dari biaya sewa dasar jika durasi sewa LEBIH DARI 7 hari.
public interface IDicountable
{
    decimal CalculateFeeDiscount();
}