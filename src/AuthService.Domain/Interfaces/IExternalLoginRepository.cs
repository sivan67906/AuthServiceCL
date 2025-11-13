using AuthService.Domain.Entities;
using AuthService.Domain.Enums;
using System.Linq.Expressions;

namespace AuthService.Domain.Interfaces;

public interface IExternalLoginRepository
{
    Task<ExternalLogin?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ExternalLogin?> GetByProviderAsync(Guid userId, ExternalProvider provider, CancellationToken cancellationToken = default);
    Task<ExternalLogin?> GetByProviderKeyAsync(ExternalProvider provider, string providerKey, CancellationToken cancellationToken = default);
    Task<IEnumerable<ExternalLogin>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<ExternalLogin>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<ExternalLogin>> FindAsync(Expression<Func<ExternalLogin, bool>> predicate, CancellationToken cancellationToken = default);
    Task<bool> AnyAsync(Expression<Func<ExternalLogin, bool>> predicate, CancellationToken cancellationToken = default);
    Task AddAsync(ExternalLogin entity, CancellationToken cancellationToken = default);
    void Update(ExternalLogin entity);
    void Remove(ExternalLogin entity);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
