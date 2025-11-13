using AuthService.Domain.Entities;
using AuthService.Domain.Interfaces;
using AuthService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace AuthService.Infrastructure.Repositories;

public sealed class UserAddressRepository : IUserAddressRepository
{
    private readonly CommandDbContext _context;
    private readonly DbSet<UserAddress> _dbSet;

    public UserAddressRepository(CommandDbContext context)
    {
        _context = context;
        _dbSet = context.Set<UserAddress>();
    }

    public async Task<UserAddress?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(a => a.User)
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<UserAddress>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(a => a.UserId == userId)
            .OrderByDescending(a => a.IsPrimary)
            .ThenByDescending(a => a.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<UserAddress?> GetPrimaryAddressByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(a => a.UserId == userId && a.IsPrimary, cancellationToken);
    }

    public async Task<IEnumerable<UserAddress>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(a => a.User)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<UserAddress>> FindAsync(
        Expression<Func<UserAddress, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(predicate)
            .ToListAsync(cancellationToken);
    }

    public async Task<UserAddress?> FirstOrDefaultAsync(
        Expression<Func<UserAddress, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet.FirstOrDefaultAsync(predicate, cancellationToken);
    }

    public async Task<bool> AnyAsync(
        Expression<Func<UserAddress, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet.AnyAsync(predicate, cancellationToken);
    }

    public async Task AddAsync(UserAddress entity, CancellationToken cancellationToken = default)
    {
        await _dbSet.AddAsync(entity, cancellationToken);
    }

    public async Task AddRangeAsync(IEnumerable<UserAddress> entities, CancellationToken cancellationToken = default)
    {
        await _dbSet.AddRangeAsync(entities, cancellationToken);
    }

    public void Update(UserAddress entity)
    {
        _dbSet.Update(entity);
    }

    public void Remove(UserAddress entity)
    {
        _dbSet.Remove(entity);
    }

    public void RemoveRange(IEnumerable<UserAddress> entities)
    {
        _dbSet.RemoveRange(entities);
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }
}
