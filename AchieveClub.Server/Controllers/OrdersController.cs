using AchieveClub.Server.ApiContracts.Orders.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AchieveClub.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController(ILogger<OrdersController> logger, ApplicationContext db) : ControllerBase
{
    [HttpPost]
    [Authorize]
    public async Task<ActionResult> CreateProductOrder([FromBody] CreateOrderRequest request)
    {
        if (await db.Products.AnyAsync(p => p.Id == request.productId) == false)
        {
            logger.LogWarning("Product with productId:{request.productId} not found", request.productId);
            return NotFound($"Product with productId:{request.productId} not found");
        }
        
        if (await db.Variants.AnyAsync(v => v.Id == request.variantId) == false)
        {
            logger.LogWarning("Variant with variantId:{request.variantId} not found", request.variantId);
            return NotFound($"Variant with variantId:{request.variantId} not found");
        }

        return NoContent();
    }
}