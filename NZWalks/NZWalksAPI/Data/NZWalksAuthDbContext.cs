using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace NZWalksAPI.Data
{
    public class NZWalksAuthDbContext : IdentityDbContext
    {
        public NZWalksAuthDbContext(DbContextOptions<NZWalksAuthDbContext> dbContextOptions) : base(dbContextOptions)
        {

        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            string readerRoleId = "7736db15-2960-471f-bb5d-333fad5c6e00"; //Random Guid for demo purposes
            string writerRoleId = "130a43cd-8b4b-4fa3-85b4-8d57672b3a28"; //Random Guid for demo purposes

            var roles = new List<IdentityRole>() {
                new () { Id = readerRoleId, Name = "Reader", ConcurrencyStamp = readerRoleId, NormalizedName = "Reader".ToUpper() },
                new () { Id = writerRoleId, Name = "Writer", ConcurrencyStamp = writerRoleId, NormalizedName = "Writer".ToUpper() }
            };

            builder.Entity<IdentityRole>().HasData(roles);
        }
    }
}
