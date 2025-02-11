using AchieveClub.Server.ApiContracts.Products.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AchieveClub.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController(ILogger<ProductsController> logger, ApplicationContext db) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<SmallProductResponse>>> GetByCategory([FromQuery] int categoryId)
    {
        if (db.Categories.Any(x => x.Id == categoryId) == false)
        {
            logger.LogWarning("Category with categoryId:{categoryId} not found", categoryId);
            return NotFound($"Category with categoryId:{categoryId} not found");
        }

        return await db.Products
            .Where(p => p.CategoryId == categoryId)
            .Include(p => p.Variants)!
            .ThenInclude(v => v.DefaultPhoto)
            .Select(p => new SmallProductResponse(
                p.Id, p.Type, p.Name, p.Price,
                p.Variants!.Select(x=> new SmallVariantResponse(x.Id, x.Color, x.DefaultPhoto!.Url, x.Id == p.DefaultVariantId)).ToList()
            )).ToListAsync();
    }

    [HttpGet("{productId:int}")]
    public async Task<ActionResult<ProductResponse>> GetById([FromRoute] int productId)
    {
        var product = await db.Products
            .Include(p => p.Variants)!
            .ThenInclude(v => v.ProductPhotos)
            .FirstOrDefaultAsync(p => p.Id == productId);

        if (product == null)
        {
            logger.LogWarning("Product with productId:{productId} not found", productId);
            return NotFound($"Product with productId:{productId} not found");
        }

        return new ProductResponse(
            product!.Id, product.Type, product.Name, product.Details, product.Price,
            product.Variants!
                .Select(v => new VariantResponse(
                    v.Id,
                    v.Id == product.DefaultVariantId,
                    v.Name,
                    v.Color,
                    v.ProductPhotos!.Select(p => new VariantPhotoResponse(p.Id == v.DefaultPhotoId, p.Url)).ToList()
                )).ToList()
        );
    }
}