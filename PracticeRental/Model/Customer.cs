namespace PracticeRental.Model;
public class Customer
{

    public int IdCustomer { get; }
    public string Name { get; }
    public string ContactNumber { get; }
    
    public Customer(int idCustomer, string name, string contactNumber)
    {
        // Validasi input
        if (idCustomer <= 0)
            throw new ArgumentException("ID pelanggan harus lebih dari 0", nameof(idCustomer));
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Nama tidak boleh kosong", nameof(name));
        if (string.IsNullOrWhiteSpace(contactNumber))
            throw new ArgumentException("Nomor telepon tidak boleh kosong", nameof(contactNumber));

        IdCustomer = idCustomer;
        Name = name;
        ContactNumber = contactNumber;
    }

    public override string ToString() => $"[{IdCustomer}] {Name} ({ContactNumber})";
}