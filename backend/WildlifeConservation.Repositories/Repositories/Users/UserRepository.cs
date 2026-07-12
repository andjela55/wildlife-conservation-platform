using Microsoft.EntityFrameworkCore;
using WildlifeConservation.Repositories.Data;

namespace WildlifeConservation.Repositories.Repositories.Users;

public interface IUserRepository
{
    IQueryable<User> Query();
    Task<User?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<User> InsertAsync(User entity, CancellationToken cancellationToken = default);
    Task<User> UpdateAsync(User entity, CancellationToken cancellationToken = default);
}

public class UserRepository(WildlifeDbContext dbContext)
    : BaseRepository<User>(dbContext), IUserRepository
{
    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await Query()
            .FirstOrDefaultAsync(x => x.Email == email, cancellationToken);
    }
}
