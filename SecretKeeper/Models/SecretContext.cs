using Microsoft.EntityFrameworkCore;

namespace SecretKeeper.Models
{
    public class SecretContext: DbContext
    {
        public SecretContext(DbContextOptions<SecretContext> options)
            : base(options)
        {
        }

        public DbSet<SecretItem> SecretItems { get; set; }
    }
}
