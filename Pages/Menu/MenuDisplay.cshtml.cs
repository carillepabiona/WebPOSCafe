using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WebPOSCafe.Data;
using WebPOSCafe.Models;

namespace WebPOSCafe.Pages.Menu
{
    public class MenuDisplayModel : PageModel
    {
        private readonly AppDbContext _db;


        public MenuDisplayModel(AppDbContext db)
        {
            _db = db;
        }

        [BindProperty(SupportsGet = true)]
        public string Table { get; set; } = "";   // ? reads ?table=5 from URL
        public List<Category> Categories { get; set; } = new();
        public List<MenuItem> MenuItems { get; set; } = new();

        public async Task OnGetAsync()
        {
            Categories = await _db.Categories
                .Where(c => c.IsActive && !c.IsDeleted)
                .OrderBy(c => c.DisplayOrder)
                .ToListAsync();

            MenuItems = await _db.MenuItems
                .Where(m => !m.Category.IsDeleted && m.Category.IsActive)
                .Include(m => m.Category)
                .Include(m => m.MenuItemCustomizations)
                    .ThenInclude(mic => mic.Customization)
                        .ThenInclude(c => c.Options)
                .Include(m => m.MenuItemAddons)
                    .ThenInclude(mia => mia.Addon)
                .OrderBy(m => m.Category.DisplayOrder)
                    .ThenBy(m => m.DisplayOrder)
                .ToListAsync();
        }
    }
}
