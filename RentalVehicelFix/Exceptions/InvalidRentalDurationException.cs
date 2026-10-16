namespace RentalVehicelFix.Exceptions;

public class InvalidRentalDurationException : Exception
{
    public InvalidRentalDurationException(string message) : base(message) { }
}
