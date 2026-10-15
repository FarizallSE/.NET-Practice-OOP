namespace RentalVehicelFix.Models;

public class Customer
{
    public string CustomerID { get; private set; }
    public string Name { get; set; }
    public string PhoneNumber { get; set; }
    
    public Customer(string customerID, string name, string phoneNumber)
    {
        CustomerID = customerID;
        Name = name;
        PhoneNumber = phoneNumber;
    }

    public void Info()
    {
        Console.WriteLine($"ID={CustomerID} Name={Name} Number={PhoneNumber}");
    }
}