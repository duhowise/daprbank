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
        var balance = 100.50m;
        var createdAt = DateTime.UtcNow.AddDays(-1);
        var lastUpdated = DateTime.UtcNow;

        // Act
        var accountState = new AccountState
        {
            AccountId = accountId,
            AccountName = accountName,
            Balance = balance,
            CreatedAt = createdAt,
            LastUpdated = lastUpdated
        };

        // Assert
        Assert.Equal(accountId, accountState.AccountId);
        Assert.Equal(accountName, accountState.AccountName);
        Assert.Equal(balance, accountState.Balance);
        Assert.Equal(createdAt, accountState.CreatedAt);
        Assert.Equal(lastUpdated, accountState.LastUpdated);
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