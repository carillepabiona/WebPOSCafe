// ════════════════════════════════════════════════════════════════════
// Pages/Api/KdsController.cs  — JSON API consumed by the KDS page
// ════════════════════════════════════════════════════════════════════
using Microsoft.AspNetCore.Mvc;
using WebPOSCafe.Data;
using Microsoft.EntityFrameworkCore;

namespace WebPOSCafe.Pages.Api
{
    [ApiController]
    [Route("api/kds")]
    [IgnoreAntiforgeryToken]
    public class KdsController : ControllerBase
    {
        private readonly AppDbContext _db;
        public KdsController(AppDbContext db) => _db = db;

        // ── GET /api/kds/orders  ─────────────────────────────────────────
        // Returns pending / preparing / ready orders for the KDS
        [HttpGet("orders")]
        public IActionResult GetOrders()
        {
            var orders = _db.Orders
                .Include(o => o.Items)
                .Where(o => o.Status == "pending"
                         || o.Status == "preparing"
                         || o.Status == "ready")
                .OrderBy(o => o.CreatedAt)
                .Select(o => new
                {
                    id = o.Id,
                    orderNumber = o.OrderNumber,
                    customerName = o.CustomerName,
                    type = o.Type,
                    tableNumber = o.TableNumber,
                    status = o.Status,
                    createdAt = o.CreatedAt,
                    items = o.Items.Select(i => new
                    {
                        name = i.Name,
                        quantity = i.Quantity,
                        customization = i.Customization,
                        specialInstructions = i.SpecialInstructions
                    })
                })
                .ToList();

            return Ok(orders);
        }

        // ── POST /api/kds/status  ────────────────────────────────────────
        [HttpPost("status")]
        public async Task<IActionResult> UpdateStatus([FromBody] KdsStatusDto dto)
        {
            var order = await _db.Orders.FindAsync(dto.OrderId);
            if (order == null)
                return NotFound(new { error = "Order not found" });

            var allowed = new Dictionary<string, string[]>
            {
                { "awaiting_payment", new[] { "pending", "cancelled"  } },
                { "pending",          new[] { "preparing", "cancelled" } },
                { "preparing",        new[] { "ready"                 } },
                { "ready",            new[] { "served"             } }
            };

            if (!allowed.TryGetValue(order.Status, out var validNext) || !validNext.Contains(dto.NewStatus))
                return BadRequest(new { error = $"Cannot transition from {order.Status} to {dto.NewStatus}" });

            order.Status = dto.NewStatus;
            await _db.SaveChangesAsync();

            return Ok(new { success = true, newStatus = order.Status });
        }

        // GET /api/kds/orders-all  — returns ALL active orders (for cashier sync)
        [HttpGet("orders-all")]
        public IActionResult GetAllOrders()
        {
            var orders = _db.Orders
            // Change to:
            .Where(o => o.Status != "completed" && o.Status != "cancelled") 
                .OrderBy(o => o.CreatedAt)
                .Select(o => new
                {
                    id = o.Id,
                    orderNumber = o.OrderNumber,
                    status = o.Status,
                    elapsedTime = (int)(DateTime.UtcNow - o.CreatedAt).TotalMinutes + " mins"
                })
                .ToList();

            return Ok(orders);
        }
    }

    public class KdsStatusDto
    {
        public int OrderId { get; set; }
        public string NewStatus { get; set; } = "";
    }
}
