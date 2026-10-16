using RentalVehicelFix.Models.Vehicles;

namespace RentalVehicelFix.Repositories;

public class Repository<T> where T : Vehicle
{
    private readonly List<T> _items = new();
    private readonly Dictionary<string, T> _byPlate = new();

    public void Add(T item)
    {
        _items.Add(item);
        _byPlate[item.PlateNumber] = item;
    }

    public List<T> GetAll() => _items;

    public T FindByPlate(string plateNumber)
    {
        if (string.IsNullOrWhiteSpace(plateNumber)) return null;
        return _byPlate.TryGetValue(plateNumber, out var item) ? item : null;
    }

    public bool Contains(string plateNumber) => _byPlate.ContainsKey(plateNumber);
}
