using AuthService.Domain.Entities;
using System.Linq.Expressions;

namespace AuthService.Domain.Interfaces;

public interface IUserAddressRepository
{
    Task<UserAddress?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<UserAddress>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<UserAddress?> GetPrimaryAddressByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<UserAddress>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<UserAddress>> FindAsync(Expression<Func<UserAddress, bool>> predicate, CancellationToken cancellationToken = default);
    Task<UserAddress?> FirstOrDefaultAsync(Expression<Func<UserAddress, bool>> predicate, CancellationToken cancellationToken = default);
    Task<bool> AnyAsync(Expression<Func<UserAddress, bool>> predicate, CancellationToken cancellationToken = default);
    Task AddAsync(UserAddress entity, CancellationToken cancellationToken = default);
    Task AddRangeAsync(IEnumerable<UserAddress> entities, CancellationToken cancellationToken = default);
    void Update(UserAddress entity);
    void Remove(UserAddress entity);
    void RemoveRange(IEnumerable<UserAddress> entities);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
