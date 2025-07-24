using Dapr.Actors.Runtime;
using Dapr.Client;
using BankService.Models;

namespace BankService.Actors;

public class AccountActor : Actor, IAccountActor
{
    private const string AccountStateKey = "accountState";
    private const string AccountEventsTopic = "account-events";
    private readonly DaprClient _daprClient;

    public AccountActor(ActorHost host, DaprClient daprClient) : base(host)
    {
        _daprClient = daprClient;
    }

    public async Task CreateAccount(string accountName)
    {
        var existingState = await StateManager.TryGetStateAsync<AccountState>(AccountStateKey);
        if (existingState.HasValue)
        {
            throw new InvalidOperationException($"Account {Id.GetId()} already exists");
        }

        var accountState = new AccountState
        {
            AccountId = Id.GetId(),
            AccountName = accountName,
            Balance = 0m,
            CreatedAt = DateTime.UtcNow,
            LastUpdated = DateTime.UtcNow
        };

        await StateManager.SetStateAsync(AccountStateKey, accountState);

        var accountCreatedEvent = new AccountCreated(accountState.AccountId, accountState.AccountName, accountState.CreatedAt);
        await PublishEvent(accountCreatedEvent);

        Logger.LogInformation("Account {AccountId} created with name {AccountName}", accountState.AccountId, accountState.AccountName);
    }

    public async Task Deposit(decimal amount)
    {
        if (amount <= 0)
        {
            throw new ArgumentException("Deposit amount must be positive", nameof(amount));
        }

        var accountState = await GetAccountState();
        accountState.Balance += amount;
        accountState.LastUpdated = DateTime.UtcNow;

        await StateManager.SetStateAsync(AccountStateKey, accountState);

        var depositEvent = new MoneyDeposited(accountState.AccountId, amount, accountState.Balance, accountState.LastUpdated);
        await PublishEvent(depositEvent);

        Logger.LogInformation("Deposited {Amount} to account {AccountId}. New balance: {Balance}", amount, accountState.AccountId, accountState.Balance);
    }

    public async Task Withdraw(decimal amount)
    {
        if (amount <= 0)
        {
            throw new ArgumentException("Withdrawal amount must be positive", nameof(amount));
        }

        var accountState = await GetAccountState();
        
        if (accountState.Balance < amount)
        {
            throw new InvalidOperationException($"Insufficient funds. Current balance: {accountState.Balance}, Requested withdrawal: {amount}");
        }

        accountState.Balance -= amount;
        accountState.LastUpdated = DateTime.UtcNow;

        await StateManager.SetStateAsync(AccountStateKey, accountState);

        var withdrawEvent = new MoneyWithdrawn(accountState.AccountId, amount, accountState.Balance, accountState.LastUpdated);
        await PublishEvent(withdrawEvent);

        Logger.LogInformation("Withdrew {Amount} from account {AccountId}. New balance: {Balance}", amount, accountState.AccountId, accountState.Balance);
    }

    public async Task<decimal> GetBalance()
    {
        var accountState = await GetAccountState();
        return accountState.Balance;
    }

    private async Task<AccountState> GetAccountState()
    {
        var stateResult = await StateManager.TryGetStateAsync<AccountState>(AccountStateKey);
        if (!stateResult.HasValue)
        {
            throw new InvalidOperationException($"Account {Id.GetId()} not found. Please create the account first.");
        }
        return stateResult.Value;
    }

    private async Task PublishEvent<T>(T eventData)
    {
        try
        {
            await _daprClient.PublishEventAsync("pubsub", AccountEventsTopic, eventData);
            Logger.LogInformation("Published event {EventType} for account {AccountId}", typeof(T).Name, Id.GetId());
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to publish event {EventType} for account {AccountId}", typeof(T).Name, Id.GetId());
            // Don't throw here to avoid failing the business operation due to event publishing issues
        }
    }
}