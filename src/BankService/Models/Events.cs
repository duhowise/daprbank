namespace BankService.Models;

public record AccountCreated(string AccountId, string AccountName, DateTime CreatedAt);

public record MoneyDeposited(string AccountId, decimal Amount, decimal NewBalance, DateTime Timestamp);

public record MoneyWithdrawn(string AccountId, decimal Amount, decimal NewBalance, DateTime Timestamp);