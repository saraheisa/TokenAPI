using Microsoft.EntityFrameworkCore;
using System;

public class TokenDbContext : DbContext
{
    public TokenDbContext(DbContextOptions<TokenDbContext> options) : base(options) { }

    public DbSet<TokenData> TokenData { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Define seed data for TokenData table
        modelBuilder.Entity<TokenData>().HasData(
            new TokenData { Id = 1, Name = "BLP Token", TotalSupply = "0", CirculatingSupply = "0" }
        );
    }

}