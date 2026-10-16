namespace RentalVehicelFix.Exceptions;

public class VehicleNotAvailableException : Exception
{
    public VehicleNotAvailableException() { }

    public VehicleNotAvailableException(string message) : base(message) { }

    public VehicleNotAvailableException(string message, Exception inner) : base(message, inner) { }
}
