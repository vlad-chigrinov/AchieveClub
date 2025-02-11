using AchieveClub.Server.ApiContracts.Orders.Request;
using AchieveClub.Server.ApiContracts.Orders.Response;
using AchieveClub.Server.ApiContracts.Products.Response;
using AchieveClub.Server.RepositoryItems;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AchieveClub.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController(ILogger<OrdersController> logger, ApplicationContext db) : ControllerBase
{
    [HttpGet]
    [Authorize]
    public async Task<ActionResult<List<OrderResponse>>> GetUserOrders()
    {
        var userIdString = HttpContext.User.Identity?.Name;
        if (userIdString == null || int.TryParse(userIdString, out int userId) == false)
        {
            logger.LogWarning("Access token not contains userId or userId is the wrong format: {userIdString}",
                userIdString);
            return NotFound($"Access token not contains userId or userId is the wrong format: {userIdString}");
        }

        if (await db.Users.AnyAsync(u => u.Id == userId) == false)
        {
            logger.LogWarning("User with userId:{userId} not found", userId);
            return NotFound($"User with userId:{userId} not found");
        }

        return await db.Orders
            .Where(o => o.UserId == userId)
            .Include(o=>o.Product)
            .Include(o=>o.DeliveryStatus)
            .Include(o=>o.Variant)
            .ThenInclude(v=>v!.DefaultPhoto)
            .Select(o => new OrderResponse(o.Id, o.Product!.Type, o.Product.Name, o.Price, o.Variant!.Name,
                o.Variant.DefaultPhoto!.Url, o.OrderDate, o.DeliveryStatus!.Title, o.DeliveryStatus.Color))
            .ToListAsync();
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult> CreateOrder([FromBody] CreateOrderRequest request)
    {
        var userIdString = HttpContext.User.Identity?.Name;
        if (userIdString == null || int.TryParse(userIdString, out int userId) == false)
        {
            logger.LogWarning("Access token not contains userId or userId is the wrong format: {userIdString}",
                userIdString);
            return NotFound($"Access token not contains userId or userId is the wrong format: {userIdString}");
        }

        var user = await db.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null)
        {
            logger.LogWarning("User with userId:{userId} not found", userId);
            return NotFound($"User with userId:{userId} not found");
        }

        var variant = await db.Variants
            .Include(v => v.Product)
            .FirstOrDefaultAsync(v => v.Id == request.variantId && v.ProductId == request.productId);

        if (variant == null)
        {
            logger.LogWarning("Product:{request.productId} with Variant:{request.variantId} not found",
                request.productId, request.variantId);
            return NotFound($"Product:{request.productId} with Variant:{request.variantId} not found");
        }

        if (user.Balance < variant.Product!.Price)
        {
            logger.LogWarning(
                "The current user does not have enough money to order the product. Balance:{user.Balance} < Price:{variant.Product.Price}",
                user.Balance, variant.Product.Price);
            return BadRequest(
                $"The current user does not have enough money to order the product. Balance:{user.Balance} < Price:{variant.Product.Price}");
        }

        if (variant.Quantity <= 0)
        {
            logger.LogWarning(
                "Out of stock. Product:{request.productId} Variant:{request.variantId}",
                request.productId, request.variantId);
            return BadRequest(
                $"Out of stock. Product:{request.productId} Variant:{request.variantId}");
        }

        user.Balance -= variant.Product!.Price;

        variant.Quantity--;

        var order = new OrderDBO
        {
            OrderDate = DateTime.Now,
            Price = variant.Product!.Price,
            User = user,
            Product = variant.Product,
            Variant = variant,
            DeliveryStatusId = 1
        };
        db.Orders.Add(order);

        await db.SaveChangesAsync();

        return Created();
    }
}