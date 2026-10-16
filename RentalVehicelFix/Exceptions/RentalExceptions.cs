namespace RentalVehicelFix.Exceptions;

public class RentalExceptions : Exception
{
    public RentalExceptions(string message) : base(message) { }
}

public class VehicleNotAvailableException : RentalExceptions
{
    public VehicleNotAvailableException(string message) : base(message) { }
}

public class InvalidRentalDurationException : RentalExceptions
{
    public InvalidRentalDurationException(string message) : base(message) { }
}