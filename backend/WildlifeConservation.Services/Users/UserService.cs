using WildlifeConservation.Shared.Security;

namespace WildlifeConservation.Services.Users;

public class UserService(
    IUserRepository userRepository,
    IUserValidationService validationService,
    IMapper mapper,
    IAlertRepository alertRepository,
    IRangerReportRepository rangerReportRepository) : IUserService
{
    public async Task<PagedResult<User>> GetAllAsync(PaginationQuery pagination, CancellationToken cancellationToken = default)
    {
        return await userRepository.Query()
            .OrderBy(x => x.FullName)
            .ToPagedResultAsync(pagination, cancellationToken);
    }

    public async Task<User> CreateAsync(CreateUserDto dto, int actorUserId, CancellationToken cancellationToken = default)
    {
        await validationService.ValidateCreateAsync(dto, actorUserId, cancellationToken);

        var user = mapper.Map<User>(dto);
        user.PasswordSalt = PasswordHasher.CreateSalt();
        user.PasswordHash = PasswordHasher.HashPassword(dto.Password, user.PasswordSalt);

        return await userRepository.InsertAsync(user, dto.RoleIds, cancellationToken);
    }

    public async Task<User> UpdateAssignedAreaAsync(int id, UpdateUserAssignedAreaDto dto, int actorUserId, CancellationToken cancellationToken = default)
    {
        var user = await validationService.ValidateAssignedAreaUpdateAsync(id, dto, actorUserId, cancellationToken);
        mapper.Map(dto, user);

        return await userRepository.UpdateAsync(user, cancellationToken);
    }

    public async Task<User> UpdateAsync(int id, UpdateUserDto dto, int actorUserId, CancellationToken cancellationToken = default)
    {
        var user = await validationService.ValidateUpdateAsync(id, dto, actorUserId, cancellationToken);

        mapper.Map(dto, user);
        if (!string.IsNullOrWhiteSpace(dto.Password))
        {
            user.PasswordSalt = PasswordHasher.CreateSalt();
            user.PasswordHash = PasswordHasher.HashPassword(dto.Password, user.PasswordSalt);
        }
        return await userRepository.UpdateAsync(user, dto.RoleIds, cancellationToken);
    }

    public async Task DeleteAsync(int id, int actorUserId, CancellationToken cancellationToken = default)
    {
        var user = await validationService.ValidateDeleteAsync(id, actorUserId, cancellationToken);
        await using var transaction = await userRepository.StartTransactionAsync(cancellationToken);
        try
        {
            var alerts = await alertRepository.Query().Where(x => x.CreatedByUserId == id).ToListAsync(cancellationToken);
            var reports = await rangerReportRepository.Query().Where(x => x.UserId == id).ToListAsync(cancellationToken);
            await alertRepository.DeleteRangeAsync(alerts, cancellationToken);
            await rangerReportRepository.DeleteRangeAsync(reports, cancellationToken);
            await userRepository.DeleteAsync(user, cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }
}
