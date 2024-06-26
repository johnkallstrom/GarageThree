using GarageThree.Persistence.Data;
using GarageThree.Persistence.Parameters;
using Microsoft.IdentityModel.Tokens;
using System.Linq.Expressions;

namespace GarageThree.Persistence.Repositories;

public class MemberRepository(ApplicationDbContext context) : IRepository<Member>
{
    private readonly ApplicationDbContext _context = context;

    public async Task<Member> Create(Member entity)
    {
        var createdMember = await _context.Members.AddAsync(entity);
        await _context.SaveChangesAsync();
        return createdMember.Entity;
    }

    public async Task<Member?> Delete(int id)
    {
        var memberToDelete = await _context.Members.FirstOrDefaultAsync(m => m.Id == id);
        if (memberToDelete != null)
        {
            var deletedMember = _context.Members.Remove(memberToDelete);
            await _context.SaveChangesAsync();
            return deletedMember.Entity;
        }
        return null;
    }

    public async Task<IEnumerable<Member>> GetAll()
    {
        return await _context.Members
                             .Include(m => m.Vehicles)
                             .ToListAsync();
    }

    public async Task<Member?> GetById(int id)
    {
        var member = await _context.Members
                                   .Include(m => m.Vehicles)
                                   .FirstOrDefaultAsync(m => m.Id == id);
        return member;
    }

    public async Task<Member?> Update(Member entity)
    {
        var updatedMember = _context.Update(entity).Entity;
        await _context.SaveChangesAsync();
        return updatedMember;
    }

    public async Task<Member?> Single(QueryParams parameters)
    {
        var member = await _context.Members
                                    .Include(m => m.Vehicles)
                                    .FirstOrDefaultAsync(m => m.Id == (int?)parameters.Id ||
                                                    m.SSN == parameters.SSN);
        return member;
    }

    public async Task<IEnumerable<Member>> Filter(QueryParams parameters)
    {
        IQueryable<Member> members = _context.Members.Include(m => m.Vehicles);

        if (!string.IsNullOrWhiteSpace(parameters.SearchTerm))
        {
            members = members.Where(m =>
                m.Username.Contains(parameters.SearchTerm) ||
                m.Email.Contains(parameters.SearchTerm) ||
                m.FirstName.Contains(parameters.SearchTerm) ||
                m.LastName.Contains(parameters.SearchTerm)
            );
        };
        return await members.ToListAsync();
    }

    public bool Any()
    {
        return _context.Members.Any();
    }

    public bool Any(Func<Member, bool> predicate)
    {
        return _context.Members.Any(predicate);
    }

        public async Task<bool> AnyAsync()
    {
        return await _context.Members.AnyAsync();
    }

    public async Task<bool> AnyAsync(Expression<Func<Member, bool>> predicate)
    {
        return await _context.Members.AnyAsync(predicate);
    }
}