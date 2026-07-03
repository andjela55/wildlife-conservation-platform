namespace WildlifeConservation.Services.Transactions;

public interface ITransactionService
{
    Task<TResult> ExecuteAsync<TResult>(
        Func<Task<TResult>> operation,
        CancellationToken cancellationToken = default);
}
