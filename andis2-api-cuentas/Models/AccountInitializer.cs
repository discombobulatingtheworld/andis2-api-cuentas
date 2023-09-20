
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace andis2_api_cuentas.Models;

public class AccountInitializer
{
    private readonly ModelBuilder _modelBuilder;

    public AccountInitializer(ModelBuilder modelBuilder)
    {
        _modelBuilder = modelBuilder;
    }
    
    public void Seed()
    {
        _modelBuilder.Entity<Account>().HasData(
            new Account
            {
                accountNumber = 1,
                accountBalance = 1000,
                accountName = "test",
                ownerId = 1,
                permissions = "nope"
            }
        );
    }
}