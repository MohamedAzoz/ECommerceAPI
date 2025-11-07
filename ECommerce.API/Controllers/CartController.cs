using ECommerce.Core.DTOs.Cart;
using ECommerce.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ECommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        //GET    /api/cart
        //POST   /api/cart/items
        //PUT    /api/cart/items/{id}
        //DELETE /api/cart/items/{id}
        //DELETE /api/cart/clear

        private readonly ICartService cartService;
        public CartController( ICartService _cartService)
        {
            cartService = _cartService;
        }

        [Authorize]
        [HttpGet("ViewCart")]
        public async Task<ActionResult> ViewCart()
        {
            var userId = User.FindFirstValue(JwtRegisteredClaimNames.Sub);
            //var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return NotFound(new { message = "User Cart not found." });
            var result = await cartService.ViewCart(userId);
            if (!result.IsSuccess)
            {
                int statusCode = result.StatusCode ?? 400;
                return StatusCode(statusCode,result.Error);
            }
            
            return Ok(result.Value);
        }
        
        [HttpPost("AddToCart")]
        public async Task<ActionResult> AddToCart(AddToCartDto addToCartDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await cartService.AddToCart(addToCartDto);
            if (!result.IsSuccess)
            {
                int statusCode = result.StatusCode ?? 400;
                return StatusCode( statusCode , result.Error );
            }

            return Ok(new { message = "Item added/updated successfully." });
        }

        [HttpPut("IncresItem{id:int}")]
        public async Task<ActionResult> IncresItem(int id)
        {
            var result = await cartService.IncresItem(id);
            if (!result.IsSuccess)
            {
                int statusCode = result.StatusCode ?? 400;
                return StatusCode(statusCode,result.Error);
            }
            
            return NoContent();
        }

        [HttpDelete("DeleteItem{id:int}")]
        public async Task<ActionResult> DeleteItem(int id)
        {
            var result = await cartService.DeleteItem(id);
            if (!result.IsSuccess)
            {
                int statusCode = result.StatusCode ?? 400;
                return StatusCode(statusCode,result.Error);
            }
            return NoContent();
        }

        [HttpDelete("DecriseItem{id:int}")]
        public async Task<ActionResult> DecriseItem(int id)
        {
            var result = await cartService.DecriseItem(id);
            if (!result.IsSuccess)
            {
                int statusCode = result.StatusCode ?? 400;
                return StatusCode(statusCode,result.Error);
            }
           
            return NoContent();
        }

        [Authorize]
        [HttpDelete("ClearItems")]
        public async Task<ActionResult> ClearItems()
        {
            var userId = User.FindFirstValue(JwtRegisteredClaimNames.Sub);
            //var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return NotFound(new { message = "User Cart not found." });

            var resultUser = await cartService.ClearItems(userId);
            if (!resultUser.IsSuccess)
            {
                int statusCode = resultUser.StatusCode ?? 400;
                return StatusCode(statusCode , resultUser.Error);
            }
            return NoContent();

        }

    }
}
