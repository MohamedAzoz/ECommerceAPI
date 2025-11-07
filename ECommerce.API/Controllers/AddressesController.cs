using ECommerce.Core.DTOs.Address;
using ECommerce.Core.Entities;
using ECommerce.Core.Result_Pattern;
using ECommerce.Core.Services;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ECommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AddressesController : ControllerBase
    {
        private readonly IAddressService addressService;

        public AddressesController(IAddressService _addressService)
        {
            addressService = _addressService;
        }

        /// <summary>
        /// Retrieves all addresses from the data source.
        /// </summary>
        /// <remarks>This method performs an asynchronous operation to fetch all address records. If the
        /// operation fails, a 500 Internal Server Error response is returned.</remarks>
        /// <returns>An <see cref="ActionResult"/> containing an HTTP 200 OK response with the list of addresses if the operation
        /// is successful, or an HTTP 500 Internal Server Error response if an error occurs.</returns>
        [HttpGet]
        public async Task<ActionResult> GetAll()
        {
            var result =await addressService.GetAll();
            if (!result.IsSuccess)
            {
                int statusCode = result.StatusCode ?? 400;
                return StatusCode(statusCode, result.Error);
            }
            return Ok(result.Value);            
        }
       
        /// <summary>
        /// Retrieves an address by its unique identifier.
        /// </summary>
        /// <remarks>This method performs an asynchronous operation to fetch the address from the data
        /// source. Ensure the provided <paramref name="id"/> is valid and corresponds to an existing address.</remarks>
        /// <param name="id">The unique identifier of the address to retrieve. Must be a positive integer.</param>
        /// <returns>An <see cref="ActionResult"/> containing the address if found, or a <see cref="NotFoundResult"/> if no
        /// address with the specified identifier exists.</returns>
        [HttpGet("{id:int}")]
        public async Task<ActionResult> GetById(int id)
        {
            var result =await addressService.GetById(id);
            if (!result.IsSuccess)
            {
                int statusCode = result.StatusCode ?? 400;
                return StatusCode(statusCode, result.Error);
            }
            return Ok(result.Value);
        }
        /// <summary>
        /// Retrieves the address associated with the specified user ID.
        /// </summary>
        /// <remarks>This method performs a lookup for the address based on the provided user ID. If no
        /// address is found, a 404 Not Found response is returned. Ensure that the <paramref name="userId"/> is valid
        /// and corresponds to an existing user in the system.</remarks>
        /// <param name="userId">The unique identifier of the user. This must be an alphanumeric string.</param>
        /// <returns>An <see cref="ActionResult"/> containing the address associated with the specified user ID if found;
        /// otherwise, a <see cref="NotFoundResult"/> with an error message.</returns>
        [HttpGet("GetByUserId")]
        public async Task<ActionResult> GetByUserId()
        {
            var userId = User.FindFirstValue(JwtRegisteredClaimNames.Sub);
            //var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return NotFound("User not found.");

            var result =await addressService.GetByUserId(userId);
            if (!result.IsSuccess)
            {
                int statusCode = result.StatusCode ?? 400;
                return StatusCode(statusCode, result.Error);
            }
            return Ok(result.Value);
        }

        /// <summary>
        /// Adds a new address to the system.
        /// </summary>
        /// <remarks>This method validates the provided address data, maps it to the domain model, and
        /// attempts to add it to the database.  Ensure that the <paramref name="addressDto"/> contains valid data
        /// before calling this method.</remarks>
        /// <param name="addressDto">The data transfer object containing the details of the address to be added.</param>
        /// <returns>An <see cref="ActionResult"/> indicating the result of the operation.  Returns <see cref="BadRequest"/> if
        /// the model state is invalid or the operation fails.  Returns <see cref="Created"/> if the address is
        /// successfully added.</returns>
        [HttpPost]
        public async Task<ActionResult> AddAddress(AddressDto addressDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result =await addressService.AddAddress(addressDto);
            if (!result.IsSuccess)
            {
                int statusCode = result.StatusCode ?? 400;
                return StatusCode(statusCode, result.Error);
            }
            return Created();
        }

        /// <summary>
        /// Updates an existing address based on the provided address details.
        /// </summary>
        /// <remarks>This method performs a partial update of an address. The address is identified by the
        /// combination of <see cref="AddressDto.UserId"/>, <see cref="AddressDto.City"/>, and <see
        /// cref="AddressDto.State"/>. Ensure that the provided <paramref name="addressDto"/> contains valid
        /// data.</remarks>
        /// <param name="addressDto">The data transfer object containing the updated address details. The <see cref="AddressDto.UserId"/>
        /// identifies the user, and the combination of <see cref="AddressDto.City"/> and <see cref="AddressDto.State"/>
        /// identifies the address to update.</param>
        /// <returns>An <see cref="ActionResult"/> indicating the result of the operation: <list type="bullet">
        /// <item><description><see cref="BadRequestObjectResult"/> if the model state is invalid or the update
        /// operation fails.</description></item> <item><description><see cref="NotFoundObjectResult"/> if no address
        /// matching the specified criteria is found.</description></item> <item><description><see
        /// cref="OkObjectResult"/> containing the updated address if the operation succeeds.</description></item>
        /// </list></returns>
        [HttpPatch]
        public async Task<ActionResult> Update(AddressDto addressDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await addressService.Update(addressDto);
            if (!result.IsSuccess)
            {
                int statusCode = result.StatusCode ?? 400;
                return StatusCode(statusCode, result.Error);
            }
            return Ok(result.Value);
        }

        /// <summary>
        /// Deletes the address with the specified identifier.
        /// </summary>
        /// <remarks>This method performs a soft delete operation by removing the specified address from
        /// the data store. Ensure that the <paramref name="id"/> corresponds to an existing address before calling this
        /// method.</remarks>
        /// <param name="id">The unique identifier of the address to delete.</param>
        /// <returns>A <see cref="Task{ActionResult}"/> representing the asynchronous operation. Returns <see
        /// cref="NoContentResult"/> if the deletion is successful, <see cref="NotFoundObjectResult"/> if the address
        /// with the specified identifier is not found, or <see cref="BadRequestObjectResult"/> if an error occurs
        /// during deletion.</returns>
        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
           var result =await addressService.Delete(id);
            if (!result.IsSuccess)
            {
                int statusCode = result.StatusCode ?? 400;
                return StatusCode(statusCode, result.Error);
            }
            return NoContent();
        }

    }
}
