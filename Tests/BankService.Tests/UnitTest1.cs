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
        var transactions = new List<Transaction>
        {
            new Transaction { Type = TransactionType.Deposit, Amount = 100.50m }
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
        var accountState = new AccountState
        {
            AccountId = "test-123",
            AccountName = "Test Account",
            Transactions = new List<Transaction>
            {
                new Transaction { Type = TransactionType.Deposit, Amount = 100m },
                new Transaction { Type = TransactionType.Deposit, Amount = 50m },
                new Transaction { Type = TransactionType.Withdrawal, Amount = 25m },
                new Transaction { Type = TransactionType.Deposit, Amount = 75m }
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
        var accountState = new AccountState
        {
            Transactions = new List<Transaction>
            {
                new Transaction { Type = TransactionType.Deposit, Amount = 100m },
                new Transaction { Type = TransactionType.Deposit, Amount = 50m }
            }
        };

        // Act & Assert
        Assert.Equal(150m, accountState.Balance);
    }

    [Fact]
    public void AccountState_BalanceCalculation_WithOnlyWithdrawals_ShouldBeNegative()
    {
        // Arrange
        var accountState = new AccountState
        {
            Transactions = new List<Transaction>
            {
                new Transaction { Type = TransactionType.Withdrawal, Amount = 25m },
                new Transaction { Type = TransactionType.Withdrawal, Amount = 15m }
            }
        };

        // Act & Assert
        Assert.Equal(-40m, accountState.Balance);
    }
}

public class TransactionTests
{
    [Fact]
    public void Transaction_DefaultValues_ShouldBeCorrect()
    {
        // Act
        var transaction = new Transaction();

        // Assert
        Assert.NotEqual(Guid.Empty, transaction.Id);
        Assert.Equal(TransactionType.Deposit, transaction.Type);
        Assert.Equal(0m, transaction.Amount);
        Assert.True(transaction.Timestamp <= DateTime.UtcNow);
        Assert.Equal(string.Empty, transaction.Description);
    }

    [Fact]
    public void Transaction_SetProperties_ShouldWorkCorrectly()
    {
        // Arrange
        var id = Guid.NewGuid();
        var type = TransactionType.Withdrawal;
        var amount = 50.25m;
        var timestamp = DateTime.UtcNow.AddMinutes(-5);
        var description = "Test withdrawal";

        // Act
        var transaction = new Transaction
        {
            Id = id,
            Type = type,
            Amount = amount,
            Timestamp = timestamp,
            Description = description
        };

        // Assert
        Assert.Equal(id, transaction.Id);
        Assert.Equal(type, transaction.Type);
        Assert.Equal(amount, transaction.Amount);
        Assert.Equal(timestamp, transaction.Timestamp);
        Assert.Equal(description, transaction.Description);
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
        var newBalance = 150.75m;
        var timestamp = DateTime.UtcNow;

        // Act
        var moneyDeposited = new MoneyDeposited(accountId, amount, newBalance, timestamp);

        // Assert
        Assert.Equal(accountId, moneyDeposited.AccountId);
        Assert.Equal(amount, moneyDeposited.Amount);
        Assert.Equal(newBalance, moneyDeposited.NewBalance);
        Assert.Equal(timestamp, moneyDeposited.Timestamp);
    }

    [Fact]
    public void MoneyWithdrawn_ShouldHaveCorrectProperties()
    {
        // Arrange
        var accountId = "test-123";
        var amount = 25.50m;
        var newBalance = 75.25m;
        var timestamp = DateTime.UtcNow;

        // Act
        var moneyWithdrawn = new MoneyWithdrawn(accountId, amount, newBalance, timestamp);

        // Assert
        Assert.Equal(accountId, moneyWithdrawn.AccountId);
        Assert.Equal(amount, moneyWithdrawn.Amount);
        Assert.Equal(newBalance, moneyWithdrawn.NewBalance);
        Assert.Equal(timestamp, moneyWithdrawn.Timestamp);
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