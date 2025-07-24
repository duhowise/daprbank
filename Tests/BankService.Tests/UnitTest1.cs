using BankService.Models;

namespace BankService.Tests;

public class AccountStateTests
{
    [Fact]
    public void AccountState_DefaultValues_ShouldBeCorrect()
    {
        // Act
        var accountState = new AccountState();

        // Assert
        Assert.Equal(string.Empty, accountState.AccountId);
        Assert.Equal(string.Empty, accountState.AccountName);
        Assert.Empty(accountState.Transactions);
        Assert.Equal(0m, accountState.Balance);
        Assert.True(accountState.CreatedAt <= DateTime.UtcNow);
        Assert.True(accountState.LastUpdated <= DateTime.UtcNow);
    }

    [Fact]
    public void AccountState_SetProperties_ShouldWorkCorrectly()
    {
        // Arrange
        var accountId = "test-123";
        var accountName = "Test Account";
        var createdAt = DateTime.UtcNow.AddDays(-1);
        var lastUpdated = DateTime.UtcNow;
        var transactions = new List<ITransactionEvent>
        {
            new MoneyDeposited(accountId, 100.50m, DateTime.UtcNow, Guid.NewGuid(), "Test deposit")
        };

        // Act
        var accountState = new AccountState
        {
            AccountId = accountId,
            AccountName = accountName,
            Transactions = transactions,
            CreatedAt = createdAt,
            LastUpdated = lastUpdated
        };

        // Assert
        Assert.Equal(accountId, accountState.AccountId);
        Assert.Equal(accountName, accountState.AccountName);
        Assert.Equal(transactions, accountState.Transactions);
        Assert.Equal(100.50m, accountState.Balance);
        Assert.Equal(createdAt, accountState.CreatedAt);
        Assert.Equal(lastUpdated, accountState.LastUpdated);
    }

    [Fact]
    public void AccountState_BalanceCalculation_WithMultipleTransactions_ShouldBeCorrect()
    {
        // Arrange
        var accountId = "test-123";
        var accountState = new AccountState
        {
            AccountId = accountId,
            AccountName = "Test Account",
            Transactions = new List<ITransactionEvent>
            {
                new MoneyDeposited(accountId, 100m, DateTime.UtcNow, Guid.NewGuid(), "Deposit 1"),
                new MoneyDeposited(accountId, 50m, DateTime.UtcNow, Guid.NewGuid(), "Deposit 2"),
                new MoneyWithdrawn(accountId, 25m, DateTime.UtcNow, Guid.NewGuid(), "Withdrawal 1"),
                new MoneyDeposited(accountId, 75m, DateTime.UtcNow, Guid.NewGuid(), "Deposit 3")
            }
        };

        // Act
        var balance = accountState.Balance;

        // Assert
        Assert.Equal(200m, balance); // 100 + 50 - 25 + 75 = 200
    }

    [Fact]
    public void AccountState_BalanceCalculation_WithOnlyDeposits_ShouldBeCorrect()
    {
        // Arrange
        var accountId = "test-123";
        var accountState = new AccountState
        {
            Transactions = new List<ITransactionEvent>
            {
                new MoneyDeposited(accountId, 100m, DateTime.UtcNow, Guid.NewGuid(), "Deposit 1"),
                new MoneyDeposited(accountId, 50m, DateTime.UtcNow, Guid.NewGuid(), "Deposit 2")
            }
        };

        // Act & Assert
        Assert.Equal(150m, accountState.Balance);
    }

    [Fact]
    public void AccountState_BalanceCalculation_WithOnlyWithdrawals_ShouldBeNegative()
    {
        // Arrange
        var accountId = "test-123";
        var accountState = new AccountState
        {
            Transactions = new List<ITransactionEvent>
            {
                new MoneyWithdrawn(accountId, 25m, DateTime.UtcNow, Guid.NewGuid(), "Withdrawal 1"),
                new MoneyWithdrawn(accountId, 15m, DateTime.UtcNow, Guid.NewGuid(), "Withdrawal 2")
            }
        };

        // Act & Assert
        Assert.Equal(-40m, accountState.Balance);
    }
}

public class TransactionEventTests
{
    [Fact]
    public void MoneyDeposited_ShouldImplementITransactionEvent()
    {
        // Arrange
        var accountId = "test-123";
        var amount = 50.25m;
        var timestamp = DateTime.UtcNow;
        var id = Guid.NewGuid();
        var description = "Test deposit";

        // Act
        var moneyDeposited = new MoneyDeposited(accountId, amount, timestamp, id, description);

        // Assert
        Assert.Equal(accountId, moneyDeposited.AccountId);
        Assert.Equal(amount, moneyDeposited.Amount);
        Assert.Equal(timestamp, moneyDeposited.Timestamp);
        Assert.Equal(id, moneyDeposited.Id);
        Assert.Equal(description, moneyDeposited.Description);
    }

    [Fact]
    public void MoneyWithdrawn_ShouldImplementITransactionEvent()
    {
        // Arrange
        var accountId = "test-123";
        var amount = 25.50m;
        var timestamp = DateTime.UtcNow;
        var id = Guid.NewGuid();
        var description = "Test withdrawal";

        // Act
        var moneyWithdrawn = new MoneyWithdrawn(accountId, amount, timestamp, id, description);

        // Assert
        Assert.Equal(accountId, moneyWithdrawn.AccountId);
        Assert.Equal(amount, moneyWithdrawn.Amount);
        Assert.Equal(timestamp, moneyWithdrawn.Timestamp);
        Assert.Equal(id, moneyWithdrawn.Id);
        Assert.Equal(description, moneyWithdrawn.Description);
    }
}

public class EventTests
{
    [Fact]
    public void AccountCreated_ShouldHaveCorrectProperties()
    {
        // Arrange
        var accountId = "test-123";
        var accountName = "Test Account";
        var createdAt = DateTime.UtcNow;

        // Act
        var accountCreated = new AccountCreated(accountId, accountName, createdAt);

        // Assert
        Assert.Equal(accountId, accountCreated.AccountId);
        Assert.Equal(accountName, accountCreated.AccountName);
        Assert.Equal(createdAt, accountCreated.CreatedAt);
    }

    [Fact]
    public void MoneyDeposited_ShouldHaveCorrectProperties()
    {
        // Arrange
        var accountId = "test-123";
        var amount = 50.25m;
        var timestamp = DateTime.UtcNow;
        var id = Guid.NewGuid();
        var description = "Test deposit";

        // Act
        var moneyDeposited = new MoneyDeposited(accountId, amount, timestamp, id, description);

        // Assert
        Assert.Equal(accountId, moneyDeposited.AccountId);
        Assert.Equal(amount, moneyDeposited.Amount);
        Assert.Equal(timestamp, moneyDeposited.Timestamp);
        Assert.Equal(id, moneyDeposited.Id);
        Assert.Equal(description, moneyDeposited.Description);
    }

    [Fact]
    public void MoneyWithdrawn_ShouldHaveCorrectProperties()
    {
        // Arrange
        var accountId = "test-123";
        var amount = 25.50m;
        var timestamp = DateTime.UtcNow;
        var id = Guid.NewGuid();
        var description = "Test withdrawal";

        // Act
        var moneyWithdrawn = new MoneyWithdrawn(accountId, amount, timestamp, id, description);

        // Assert
        Assert.Equal(accountId, moneyWithdrawn.AccountId);
        Assert.Equal(amount, moneyWithdrawn.Amount);
        Assert.Equal(timestamp, moneyWithdrawn.Timestamp);
        Assert.Equal(id, moneyWithdrawn.Id);
        Assert.Equal(description, moneyWithdrawn.Description);
    }

    [Fact]
    public void EventRecords_WithSameValues_ShouldBeEqual()
    {
        // Arrange
        var accountId = "test-123";
        var timestamp = DateTime.UtcNow;

        // Act
        var event1 = new AccountCreated(accountId, "Test", timestamp);
        var event2 = new AccountCreated(accountId, "Test", timestamp);

        // Assert
        Assert.Equal(event1, event2);
        Assert.True(event1 == event2);
    }

    [Fact]
    public void EventRecords_WithDifferentValues_ShouldNotBeEqual()
    {
        // Arrange
        var timestamp = DateTime.UtcNow;

        // Act
        var event1 = new AccountCreated("test-123", "Test", timestamp);
        var event2 = new AccountCreated("test-456", "Test", timestamp);

        // Assert
        Assert.NotEqual(event1, event2);
        Assert.False(event1 == event2);
    }
}