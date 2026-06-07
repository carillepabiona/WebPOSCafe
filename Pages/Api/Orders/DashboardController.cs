using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebPOSCafe.Data;

[ApiController]
[Route("api/dashboard")]
[IgnoreAntiforgeryToken]
public class DashboardController : ControllerBase
{
    private readonly AppDbContext _db;
    public DashboardController(AppDbContext db) => _db = db;

    // GET /api/dashboard/summary
    [HttpGet("summary")]
    public async Task<IActionResult> GetSummary()
    {
        var today = DateTime.Today;
        var tomorrow = today.AddDays(1);

        var todayOrders = await _db.Orders
            .Where(o => o.CreatedAt >= today && o.CreatedAt < tomorrow)
            .ToListAsync();

        var totalSales = todayOrders.Where(o => o.Status != "Cancelled").Sum(o => o.Total);
        var totalOrders = todayOrders.Count;
        var pendingOrders = todayOrders.Count(o => o.Status == "Pending");
        var completedOrders = todayOrders.Count(o => o.Status == "Completed");
        var avgOrderValue = totalOrders > 0 ? totalSales / totalOrders : 0;

        var totalItemsSold = await _db.OrderItems
            .Where(oi => _db.Orders
                .Where(o => o.CreatedAt >= today && o.CreatedAt < tomorrow && o.Status != "Cancelled")
                .Select(o => o.Id)
                .Contains(oi.OrderId))
            .SumAsync(oi => oi.Quantity);

        var tables = await _db.Tables.ToListAsync();
        var totalTables = tables.Count;
        var occupiedTables = tables.Count(t => t.Status == "Occupied");
        var availableTables = tables.Count(t => t.Status == "Available");

        return Ok(new
        {
            totalSales,
            totalOrders,
            pendingOrders,
            completedOrders,
            avgOrderValue,
            totalItemsSold,
            totalTables,
            occupiedTables,
            availableTables
        });
    }

    // GET /api/dashboard/recent-orders
    [HttpGet("recent-orders")]
    public async Task<IActionResult> GetRecentOrders()
    {
        var today = DateTime.Today;
        var tomorrow = today.AddDays(1);

        var orders = await _db.Orders
            .Where(o => o.CreatedAt >= today && o.CreatedAt < tomorrow)
            .OrderByDescending(o => o.CreatedAt)
            .Take(10)
            .Select(o => new {
                o.OrderNumber,
                o.CustomerName,
                o.TableNumber,
                o.Status,
                o.Total,
                TotalAmount = o.Total,
                o.CreatedAt
            })
            .ToListAsync();

        return Ok(orders);
    }

    // GET /api/dashboard/top-items
    [HttpGet("top-items")]
    public async Task<IActionResult> GetTopItems()
    {
        var today = DateTime.Today;
        var tomorrow = today.AddDays(1);

        var topItems = await _db.OrderItems
            .Where(oi => _db.Orders
                .Where(o => o.CreatedAt >= today && o.CreatedAt < tomorrow && o.Status != "Cancelled")
                .Select(o => o.Id)
                .Contains(oi.OrderId))
            .GroupBy(oi => oi.Name)
            .Select(g => new {
                name = g.Key,
                qtySold = g.Sum(x => x.Quantity),
                revenue = g.Sum(x => x.Price * x.Quantity)
            })
            .OrderByDescending(x => x.qtySold)
            .Take(5)
            .ToListAsync();

        return Ok(topItems);
    }
}