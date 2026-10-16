namespace RentalVehicelFix.Exceptions;

public class RentalExceptions : Exception
{
    public RentalExceptions(string message) : base(message) { }
}

public class VehicleNotAvailableException : RentalExceptions
{
    public VehicleNotAvailableException(string message) : base("Kendaraan tidak tersedia") { }
}

public class InvalidRentalDurationException : RentalExceptions
{
    public InvalidRentalDurationException(string message) : base("Masa peminjaman tidak valid") { }
}