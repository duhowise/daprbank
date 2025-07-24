namespace BankService.Models;

public interface ITransactionEvent
{
    string AccountId { get; }
    decimal Amount { get; }
    DateTime Timestamp { get; }
    Guid Id { get; }
    string Description { get; }
}

public record AccountCreated(string AccountId, string AccountName, DateTime CreatedAt);

public record MoneyDeposited(string AccountId, decimal Amount, DateTime Timestamp, Guid Id, string Description) : ITransactionEvent;

public record MoneyWithdrawn(string AccountId, decimal Amount, DateTime Timestamp, Guid Id, string Description) : ITransactionEvent;