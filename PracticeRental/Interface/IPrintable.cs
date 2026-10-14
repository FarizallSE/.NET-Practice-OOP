namespace PracticeRental.Interface;

public interface IPrintable
{
    // void PrintReceipt();
    // Nama dibuat umum (Print) karena dipakai oleh RentalItem (baris item) & RentalTransaction (invoice)
    void Print();
}