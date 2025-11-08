using ECommerce.Core.DTOs.Order;
using ECommerce.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ECommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        //GET    /api/orders  ==> (ok)
        //GET    /api/orders/{id}   ==> (ok)
        //POST   /api/orders ==> (ok)
        //PUT    /api/orders/{id}/status (Admin)  ==> X
        //GET    /api/orders/user/{userId} ==> (ok)

        private readonly IOrderService orderService;
        public OrdersController (IOrderService _orderService)
        {
            orderService = _orderService;
        }

        [HttpGet]
        public async Task<ActionResult> GetAll()
        {
            var result =await orderService.GetAll();
            if (!result.IsSuccess)
            {
                int statusCode = result.StatusCode ?? 400;
                return StatusCode(statusCode, result.Error);
            }
            return Ok(result.Value);
        }

        [Authorize]
        [HttpGet("GetByUserId")]
        public async Task<ActionResult> GetByUserId()
        {
            var userId = User.FindFirstValue(JwtRegisteredClaimNames.Sub);
            //var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return NotFound(new { message = "User Cart not found." });
            var result =await orderService.GetByUserId(userId);
            if (!result.IsSuccess)
            {
                int statusCode = result.StatusCode ?? 400;
                return StatusCode(statusCode, result.Value);
            }

            return Ok(result.Value);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult> GetById(int id)
        {
            var result =await orderService.GetById(id);
            if (!result.IsSuccess)
            {
                int statusCode = result.StatusCode ?? 400;
                return StatusCode(statusCode, result.Error);
            }
            return Ok(result.Value);
        }

        [Authorize]
        [HttpPost("NewOrder")]
        public async Task<ActionResult> NewOrder([FromBody] CreateOrderDto orderDto)
        {
            //var userId = User.FindFirstValue(JwtRegisteredClaimNames.Sub);
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return NotFound("User Cart not found.");
            var result = await orderService.NewOrder(orderDto, userId);
            if (!result.IsSuccess)
            {
                int statusCode = result.StatusCode ?? 400;
                return StatusCode(statusCode,result.Error);
            }
            return Created();
        }

        [Authorize]
        [HttpPut("{id:int}/status")]
        public async Task<ActionResult> UpdateStatus(UpdateOrderStatusDto orderStatusDto)
        {
            var result =await orderService.UpdateStatus(orderStatusDto);
            if (!result.IsSuccess)
            {
                int statusCode = result.StatusCode ?? 400;
                return StatusCode(statusCode,result.Error);
            }
            
            return Ok(result.Value);
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var result =await orderService.Delete(id);
            if (!result.IsSuccess)
            {
                int statusCode = result.StatusCode ?? 400;
                return StatusCode(statusCode,result.Error);
            }
            
            return NoContent();
        }

    }
}
