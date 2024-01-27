using AkakceProject.Application.Mediators;
using AkakceProject.Core.Dto;
using AkakceProject.Core.Entities;
using AkakceProject.Core.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace AkakceProject.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ProductController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [Route("GetProducts")]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
            Stopwatch stopwatch = Stopwatch.StartNew();

            try
            {
                var products = await _mediator.Send(new ProductMediator.GetAllProductsQuery());
                stopwatch.Stop();
                Console.WriteLine($"Time elapsed for GetProducts query: {stopwatch.Elapsed}");
                return Ok(products.Take(200));
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                Console.WriteLine($"Time elapsed for GetProducts query: {stopwatch.Elapsed}");
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet]
        [Route("GetProductById")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();

            try
            {
                var product = await _mediator.Send(new ProductMediator.GetProductQuery { ProductId = id });
                if (product == null)
                {
                    stopwatch.Stop();
                    Console.WriteLine($"Time elapsed for GetProduct query: {stopwatch.Elapsed}");
                    return NotFound();
                }
                stopwatch.Stop();
                Console.WriteLine($"Time elapsed for GetProduct query: {stopwatch.Elapsed}");
                return Ok(product);
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                Console.WriteLine($"Time elapsed for GetProduct query: {stopwatch.Elapsed}");
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet]
        [Route("GetProductsByUserId")]
        public async Task<ActionResult<IEnumerable<Product>>> GetUserProducts(int id)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();

            try
            {
                var products = await _mediator.Send(new ProductMediator.GetUserProductsQuery { UserId = id });
                stopwatch.Stop();
                Console.WriteLine($"Time elapsed for GetUserProducts query: {stopwatch.Elapsed}");
                return Ok(products.Take(200));
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                Console.WriteLine($"Time elapsed for GetUserProducts query: {stopwatch.Elapsed}");
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet]
        [Route("GetProductsByCampaignId")]
        public async Task<ActionResult<IEnumerable<Product>>> GetCampaignProducts(int id)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();

            try
            {
                var products = await _mediator.Send(new ProductMediator.GetCampaignProductsQuery { CampaignId = id });
                stopwatch.Stop();
                Console.WriteLine($"Time elapsed for GetCampaignProducts query: {stopwatch.Elapsed}");
                return Ok(products.Take(200));
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                Console.WriteLine($"Time elapsed for GetCampaignProducts query: {stopwatch.Elapsed}");
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet]
        [Route("GetProductInfo")]
        public async Task<ActionResult<ProductResponse>> GetProductInfo(int userId, int productId)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();

            try
            {
                var product = await _mediator.Send(new ProductMediator.GetProductInfoQuery { UserId = userId, ProductId = productId });
                if (product == null)
                {
                    stopwatch.Stop();
                    Console.WriteLine($"Time elapsed for GetProductInfo query: {stopwatch.Elapsed}");
                    return NotFound();
                }
                stopwatch.Stop();
                Console.WriteLine($"Time elapsed for GetProductInfo query: {stopwatch.Elapsed}");
                return Ok(product);
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                Console.WriteLine($"Time elapsed for GetProductInfo query: {stopwatch.Elapsed}");
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        [Route("CreateProduct")]
        public async Task<ActionResult<Product>> CreateProduct(ProductForCreationDto product)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();

            try
            {
                var createdProduct = await _mediator.Send(new ProductMediator.CreateProductQuery { Product = product });
                stopwatch.Stop();
                Console.WriteLine($"Time elapsed for CreateProduct query: {stopwatch.Elapsed}");
                return CreatedAtAction(nameof(GetProduct), new { id = createdProduct.ProductId }, createdProduct);
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                Console.WriteLine($"Time elapsed for CreateProduct query: {stopwatch.Elapsed}");
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut]
        [Route("UpdateProduct")]
        public async Task<ActionResult> UpdateProduct(int id, ProductForUpdateDto product)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();

            try
            {
                var dbProduct = await _mediator.Send(new ProductMediator.GetProductQuery { ProductId = id });
                if (dbProduct == null)
                {
                    stopwatch.Stop();
                    Console.WriteLine($"Time elapsed for UpdateProduct query: {stopwatch.Elapsed}");
                    return NotFound();
                }
                await _mediator.Send(new ProductMediator.UpdateProductQuery { ProductId = id, Product = product });
                stopwatch.Stop();
                Console.WriteLine($"Time elapsed for UpdateProduct query: {stopwatch.Elapsed}");
                return NoContent();
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                Console.WriteLine($"Time elapsed for UpdateProduct query: {stopwatch.Elapsed}");
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete]
        [Route("DeleteProduct")]
        public async Task<ActionResult> DeleteProduct(int id)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();

            try
            {
                var dbProduct = await _mediator.Send(new ProductMediator.GetProductQuery { ProductId = id });
                if (dbProduct == null)
                {
                    stopwatch.Stop();
                    Console.WriteLine($"Time elapsed for DeleteProduct query: {stopwatch.Elapsed}");
                    return NotFound();
                }
                await _mediator.Send(new ProductMediator.DeleteProductQuery { ProductId = id });
                stopwatch.Stop();
                Console.WriteLine($"Time elapsed for DeleteProduct query: {stopwatch.Elapsed}");
                return NoContent();
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                Console.WriteLine($"Time elapsed for DeleteProduct query: {stopwatch.Elapsed}");
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete]
        [Route("DeleteProductsByUserId")]
        public async Task<ActionResult> DeleteUserProducts(int id)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();

            try
            {
                await _mediator.Send(new ProductMediator.DeleteUserProductsQuery { UserId = id });
                stopwatch.Stop();
                Console.WriteLine($"Time elapsed for DeleteUserProducts query: {stopwatch.Elapsed}");
                return NoContent();
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                Console.WriteLine($"Time elapsed for DeleteUserProducts query: {stopwatch.Elapsed}");
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete]
        [Route("DeleteProductsByCampaignId")]
        public async Task<ActionResult> DeleteCampaignProducts(int id)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();

            try
            {
                await _mediator.Send(new ProductMediator.DeleteCampaignProductsQuery { CampaignId = id });
                stopwatch.Stop();
                Console.WriteLine($"Time elapsed for DeleteCampaignProducts query: {stopwatch.Elapsed}");
                return NoContent();
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                Console.WriteLine($"Time elapsed for DeleteCampaignProducts query: {stopwatch.Elapsed}");
                return StatusCode(500, ex.Message);
            }
        }
    }
}