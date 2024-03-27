using Microsoft.EntityFrameworkCore;
using System;

public class TokenDbContext : DbContext
{
    public TokenDbContext(DbContextOptions<TokenDbContext> options) : base(options) { }

    public DbSet<TokenData> TokenData { get; set; }

}