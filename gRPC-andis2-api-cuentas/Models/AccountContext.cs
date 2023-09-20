using Microsoft.EntityFrameworkCore;

namespace gRPC_andis2_api_cuentas.Models
{
    public class AccountContext : DbContext
    {
        public AccountContext(DbContextOptions<AccountContext> options)
            : base(options)
        {
        }

        public DbSet<AccountModel>? Account { get; set; }
    }
}
