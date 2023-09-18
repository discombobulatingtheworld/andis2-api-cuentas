using Microsoft.AspNetCore.Mvc;
using andis2_api_cuentas.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace andis2_api_cuentas.Controllers
{
    [Route("api/accounts")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private List<Account> accounts;

        public AccountController()
        {
            string json = System.IO.File.ReadAllText("data.json");
            accounts = JsonSerializer.Deserialize<List<Account>>(json);
        }

        [HttpGet("{id}")]
        public ActionResult<Account> GetAccountById(int id)
        {
            var account = accounts.FirstOrDefault(a => a.accountNumber == id);

            if (account == null)
            {
                return NotFound();
            }

            return account;
        }

        [HttpPost]
        public ActionResult<Account> CreateAccount([FromBody] Account newAccount)
        {
            if (newAccount == null)
            {
                return BadRequest("Invalid account data");
            }

            try
            {
                int newAccountNumber = accounts.Max(a => a.accountNumber) + 1;
                newAccount.accountNumber = newAccountNumber;

                accounts.Add(newAccount);

                string updatedJson = JsonSerializer.Serialize(accounts);
                System.IO.File.WriteAllText("data.json", updatedJson);

                return CreatedAtAction(nameof(GetAccountById), new { id = newAccount.accountNumber }, newAccount);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating account: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{id}/permissions")]
        public ActionResult<Account> ModifyPermissions(int id, [FromBody] List<string> newPermissions)
        {
            var account = accounts.FirstOrDefault(a => a.accountNumber == id);

            if (account == null)
            {
                return NotFound();
            }

            account.permissions = newPermissions;

            string updatedJson = JsonSerializer.Serialize(accounts);
            System.IO.File.WriteAllText("data.json", updatedJson);

            return Ok(account);
        }

        [HttpDelete("{id}")]
        public ActionResult DeleteAccount(int id)
        {
            var accountToRemove = accounts.FirstOrDefault(a => a.accountNumber == id);

            if (accountToRemove == null)
            {
                return NotFound();
            }

            accounts.Remove(accountToRemove);

            string updatedJson = JsonSerializer.Serialize(accounts);
            System.IO.File.WriteAllText("data.json", updatedJson);

            return NoContent();
        }
    }
}