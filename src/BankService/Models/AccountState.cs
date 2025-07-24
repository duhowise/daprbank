namespace BankService.Models;

public class AccountState
{
    public string AccountId { get; set; } = string.Empty;
    public string AccountName { get; set; } = string.Empty;
    public List<ITransactionEvent> Transactions { get; set; } = new List<ITransactionEvent>();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;

    public decimal Balance => Transactions.Sum(t => t switch
    {
        MoneyDeposited => t.Amount,
        MoneyWithdrawn => -t.Amount,
        _ => 0
    });
}