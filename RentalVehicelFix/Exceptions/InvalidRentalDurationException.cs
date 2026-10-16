namespace RentalVehicelFix.Exceptions;

public class InvalidRentalDurationException : Exception
{
    public InvalidRentalDurationException() { }

    public InvalidRentalDurationException(string message) : base(message) { }

    public InvalidRentalDurationException(string message, Exception inner) : base(message, inner) { }
}
