using RentalVehicelFix.Models.Vehicles;

namespace RentalVehicelFix.Repositories;

public class Repository<T> where T : Vehicle
{
    private List<T> _items = new();
    private Dictionary<string, T> _byPlate = new Dictionary<string, T>(StringComparer.OrdinalIgnoreCase);

    public void Add(T item)
    {
        if (item == null) throw new ArgumentNullException(nameof(item));
        _items.Add(item);
        _byPlate[item.PlateNumber] = item;
    }

    public T? FindByPlate(string plateNumber)
    {
        if (string.IsNullOrWhiteSpace(plateNumber)) return null;
        _byPlate.TryGetValue(plateNumber, out var item);
        return item;
    }

    public List<T> GetAll() => _items;

    public bool Remove(T item)
    {
        if (item == null) return false;
        _byPlate.Remove(item.PlateNumber);
        return _items.Remove(item);
    }
}