using RentalVehicelFix.Models.Vehicles;

namespace RentalVehicelFix.Services;

public class VehicleService
{
    private List<Vehicle> _vehicleRepo;
    public VehicleService(List<Vehicle> vehicles)
    {
        this._vehicleRepo = vehicles;
    }
    
    public List<Vehicle> GetVehiclesIsAvalaible()
    {
        var response = _vehicleRepo.Where(v => v.Status == "Avalaible").ToList();
        return response;
    }
}