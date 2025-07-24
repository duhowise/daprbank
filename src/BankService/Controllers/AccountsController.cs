using Dapr.Actors;
using Dapr.Actors.Client;
using Microsoft.AspNetCore.Mvc;
using BankService.Actors;

namespace BankService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountsController : ControllerBase
{
    private readonly IActorProxyFactory _actorProxyFactory;
    private readonly ILogger<AccountsController> _logger;

    public AccountsController(IActorProxyFactory actorProxyFactory, ILogger<AccountsController> logger)
    {
        _actorProxyFactory = actorProxyFactory;
        _logger = logger;
    }

    [HttpPost("{id}/create")]
    public async Task<IActionResult> CreateAccount(string id, [FromBody] CreateAccountRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return BadRequest("Account ID cannot be empty");
            }

            if (string.IsNullOrWhiteSpace(request.AccountName))
            {
                return BadRequest("Account name cannot be empty");
            }

            var actorId = new ActorId(id);
            var accountActor = _actorProxyFactory.CreateActorProxy<IAccountActor>(actorId, nameof(AccountActor));
            
            await accountActor.CreateAccount(request.AccountName);
            
            _logger.LogInformation("Account {AccountId} created successfully", id);
            return Ok(new { AccountId = id, Message = "Account created successfully" });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning("Account creation failed: {Message}", ex.Message);
            return Conflict(new { Error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating account {AccountId}", id);
            return StatusCode(500, new { Error = "Internal server error" });
        }
    }

    [HttpPost("{id}/deposit")]
    public async Task<IActionResult> Deposit(string id, [FromBody] TransactionRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return BadRequest("Account ID cannot be empty");
            }

            if (request.Amount <= 0)
            {
                return BadRequest("Deposit amount must be positive");
            }

            var actorId = new ActorId(id);
            var accountActor = _actorProxyFactory.CreateActorProxy<IAccountActor>(actorId, nameof(AccountActor));
            
            await accountActor.Deposit(request.Amount);
            var newBalance = await accountActor.GetBalance();
            
            _logger.LogInformation("Deposited {Amount} to account {AccountId}", request.Amount, id);
            return Ok(new { AccountId = id, Amount = request.Amount, NewBalance = newBalance, Message = "Deposit successful" });
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning("Invalid deposit request: {Message}", ex.Message);
            return BadRequest(new { Error = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning("Deposit failed: {Message}", ex.Message);
            return NotFound(new { Error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing deposit for account {AccountId}", id);
            return StatusCode(500, new { Error = "Internal server error" });
        }
    }

    [HttpPost("{id}/withdraw")]
    public async Task<IActionResult> Withdraw(string id, [FromBody] TransactionRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return BadRequest("Account ID cannot be empty");
            }

            if (request.Amount <= 0)
            {
                return BadRequest("Withdrawal amount must be positive");
            }

            var actorId = new ActorId(id);
            var accountActor = _actorProxyFactory.CreateActorProxy<IAccountActor>(actorId, nameof(AccountActor));
            
            await accountActor.Withdraw(request.Amount);
            var newBalance = await accountActor.GetBalance();
            
            _logger.LogInformation("Withdrew {Amount} from account {AccountId}", request.Amount, id);
            return Ok(new { AccountId = id, Amount = request.Amount, NewBalance = newBalance, Message = "Withdrawal successful" });
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning("Invalid withdrawal request: {Message}", ex.Message);
            return BadRequest(new { Error = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning("Withdrawal failed: {Message}", ex.Message);
            return BadRequest(new { Error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing withdrawal for account {AccountId}", id);
            return StatusCode(500, new { Error = "Internal server error" });
        }
    }

    [HttpGet("{id}/balance")]
    public async Task<IActionResult> GetBalance(string id)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return BadRequest("Account ID cannot be empty");
            }

            var actorId = new ActorId(id);
            var accountActor = _actorProxyFactory.CreateActorProxy<IAccountActor>(actorId, nameof(AccountActor));
            
            var balance = await accountActor.GetBalance();
            
            _logger.LogInformation("Retrieved balance for account {AccountId}: {Balance}", id, balance);
            return Ok(new { AccountId = id, Balance = balance });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning("Balance query failed: {Message}", ex.Message);
            return NotFound(new { Error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving balance for account {AccountId}", id);
            return StatusCode(500, new { Error = "Internal server error" });
        }
    }
}

public record CreateAccountRequest(string AccountName);
public record TransactionRequest(decimal Amount);