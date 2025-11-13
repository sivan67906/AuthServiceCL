using AuthService.Domain.Entities;
using AuthService.Domain.Enums;
using AuthService.Domain.Interfaces;
using AuthService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace AuthService.Infrastructure.Repositories;

public sealed class ExternalLoginRepository : IExternalLoginRepository
{
    private readonly CommandDbContext _context;
    private readonly DbSet<ExternalLogin> _dbSet;

    public ExternalLoginRepository(CommandDbContext context)
    {
        _context = context;
        _dbSet = context.Set<ExternalLogin>();
    }

    public async Task<ExternalLogin?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(e => e.User)
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    }

    public async Task<ExternalLogin?> GetByProviderAsync(
        Guid userId,
        ExternalProvider provider,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(e => e.UserId == userId && e.Provider == provider, cancellationToken);
    }

    public async Task<ExternalLogin?> GetByProviderKeyAsync(
        ExternalProvider provider,
        string providerKey,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(e => e.User)
            .FirstOrDefaultAsync(e => e.Provider == provider && e.ProviderKey == providerKey, cancellationToken);
    }

    public async Task<IEnumerable<ExternalLogin>> GetByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(e => e.UserId == userId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<ExternalLogin>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(e => e.User)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<ExternalLogin>> FindAsync(
        Expression<Func<ExternalLogin, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(predicate)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> AnyAsync(
        Expression<Func<ExternalLogin, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet.AnyAsync(predicate, cancellationToken);
    }

    public async Task AddAsync(ExternalLogin entity, CancellationToken cancellationToken = default)
    {
        await _dbSet.AddAsync(entity, cancellationToken);
    }

    public void Update(ExternalLogin entity)
    {
        _dbSet.Update(entity);
    }

    public void Remove(ExternalLogin entity)
    {
        _dbSet.Remove(entity);
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }
}
