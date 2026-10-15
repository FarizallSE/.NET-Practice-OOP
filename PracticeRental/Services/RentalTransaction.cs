using PracticeRental.Interface;
using PracticeRental.Model;
using PracticeRental.Model.Vehicles;

namespace PracticeRental.Services;

public class RentalTransaction : IPrintable
{
    private const int MaxItem = 20;

    public int IdTransaction { get; }
    public Customer Customer { get; }
    public bool IsCompleted { get; private set; }
    public int ItemCount { get; private set; }

    private readonly RentalItem[] _items = new RentalItem[MaxItem];

    public RentalTransaction(int idTransaction, Customer customer)
    {
        IdTransaction = idTransaction;
        Customer = customer ?? throw new Exception("Pelanggan tidak boleh null.");
    }

    public bool AddItem(RentalItem item)
    {
        if (IsCompleted || ItemCount >= MaxItem) return false;
        if (!item.TargetVehicle.IsAvailable) return false;

        item.TargetVehicle.SetStatus(VehicleStatus.Rented);
        _items[ItemCount] = item;
        ItemCount++;
        return true;
    }

    public bool ProcessReturn(string licensePlate, int hoursLate)
    {
        for (int i = 0; i < ItemCount; i++)
        {
            RentalItem item = _items[i];
            if (!item.IsReturned &&
                item.TargetVehicle.LicensePlate.Equals(licensePlate, StringComparison.OrdinalIgnoreCase))
            {
                item.SetHoursLate(hoursLate);
                item.TargetVehicle.SetStatus(VehicleStatus.Available);
                IsCompleted = AllReturned();
                return true;
            }
        }
        return false;
    }

    private bool AllReturned()
    {
        for (int i = 0; i < ItemCount; i++)
        {
            if (!_items[i].IsReturned) return false;
        }
        return ItemCount > 0;
    }

    public RentalItem[] GetItems()
    {
        RentalItem[] result = new RentalItem[ItemCount];
        Array.Copy(_items, result, ItemCount);
        return result;
    }

    public decimal CalculateTotalCost()
    {
        decimal total = 0;
        for (int i = 0; i < ItemCount; i++)
        {
            total += _items[i].CalculateTotal();
        }
        return total;
    }

    // Invoice
    public void Print()
    {
        Console.WriteLine("====================================================================");
        Console.WriteLine($" INVOICE #{IdTransaction:D4}                     Status: {(IsCompleted ? "SELESAI" : "BERJALAN")}");
        Console.WriteLine($" Pelanggan : {Customer}");
        Console.WriteLine("--------------------------------------------------------------------");
        for (int i = 0; i < ItemCount; i++)
        {
            _items[i].Print();
        }
        Console.WriteLine("--------------------------------------------------------------------");
        Console.WriteLine($" TOTAL TAGIHAN                  : Rp {CalculateTotalCost(),12:N0}");
        Console.WriteLine("====================================================================");
    }
}
