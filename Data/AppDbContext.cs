using Microsoft.EntityFrameworkCore;
using WebPOSCafe.Models;

namespace WebPOSCafe.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }

        public DbSet<MenuItem> MenuItems { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Customization> Customizations { get; set; }
        public DbSet<CustomizationOption> CustomizationOptions { get; set; }
        public DbSet<Addon> Addons { get; set; }

        public DbSet<MenuItemCustomization> MenuItemCustomizations { get; set; }
        public DbSet<MenuItemAddon> MenuItemAddons { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // existing config...

            modelBuilder.Entity<MenuItemCustomization>()
                .HasKey(x => new { x.MenuItemId, x.CustomizationId });

            modelBuilder.Entity<MenuItemAddon>()
                .HasKey(x => new { x.MenuItemId, x.AddonId });
        }
    }
}
