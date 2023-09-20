using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using andis2_api_cuentas.Models;
using andis2_api_cuentas.Types;
using andis2_api_cuentas.Handlers;

namespace andis2_api_cuentas.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly AccountContext _context;
        private readonly ITaskQueue _taskQueue;

        public AccountController(AccountContext context, ITaskQueue queue)
        {
            _context = context;
            _taskQueue = queue;
        }

        // GET: api/Account
        [HttpGet]
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
        public async Task<ActionResult<Account>> GetAccount(int id)
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

            return account;
        }

        // PUT: api/Account/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAccount(int id, Account account)
        {
            if (id != account.accountNumber)
            {
                return BadRequest();
            }

            _context.Entry(account).State = EntityState.Modified;

            try
            {
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
        public async Task<IActionResult> PutDeposit(int id, int amount)
        {
            if (!int.IsPositive(amount))
                return BadRequest();
            if (_context.Account == null)
                return NotFound();
            if (!int.IsPositive(id))
                return BadRequest();
            if (!AccountExists(id))
                return NotFound();

            var taskId = Guid.NewGuid();
            await _taskQueue.QueueTaskAsync(new AccountDeposit(taskId, id, amount));

            return Ok(taskId);

        }

        private bool AccountExists(int id)
        {
            return (_context.Account?.Any(e => e.accountNumber == id)).GetValueOrDefault();
        }
    }
}
