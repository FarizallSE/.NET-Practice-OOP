using PracticeRental.Model.Vehicles;

namespace PracticeRental.Model;

public class RentalSystem
{
    // [REVISI] Kapasitas array berukuran tetap (sesuai diagram)
    public const int MaxFleet = 100;
    public const int MaxTransaction = 500;
    public const int MaxCustomer = 500;

    // [LAMA]
    // private vehicle[] catalog;
    // private int catalogCount;
    // private int transactionCount;
    // [REVISI] - 'vehicle' huruf kecil tidak ada (error compile) -> Vehicle
    //          - Array belum diinisialisasi (null) -> langsung dibuat dengan kapasitas tetap
    //          - Array transaksi belum ada -> ditambahkan
    //          - Ditambah array pelanggan agar pelanggan lama bisa dipakai lagi lewat ID
    private readonly Vehicle[] catalog = new Vehicle[MaxFleet];
    private int catalogCount;

    private readonly RentalTransaction[] transactions = new RentalTransaction[MaxTransaction];
    private int transactionCount;

    private readonly Customer[] customers = new Customer[MaxCustomer];
    private int customerCount;

    // ===================== KENDARAAN =====================
    public bool AddVehicle(Vehicle vehicle)
    {
        if (catalogCount >= MaxFleet || FindVehicle(vehicle.LicensePlate) != null) return false;
        catalog[catalogCount] = vehicle;
        catalogCount++;
        return true;
    }

    public Vehicle? FindVehicle(string licensePlate)
    {
        for (int i = 0; i < catalogCount; i++)
        {
            if (catalog[i].LicensePlate.Equals(licensePlate, StringComparison.OrdinalIgnoreCase))
                return catalog[i];
        }
        return null;
    }

    public Vehicle[] GetAvailableVehicles()
    {
        int count = 0;
        for (int i = 0; i < catalogCount; i++)
        {
            if (catalog[i].IsAvailable) count++;
        }

        Vehicle[] result = new Vehicle[count];
        int index = 0;
        for (int i = 0; i < catalogCount; i++)
        {
            if (catalog[i].IsAvailable) result[index++] = catalog[i];
        }
        return result;
    }

    public Vehicle[] GetAllVehicles()
    {
        Vehicle[] result = new Vehicle[catalogCount];
        Array.Copy(catalog, result, catalogCount);
        return result;
    }

    // ===================== PELANGGAN =====================
    public Customer? FindCustomer(int idCustomer)
    {
        for (int i = 0; i < customerCount; i++)
        {
            if (customers[i].IdCustomer == idCustomer) return customers[i];
        }
        return null;
    }

    public bool AddCustomer(Customer customer)
    {
        if (customerCount >= MaxCustomer || FindCustomer(customer.IdCustomer) != null) return false;
        customers[customerCount] = customer;
        customerCount++;
        return true;
    }

    // ===================== TRANSAKSI =====================
    public bool CanCreateTransaction => transactionCount < MaxTransaction;
    public int TransactionCount => transactionCount;
    public int CatalogCount => catalogCount;
    public int CustomerCount => customerCount;

    // ID transaksi dibuat otomatis berurutan
    public RentalTransaction? CreateTransaction(Customer customer)
    {
        if (!CanCreateTransaction) return null;
        RentalTransaction transaction = new RentalTransaction(transactionCount + 1, customer);
        transactions[transactionCount] = transaction;
        transactionCount++;
        return transaction;
    }

    public RentalTransaction? FindTransaction(int idTransaction)
    {
        for (int i = 0; i < transactionCount; i++)
        {
            if (transactions[i].IdTransaction == idTransaction) return transactions[i];
        }
        return null;
    }

    public RentalTransaction[] GetAllTransactions()
    {
        RentalTransaction[] result = new RentalTransaction[transactionCount];
        Array.Copy(transactions, result, transactionCount);
        return result;
    }

    public void PrintAllTransactions()
    {
        if (transactionCount == 0)
        {
            Console.WriteLine("Belum ada transaksi.");
            return;
        }
        for (int i = 0; i < transactionCount; i++)
        {
            transactions[i].Print();
        }
    }

    // ===================== PERHITUNGAN PENDAPATAN (BUSINESS LOGIC) =====================
    // Method kalkulasi data pendapatan untuk konsumsi UI / pelaporan
    public decimal CalculateTotalRevenue()
    {
        decimal total = 0;
        for (int i = 0; i < transactionCount; i++)
        {
            if (!transactions[i].IsCompleted) continue;
            total += transactions[i].CalculateTotalCost();
        }
        return total;
    }

    public int GetCompletedTransactionCount()
    {
        int completed = 0;
        for (int i = 0; i < transactionCount; i++)
        {
            if (transactions[i].IsCompleted) completed++;
        }
        return completed;
    }
}