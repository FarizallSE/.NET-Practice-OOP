using PracticeRental.Services;

namespace PracticeRental;

class Program
{
    static void Main(string[] args)
    {
        
        RentalSystem system = new RentalSystem();
        system.SeedData();
        
        while (true)
        {
            Console.Clear();
            Console.WriteLine("===== SISTEM RENTAL KENDARAAN =====");
            Console.WriteLine("1. Lihat Katalog");
            Console.WriteLine("2. Buat Transaksi Baru");
            Console.WriteLine("3. Proses Pengembalian");
            Console.WriteLine("4. Laporan");
            Console.WriteLine("0. Keluar");
            Console.Write("Pilih: ");
            var choice = Console.ReadLine();

            switch (choice)
            {
                case "1": 
                    system.ShowCatalog(false); 
                    break;
                case "0": return;
            }
            Console.WriteLine("\nEnter untuk melanjutkan...");
            Console.ReadLine();
        }
    }
}