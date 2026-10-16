namespace RentalVehicelFix.Exceptions;

public class VehicleNotAvailableException : Exception
{
    public VehicleNotAvailableException(string message) : base(message) { }
}
