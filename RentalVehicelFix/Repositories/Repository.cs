namespace RentalVehicelFix.Repositories;

public class Repository<TEntity, TKey> where TKey : notnull
{
    private readonly List<TEntity> _items = new();
    private readonly Dictionary<TKey, TEntity> _lookup;
    private readonly Func<TEntity, TKey> _keySelector;

    public Repository(Func<TEntity, TKey> keySelector, IEqualityComparer<TKey>? comparer = null)
    {
        _keySelector = keySelector ?? throw new ArgumentNullException(nameof(keySelector));
        _lookup = new Dictionary<TKey, TEntity>(comparer ?? EqualityComparer<TKey>.Default);
    }

    public void Add(TEntity item)
    {
        if (item == null) throw new ArgumentNullException(nameof(item), "Item tidak boleh null.");

        TKey key = _keySelector(item);
        if (key == null || (key is string s && string.IsNullOrWhiteSpace(s)))
            throw new ArgumentException("Kunci/ID entitas tidak boleh kosong.");

        if (_lookup.ContainsKey(key))
            throw new InvalidOperationException($"Data dengan ID '{key}' sudah terdaftar.");

        _items.Add(item);
        _lookup[key] = item;
    }

    public TEntity? FindById(TKey id)
    {
        if (id == null || (id is string s && string.IsNullOrWhiteSpace(s))) return default;
        _lookup.TryGetValue(id, out var item);
        return item;
    }

    public bool Exists(TKey id) => FindById(id) != null;

    public List<TEntity> GetAll() => _items;

    public bool Remove(TEntity item)
    {
        if (item == null) return false;
        TKey key = _keySelector(item);
        _lookup.Remove(key);
        return _items.Remove(item);
    }

    public bool RemoveById(TKey id)
    {
        var item = FindById(id);
        return item != null && Remove(item);
    }
}