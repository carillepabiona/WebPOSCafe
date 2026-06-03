using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WebPOSCafe.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebPOSCafe.Pages.Admin
{
    public class MenuDisplayModel : PageModel
    {
        private readonly AppDbContext _db;

        public MenuDisplayModel(AppDbContext db) => _db = db;

        public List<MenuDisplayCategory> Categories { get; set; } = new();
        public string ActiveCategory { get; set; } = "all";

        public async Task OnGetAsync(string category = "all")
        {
            ActiveCategory = category ?? "all";

            // Load all active categories with their items and relationships
            var categories = await _db.Categories
                .Where(c => c.IsActive)
                .OrderBy(c => c.DisplayOrder)
                .ToListAsync();

            var menuItems = await _db.MenuItems
                .Include(m => m.Category)
                .Include(m => m.MenuItemCustomizations)
                    .ThenInclude(mc => mc.Customization)
                        .ThenInclude(c => c.Options)
                .Include(m => m.MenuItemAddons)
                    .ThenInclude(ma => ma.Addon)
                .OrderBy(m => m.DisplayOrder)
                .ThenBy(m => m.Name)
                .ToListAsync();

            Categories = categories.Select(cat => new MenuDisplayCategory
            {
                Id = cat.Id.ToString(),
                Name = cat.CategoryName,
                Color = cat.Color,
                Items = menuItems
                    .Where(m => m.CategoryId == cat.Id)
                    .Select(m => new MenuDisplayItem
                    {
                        Id = m.Id.ToString(),
                        Name = m.Name,
                        Description = m.Description,
                        Price = m.Price,
                        PreparationTime = m.PreparationTime,
                        IsAvailable = m.IsAvailable,
                        ImageBase64 = m.ImageBase64,
                        ImageMimeType = m.ImageMimeType,

                        Customizations = m.MenuItemCustomizations
                            .Select(mc => new MenuDisplayCustomization
                            {
                                Id = mc.Customization.Id.ToString(),
                                Name = mc.Customization.Name,
                                Type = mc.Customization.Type,
                                IsRequired = mc.Customization.IsRequired,
                                Options = mc.Customization.Options
                                    .OrderBy(o => o.DisplayOrder)
                                    .Select(o => new MenuDisplayOption
                                    {
                                        Id = o.Id.ToString(),
                                        Label = o.OptionLabel,
                                        PriceModifier = o.PriceModifier,
                                        IsDefault = o.IsDefault
                                    }).ToList()
                            }).ToList(),

                        Addons = m.MenuItemAddons
                            .Where(ma => ma.Addon.IsActive)
                            .Select(ma => new MenuDisplayAddon
                            {
                                Id = ma.Addon.Id.ToString(),
                                Name = ma.Addon.Name,
                                Category = ma.Addon.Category,
                                Price = ma.Addon.Price
                            }).ToList()
                    }).ToList()
            })
            .Where(c => c.Items.Any()) // Only show categories that have items
            .ToList();
        }
    }

    // ── View-Model DTOs ───────────────────────────────────────────
    public class MenuDisplayCategory
    {
        public string Id { get; set; } = "";
        public string Name { get; set; } = "";
        public string Color { get; set; } = "#8B4513";
        public List<MenuDisplayItem> Items { get; set; } = new();
    }

    public class MenuDisplayItem
    {
        public string Id { get; set; } = "";
        public string Name { get; set; } = "";
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int PreparationTime { get; set; }
        public bool IsAvailable { get; set; }
        public string? ImageBase64 { get; set; }
        public string? ImageMimeType { get; set; }
        public List<MenuDisplayCustomization> Customizations { get; set; } = new();
        public List<MenuDisplayAddon> Addons { get; set; } = new();
    }

    public class MenuDisplayCustomization
    {
        public string Id { get; set; } = "";
        public string Name { get; set; } = "";
        public string Type { get; set; } = "Radio";
        public bool IsRequired { get; set; }
        public List<MenuDisplayOption> Options { get; set; } = new();
    }

    public class MenuDisplayOption
    {
        public string Id { get; set; } = "";
        public string Label { get; set; } = "";
        public decimal PriceModifier { get; set; }
        public bool IsDefault { get; set; }
    }

    public class MenuDisplayAddon
    {
        public string Id { get; set; } = "";
        public string Name { get; set; } = "";
        public string? Category { get; set; }
        public decimal Price { get; set; }
    }
}
