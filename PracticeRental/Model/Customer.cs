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
            throw new Exception("ID pelanggan harus lebih dari 0.");
        if (string.IsNullOrWhiteSpace(name))
            throw new Exception("Nama tidak boleh kosong.");
        if (string.IsNullOrWhiteSpace(contactNumber))
            throw new Exception("Nomor telepon tidak boleh kosong.");

        IdCustomer = idCustomer;
        Name = name;
        ContactNumber = contactNumber;
    }

    public override string ToString() => $"[{IdCustomer}] {Name} ({ContactNumber})";
}