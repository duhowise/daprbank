using Dapr.Actors;

namespace BankService.Actors;

public interface IAccountActor : IActor
{
    Task CreateAccount(string accountName);
    Task Deposit(decimal amount);
    Task Withdraw(decimal amount);
    Task<decimal> GetBalance();
}