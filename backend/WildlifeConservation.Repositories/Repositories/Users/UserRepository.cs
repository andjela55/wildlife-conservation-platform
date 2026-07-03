using WildlifeConservation.Repositories.Data;

namespace WildlifeConservation.Repositories.Repositories.Users;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
}

public class UserRepository(WildlifeDbContext dbContext)
    : BaseRepository<User>(dbContext), IUserRepository
{
}
