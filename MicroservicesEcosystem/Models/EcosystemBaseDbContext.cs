using Microsoft.EntityFrameworkCore;


namespace MicroservicesEcosystem.Models
{
    public class EcosystemBaseDbContext : DbContext
    {
        public EcosystemBaseDbContext()
        {
        }

        public EcosystemBaseDbContext(DbContextOptions<EcosystemBaseDbContext> options)
            : base(options)
        {
        }
        public DbSet<Document> Document { get; set; }

        public DbSet<Signature> Signature { get; set; }

        public DbSet<TokenValidation> TokenValidation { get; set; }



    }
}
