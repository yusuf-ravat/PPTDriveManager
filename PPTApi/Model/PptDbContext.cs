using Microsoft.EntityFrameworkCore;

namespace PPTApi.Model
{
    public class PptDbContext:DbContext
    {
        public PptDbContext(DbContextOptions<PptDbContext> options) : base(options) { }
        public DbSet<PPTDetails> PptDetails { get; set; }
    }
}
