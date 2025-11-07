using ECommerce.API.DTOs.Auth;
using ECommerce.Core.Entities;
using ECommerce.Core.Models;
using ECommerce.Core.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly SignInManager<AppUser> signInManager;
        private readonly IAuthService _authService;

        public AuthController(SignInManager<AppUser> _signInManager,
            IAuthService authService)
        {
            signInManager = _signInManager;
            _authService = authService;
        }

        /// <summary>
        /// Registers a new user with the provided registration details.
        /// </summary>
        /// <remarks>This method validates the provided registration details, ensures the email is unique,
        /// and creates a new user account. If the registration is successful, an email containing a verification code
        /// is sent to the user's email address.</remarks>
        /// <param name="modal">An object containing the user's registration details, including email, password, and other required
        /// information.</param>
        /// <returns>An <see cref="IActionResult"/> indicating the result of the registration process. Returns <see
        /// cref="BadRequestObjectResult"/> if the model state is invalid, the email already exists, or user creation
        /// fails. Returns <see cref="OkObjectResult"/> with a success message if the registration is successful.</returns>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel modal)
        {
            if (!ModelState.IsValid)
            {
                 return BadRequest(ModelState);
            }
            var response =await _authService.Register(modal);
           
            if (!response.IsSuccess)
            {
                return BadRequest(response.Message);
            }

            return Ok(response.Message);
        }



        /// <summary>
        /// Verifies the email address of a user based on the provided verification code.
        /// </summary>
        /// <remarks>This method checks the validity of the provided verification code and ensures that
        /// the code has not expired.  If the verification is successful, the user's email is marked as confirmed, and
        /// the verification code is cleared.</remarks>
        /// <param name="model">An instance of <see cref="VerifyModel"/> containing the user's email address and verification code.</param>
        /// <returns>Returns an <see cref="IActionResult"/> indicating the result of the verification process: <list
        /// type="bullet"> <item><description><see cref="BadRequestObjectResult"/> if the model state is invalid, the
        /// email is already confirmed, the verification code is invalid, or the code has expired.</description></item>
        /// <item><description><see cref="NotFoundObjectResult"/> if no user is found with the specified email
        /// address.</description></item> <item><description><see cref="OkObjectResult"/> if the email is successfully
        /// verified.</description></item> </list></returns>
        [HttpPost("verifyEmail")]
        public async Task<IActionResult> VerifyEmail([FromBody] VerifyModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _authService.VerifyEmail(model);
           if (!result.IsSuccess)
            {
               return BadRequest(result.Message);
            }
            return Ok(result.Message);
        }


        //[HttpPost("login")]
        //public async Task<IActionResult> Login([FromBody] LoginDto modal)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    var user=await userManager.FindByEmailAsync(modal.Email);
        //    if (user == null)
        //    {
        //        return BadRequest(new { message = "Invalid login attempt" });
        //    }

        //    var check = await userManager.CheckPasswordAsync(user,modal.Password);
        //    if (!check)
        //    {
        //        return BadRequest("Invalid login attempt");
        //    }

        //    if (!user.EmailConfirmed)
        //    {
        //        return Unauthorized(new { message = "Please confirm your email address." });
        //    }

        //    List<Claim> claims = new List<Claim>()
        //    {
        //        new Claim(ClaimTypes.NameIdentifier, user.Id),
        //        new Claim(ClaimTypes.Email, user.Email!),
        //    };

        //    var Roles = await userManager.GetRolesAsync(user);
        //    foreach (var role in Roles)
        //    {
        //        claims.Add(new Claim(ClaimTypes.Role, role));
        //    }
        //    //await userManager.AddClaimAsync(user, claims[0]);

        //    var issuer = configuration["JwtSettings:Issuer"];
        //    var audience = configuration["JwtSettings:Audience"];
        //    var secretKey = configuration["JwtSettings:Key"];
        //    var expiryDate = configuration["JwtSettings:Duration"];

        //SymmetricSecurityKey securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey!));

        //SigningCredentials credentials =new SigningCredentials(securityKey,SecurityAlgorithms.HmacSha256);

        //    JwtSecurityToken jwtSecurity = new JwtSecurityToken(
        //        issuer: issuer,
        //        audience: audience,
        //        claims: claims,
        //        expires: DateTime.UtcNow.AddHours(double.Parse(expiryDate!)),
        //        signingCredentials: credentials
        //        );

        //    Token  token=new Token { 
        //        token=new JwtSecurityTokenHandler().WriteToken(jwtSecurity), 
        //        expires= jwtSecurity.ValidTo 
        //    };
        //    return Ok(token);
        //}

        /// <summary>
        /// Authenticates a user based on the provided credentials and returns an authentication token.
        /// </summary>
        /// <remarks>If the authentication is successful and a refresh token is generated, it is stored in
        /// a secure HTTP-only cookie.</remarks>
        /// <param name="model">The <see cref="TokenRequestModel"/> containing the user's login credentials.</param>
        /// <returns>An <see cref="IActionResult"/> containing the authentication result.  Returns a 200 OK response with the
        /// authentication details if successful,  or a 400 Bad Request response with an error message if authentication
        /// fails or the model is invalid.</returns>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] TokenRequestModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.Login(model);

            if (!result.IsAuthenticated)
                return BadRequest(result.Message);

            if (!string.IsNullOrEmpty(result.RefreshToken))
                SetRefreshTokenInCookie(result.RefreshToken, result.RefreshTokenExpiration);

            return Ok(result);
        }

        /// <summary>
        /// Initiates the password reset process by generating a reset token and sending a reset link to the specified
        /// email address.
        /// </summary>
        /// <remarks>This method does not disclose whether the email address is registered in the system
        /// for security reasons. If the email address is registered, a password reset link is sent to the provided
        /// email address. The reset link includes a token and user ID, and its format depends on the client type (e.g.,
        /// mobile or web).</remarks>
        /// <param name="emailDTO">An object containing the email address of the user requesting the password reset, along with additional
        /// client-specific information.</param>
        /// <returns>An <see cref="IActionResult"/> indicating the result of the operation. Returns: <list type="bullet"> <item>
        /// <description><see cref="OkObjectResult"/> with a confirmation message if the email is registered or
        /// unregistered.</description> </item> <item> <description><see cref="BadRequestObjectResult"/> if the provided
        /// model state is invalid.</description> </item> </list></returns>
        [HttpPost("forgotPassword")]
        public async Task<IActionResult> ForgotPassword([FromBody] EmailDTO emailDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _authService.ForgotPassword(emailDTO);
            return Ok(result.Message);
        }

        /// <summary>
        /// Resets the password for a user based on the provided token and new password.
        /// </summary>
        /// <remarks>The reset token must be a valid Base64Url-encoded string. If the token is invalid or
        /// the password reset fails,  the method returns detailed error information in the response.</remarks>
        /// <param name="changePassword">An instance of <see cref="ChangePasswordModel"/> containing the user ID, reset token, and new password.</param>
        /// <returns>An <see cref="IActionResult"/> indicating the result of the operation. Returns: <list type="bullet">
        /// <item><description><see cref="OkObjectResult"/> if the password is successfully reset.</description></item>
        /// <item><description><see cref="BadRequestObjectResult"/> if the model state is invalid, the user is not
        /// found, or the reset operation fails.</description></item> </list></returns>
        [HttpPost("resetPassword")]
        public async Task<IActionResult> ResetPassword([FromBody]ChangePasswordModel changePassword )
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _authService.ResetPassword(changePassword);
           
            if (!result.IsSuccess)
            {
                return BadRequest(result.Message);
            }

            return Ok(result.Message);
        }

        #region Tokens EndPoints

        /// <summary>
        /// Refreshes the authentication token for the current user.
        /// </summary>
        /// <remarks>This method validates the request origin and the provided refresh token before
        /// generating a new authentication token.  If the request origin is not trusted or the refresh token is
        /// invalid, the request is denied.</remarks>
        /// <returns>An <see cref="IActionResult"/> containing the result of the token refresh operation.  Returns <see
        /// cref="ForbidResult"/> if the request origin is invalid, <see cref="BadRequestResult"/> if the refresh token
        /// is invalid,  or <see cref="OkObjectResult"/> with the authentication result if the operation is successful.</returns>
        [HttpPost("refreshToken")]
        public async Task<IActionResult> RefreshToken()
        {
            var origin = Request.Headers["Origin"].ToString();

            if (string.IsNullOrEmpty(origin) || !IsTrustedDomain(origin))
            {
                return Forbid("Access denied. Invalid request origin.");
            }
            var refreshToken = Request.Cookies["refreshToken"];

            var result = await _authService.RefreshTokenAsync(refreshToken);

            if (!result.IsAuthenticated)
                return BadRequest(result);

            SetRefreshTokenInCookie(result.RefreshToken, result.RefreshTokenExpiration);

            return Ok(result);
        }
       
        /// <summary>
        /// Revokes a specified token, invalidating it for future use.
        /// </summary>
        /// <remarks>This method invalidates the provided token by calling the authentication service. If
        /// the token is successfully revoked, the "refreshToken" cookie is deleted from the response.</remarks>
        /// <param name="model">An object containing the token to be revoked. If the token is not provided in the request body, the method
        /// attempts to retrieve it from the "refreshToken" cookie.</param>
        /// <returns>An <see cref="IActionResult"/> indicating the result of the operation. Returns <see
        /// cref="BadRequestObjectResult"/> if the token is missing or invalid, or <see cref="OkObjectResult"/> if the
        /// token is successfully revoked.</returns>
        [HttpPost("revokeToken")]
        public async Task<IActionResult> RevokeToken([FromBody] RevokeToken model)
        {
            var token = model.Token ?? Request.Cookies["refreshToken"];

            if (string.IsNullOrEmpty(token))
                return BadRequest("Token is required!");

            var result = await _authService.RevokeTokenAsync(token);

            if (!result)
                return BadRequest("Token is invalid!");

           Response.Cookies.Delete("refreshToken"); 
           return Ok("Token revoked successfully.");
        }

        #endregion

        /// <summary>
        /// Logs the current user out of the application.
        /// </summary>
        /// <remarks>This method terminates the user's session by signing them out and invalidating their
        /// authentication state. After the logout operation, no content is returned in the response.</remarks>
        /// <returns>An <see cref="IActionResult"/> indicating the result of the logout operation.  Always returns <see
        /// cref="NoContentResult"/> to signify a successful logout.</returns>
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            var token = Request.Cookies["refreshToken"];

            // 3. التحقق من وجوده وإلغائه
            if (!string.IsNullOrEmpty(token))
            {
                // استدعاء خدمة إلغاء التوكن التي تلغيه في قاعدة البيانات
                var result = await _authService.RevokeTokenAsync(token);
                Response.Cookies.Delete("refreshToken");
            }

            // 5. إرجاع استجابة نجاح (حتى لو لم يكن هناك توكن للحذف)
            return NoContent();
        }

        #region SetRefreshTokenInCookie
        private void SetRefreshTokenInCookie(string refreshToken, DateTime expires)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = expires.ToUniversalTime(),
                Secure = true,
                IsEssential = true,
                SameSite = SameSiteMode.None
            };

            Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);
        }

        private bool IsTrustedDomain(string origin)
        {
            var allowedDomains = new List<string> {
                "https://frontend.yourdomain.com",//  ابق غيره بعدين 
                "http://localhost:4200"
            };
            return allowedDomains.Any(d => origin.StartsWith(d, StringComparison.OrdinalIgnoreCase));
        }
        
        #endregion

    }
}
