using Microsoft.EntityFrameworkCore;

namespace andis2_api_cuentas.Models{
    public class AccountContext : DbContext
    {
        public AccountContext(DbContextOptions<AccountContext> options)
            : base(options)
        {
        }

        public DbSet<Account> Account { get; set; } = null!;


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            new AccountInitializer(modelBuilder).Seed();
        }
    }
}