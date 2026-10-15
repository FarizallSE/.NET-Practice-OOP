namespace RentalVehicelFix.Interfaces;

public interface IHasDriver
{
    public bool WithDriver();
    public int DriverCost(int days);
}