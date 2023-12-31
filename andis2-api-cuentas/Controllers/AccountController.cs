using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using andis2_api_cuentas.Models;
using Microsoft.AspNetCore.RateLimiting;

namespace andis2_api_cuentas.Controllers
{
    [Route("api/Accounts")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly AccountContext _context;
        private readonly ILogger<AccountController> _logger;

        public AccountController(AccountContext context, [FromServices]ILogger<AccountController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/Account
        [HttpGet]
        [EnableRateLimiting("fixed")]
        public async Task<ActionResult<IEnumerable<Account>>> GetAccount()
        {
          if (_context.Account == null)
          {
              return NotFound();
          }
            return await _context.Account.ToListAsync();
        }

        // GET: api/Account/5
        [HttpGet("{id}")]
        [EnableRateLimiting("sliding")]
        public async Task<ActionResult<Account>> GetAccount(int id)
        {
          if (_context.Account == null)
          {
              return NotFound();
          }
            var account = await _context.Account.FindAsync(id);

            if (account == null)
            {
                _logger.LogError("Id {id} no existe", id);
                return NotFound();
            }

            _logger.LogInformation("Funca");

            return account;
        }

        // PATCH: api/Account/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPatch("{id}")]
        [EnableRateLimiting("token")]
        public async Task<IActionResult> PutAccount(int id, string accountName)
        {
            Account? dbAccount = await _context.Account.FindAsync(id);
            if (dbAccount == null) return BadRequest();

            dbAccount.accountName = accountName;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // PATCH: api/Account/5/permissions
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPatch("{id}/permissions")]
        [EnableRateLimiting("concurrencyPolicy")]
        public async Task<IActionResult> PutAccountPermissions(int id, string permissions)
        {
            var account = await _context.Account.FindAsync(id);
            if (account == null) return NotFound();

            account.permissions = permissions;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // POST: api/Account
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Account>> PostAccount(Account account)
        {
          if (_context.Account == null)
          {
              return Problem("Entity set 'AccountContext.Account'  is null.");
          }
            _context.Account.Add(account);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetAccount", new { id = account.accountNumber }, account);
        }

        // DELETE: api/Account/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAccount(int id)
        {
            if (_context.Account == null)
            {
                return NotFound();
            }
            var account = await _context.Account.FindAsync(id);
            if (account == null)
            {
                return NotFound();
            }

            _context.Account.Remove(account);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // PUT api/accounts/:id/deposit
        // genera un endpoint para que un usuario pueda hacer depositos
        [HttpPut("{id}/deposit")]
        public async Task<IActionResult> PutDeposit(int id, Account account, int amount)
        {
            if (id != account.accountNumber)
            {
                return BadRequest();
            }
            if (amount < 0){
                return BadRequest();
            }
            

            _context.Entry(account).State = EntityState.Modified;

            try
            {
                account.accountBalance += amount;
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AccountExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();

        }

        private bool AccountExists(int id)
        {
            return (_context.Account?.Any(e => e.accountNumber == id)).GetValueOrDefault();
        }
    }
}
