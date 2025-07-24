namespace BankService.Models;

public enum TransactionType
{
    Deposit,
    Withdrawal
}

public class Transaction
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public TransactionType Type { get; set; }
    public decimal Amount { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string Description { get; set; } = string.Empty;
}

public class AccountState
{
    public string AccountId { get; set; } = string.Empty;
    public string AccountName { get; set; } = string.Empty;
    public List<Transaction> Transactions { get; set; } = new List<Transaction>();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;

    public decimal Balance => Transactions.Sum(t => t.Type == TransactionType.Deposit ? t.Amount : -t.Amount);
}