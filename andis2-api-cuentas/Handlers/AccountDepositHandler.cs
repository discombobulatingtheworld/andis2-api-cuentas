using andis2_api_cuentas.Models;
using andis2_api_cuentas.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace andis2_api_cuentas.Handlers;

public record AccountDeposit(Guid Guid, int AccId, int DepAmount) : INotification;

public class AccountDepositHandler : INotificationHandler<AccountDeposit>
{
    private readonly ILogger<AccountDepositHandler> _logger;
    private readonly AccountContext _context;
    private readonly IStatusService _statusService;

    public AccountDepositHandler(ILogger<AccountDepositHandler> logger, AccountContext context, IStatusService statusService)
    {
        _logger = logger;
        _context = context;
        _statusService = statusService;
    }

    public Task Handle(AccountDeposit notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling {Notification}", nameof(AccountDeposit));
        _statusService.SetStatus(notification.Guid, StatusValues.Processing);

        var account = _context.Account.Find(notification.AccId);
        if (account == null)
        {
            return Task.CompletedTask;
        }

        _context.Entry(account).State = EntityState.Modified;

        try
        {
            account.accountBalance += notification.DepAmount;
            _context.SaveChanges();
            _statusService.SetStatus(notification.Guid, StatusValues.Complete);
        }
        catch (DbUpdateConcurrencyException)
        {
            _statusService.SetStatus(notification.Guid, StatusValues.Failed);
        }

        return Task.CompletedTask;
    }
}