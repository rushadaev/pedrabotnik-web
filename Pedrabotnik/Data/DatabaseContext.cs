using Microsoft.EntityFrameworkCore;
using Pedrabotnik.Models;

namespace Pedrabotnik.Data;

public class DatabaseContext : DbContext
{
    public DbSet<ChatsNThreads> ChatsNThreads { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(
            "Host=79.132.140.243;Port=5432;Database=pedrabotnik;Username=dev;Password=H4!b5at+kWls");
    }
}