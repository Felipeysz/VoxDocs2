using Microsoft.EntityFrameworkCore;

namespace VoxDocs.Data
{
    public class VoxDocsContext : DbContext
    {
        public VoxDocsContext(DbContextOptions<VoxDocsContext> options)
            : base(options)
        {
        }

        public DbSet<VoxDocs.Models.UserModel> Users { get; set; }
    }
}
