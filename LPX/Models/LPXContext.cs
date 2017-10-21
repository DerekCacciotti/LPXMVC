using System;
using Microsoft.EntityFrameworkCore;
namespace LPX.Models
{
    public class LPXContext : DbContext
    {
        public DbSet<FoodModel> FoodSource { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Suppiler> Suppliers { get; set; }
        public DbSet<FulfilledOrder> FulfilledOrders { get; set; }

     
        public LPXContext()
        {
            
        }

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			var connection = @"Server=52.168.173.109;Database=LPX;User Id=lpxuser;Password=Apple123@!;";

            optionsBuilder.UseSqlServer(connection);
		}

        public LPXContext(DbContextOptions<LPXContext> options)
    : base(options)
        { 
        
        }
    }
}
