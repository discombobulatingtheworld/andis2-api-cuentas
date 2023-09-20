using Grpc.Core;
using gRPC_andis2_api_cuentas.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Principal;

namespace gRPC_andis2_api_cuentas.Services
{
    public class GreeterService : Greeter.GreeterBase
    {
        private readonly ILogger<GreeterService> _logger;
        private readonly AccountContext _context;

        private static RpcException NewRpcException(StatusCode code, string message)
        {
            return new RpcException(new Status(code, message));
        }

        private static Account AccountDbToRpc(AccountModel model)
        {
            return new Account
            {
                AccountNumber = model.accountNumber,
                AccountBalance = model.accountBalance,
                AccountName = model.accountName,
                OwnerId = model.ownerId,
                Permissions = model.permissions
            };
        }

        private static AccountModel AccountRpcToDb(Account account)
        {
            return new AccountModel
            {
                accountNumber = account.AccountNumber,
                accountBalance = account.AccountBalance,
                accountName = account.AccountName,
                ownerId = account.OwnerId,
                permissions = account.Permissions
            };
        }

        private DbSet<AccountModel> GetDb()
        {
            DbSet<AccountModel>? dbAccounts = _context.Account;
            if (dbAccounts == null)
            {
                throw NewRpcException(StatusCode.Internal, "Internal server error");
            }

            return dbAccounts;
        }

        public GreeterService(ILogger<GreeterService> logger, AccountContext context)
        {
            _logger = logger;
            _context = context;
        }

        private static Account NewAccount(int accountNumber, int accountBalance, string accountName, int ownerId, string permissions)
        {
            return new Account
            {
                AccountNumber = accountNumber,
                AccountBalance = accountBalance,
                AccountName = accountName,
                OwnerId = ownerId,
                Permissions = permissions
            };
        }

        public async override Task<Account> GetAccount(Id id, ServerCallContext context)
        {
            var db = GetDb();
            var account = await db.FindAsync(id.Id_) ?? throw NewRpcException(StatusCode.NotFound, "Account not found");
            return AccountDbToRpc(account);
        }

        public async override Task<AccountList> GetAccounts(Empty request, ServerCallContext context)
        {
            var db = GetDb();
            List<AccountModel> accounts = await db.ToListAsync();

            return new AccountList
            {
                Accounts = { accounts.Select(AccountDbToRpc) }
            };
        }

        public async override Task<Empty> PostAccount(PostAccountInput request, ServerCallContext context)
        {
            var db = GetDb();
            db.Add(new AccountModel
            {
                accountNumber = 0,
                accountBalance = 0,
                accountName = request.AccountName,
                ownerId = request.OwnerId,
                permissions = ""
            });

            await _context.SaveChangesAsync();

            return new Empty();
        }

        public async override Task<Empty> PatchAccount(PatchAccountInput request, ServerCallContext context)
        {
            var db = GetDb();
            AccountModel model = await db.FindAsync(request.Id) ?? throw NewRpcException(StatusCode.NotFound, "Account not found");

            model.accountName = request.Name;
            await _context.SaveChangesAsync();
            return new Empty();
        }

        public async override Task<Empty> DeleteAccount(Id request, ServerCallContext context)
        {
            var db = GetDb();
            var account = await db.FindAsync(request.Id_) ?? throw NewRpcException(StatusCode.NotFound, "Account not found");

            db.Remove(account);
            await _context.SaveChangesAsync();

            return new Empty();
        }

        public async override Task<Empty> Deposit(DepositInput request, ServerCallContext context)
        {
            if (request.Amount < 0) throw NewRpcException(StatusCode.InvalidArgument, "Amount must be non-negative");

            var db = GetDb();

            AccountModel model = await db.FindAsync(request.Id) ?? throw NewRpcException(StatusCode.NotFound, "Account not found");

            try
            {
                model.accountBalance += request.Amount;
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AccountExists(request.Id))
                {
                    throw NewRpcException(StatusCode.NotFound, "Account not found");
                }
                else
                {
                    throw;
                }
            }

            return new Empty();
        }

        private bool AccountExists(int id)
        {
            return (_context.Account?.Any(e => e.accountNumber == id)).GetValueOrDefault();
        }
    }
}