using Microsoft.AspNetCore.Mvc;

namespace OrderService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IHttpClientFactory _httpClientFactory;

    public OrdersController(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    [HttpPost]
    public async Task<IActionResult> CreateOrder(int productId, int quantity)
    {
        var client = _httpClientFactory.CreateClient("ProductService");

        var response = await client.GetAsync($"api/products/{productId}");

        if (!response.IsSuccessStatusCode)
        {
            return NotFound($"Product {productId} not found in ProductService");
        }

        var product = await response.Content.ReadFromJsonAsync<ProductDto>();

        var total = product!.Price * quantity;

        return Ok(new
        {
            ProductId = productId,
            ProductName = product.Name,
            Quantity = quantity,
            TotalPrice = total
        });
    }
}