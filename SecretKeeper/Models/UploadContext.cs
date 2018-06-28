using Microsoft.EntityFrameworkCore;

namespace SecretKeeper.Models
{
    public class UploadContext: DbContext
    {
        public UploadContext(DbContextOptions<UploadContext> options)
            : base(options)
        {
        }
        public DbSet<UploadItem> UploadItems { get; set; }
    }
}
