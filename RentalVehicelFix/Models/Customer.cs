namespace RentalVehicelFix.Models;

public class Customer
{
    public string CustomerID { get; private set; }
    public string Name { get; set; }
    public string PhoneNumber { get; set; }
    
    public Customer(string customerID, string name, string phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(customerID))
            throw new ArgumentException("ID Pelanggan tidak boleh kosong.");
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Nama pelanggan tidak boleh kosong.");
        if (string.IsNullOrWhiteSpace(phoneNumber))
            throw new ArgumentException("Nomor telepon tidak boleh kosong.");

        CustomerID = customerID.Trim();
        Name = name.Trim();
        PhoneNumber = phoneNumber.Trim();
    }

    public void Info()
    {
        Console.WriteLine($"ID={CustomerID} Name={Name} Number={PhoneNumber}");
    }
}