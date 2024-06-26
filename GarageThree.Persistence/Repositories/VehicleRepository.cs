using GarageThree.Persistence.Data;
using GarageThree.Persistence.Parameters;
using System.Linq.Expressions;

namespace GarageThree.Persistence.Repositories;

public class VehicleRepository(ApplicationDbContext context) : IRepository<Vehicle>
{
    private readonly ApplicationDbContext _context = context;

    public async Task<Vehicle> Create(Vehicle entity)
    {
        var createdVehicle = await _context.Vehicles.AddAsync(entity);
        await _context.SaveChangesAsync();
        return createdVehicle.Entity;
    }

    public async Task<Vehicle?> Delete(int id)
    {
        var vehicleToDelete = await _context.Vehicles.FirstOrDefaultAsync(v => v.Id == id);
        if (vehicleToDelete != null)
        {
            var deletedVehicle = _context.Vehicles.Remove(vehicleToDelete);
            await _context.SaveChangesAsync();
            return deletedVehicle.Entity;
        }
        return null;
    }

    public async Task<bool> AnyAsync()
    {
        return await _context.Vehicles.AnyAsync();
    }

    public async Task<bool> AnyAsync(Expression<Func<Vehicle, bool>> predicate)
    {
        return await _context.Vehicles.AnyAsync(predicate);
    }

    public async Task<IEnumerable<Vehicle>> GetAll()
    {
        return await _context.Vehicles
                             .Include(v => v.VehicleType)
                             .Include(v => v.Garage)
                             .ToListAsync();
    }

    public async Task<Vehicle?> GetById(int id)
    {
        var vehicle = await _context.Vehicles
                                    .Include(v => v.Garage)
                                    .Include(v => v.Member)
                                    .FirstOrDefaultAsync(v => v.Id == id);
        return vehicle;
    }

    public async Task<Vehicle?> Update(Vehicle entity)
    {
        var updatedVehicle = _context.Vehicles.Update(entity).Entity;
        await _context.SaveChangesAsync();
        return updatedVehicle;
    }

    public async Task<IEnumerable<Vehicle>> Filter(QueryParams parameters)
    {
        var vehicles = _context.Vehicles
            .Include(v => v.VehicleType)
            .Include(v => v.Member)
            .Include(v => v.Garage) as IQueryable<Vehicle>;

        if (parameters.VehicleTypeId is not null)
        {
            return await vehicles.Where(v => v.VehicleTypeId == parameters.VehicleTypeId).ToListAsync();
        }

        if (!string.IsNullOrWhiteSpace(parameters.SearchTerm))
        {
            vehicles = vehicles.Where(v =>
            v.RegNumber.Contains(parameters.SearchTerm) ||
            v.VehicleType.Name.Contains(parameters.SearchTerm) ||
            v.Brand.Contains(parameters.SearchTerm) ||
            v.Model.Contains(parameters.SearchTerm));
        }

        int? garageId = (int?)parameters.Id;
        if (garageId.HasValue)
        {
            vehicles = vehicles.Where(v => v.GarageId == garageId.Value);
        }

        return await vehicles.ToListAsync();
    }

    public Task<Vehicle?> Single(QueryParams parameters)
    {
        throw new NotImplementedException();
    }

    public bool Any() => _context.Vehicles.Any();

    public bool Any(Func<Vehicle, bool> predicate) => _context.Vehicles.Any(predicate);
}