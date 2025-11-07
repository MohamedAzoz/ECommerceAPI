using AutoMapper;
using ECommerce.API.DTOs.Auth;
using ECommerce.Core.Entities;
using ECommerce.Core.Helper;
using ECommerce.Core.Interfaces;
using ECommerce.Core.Models;
using ECommerce.Core.Result_Pattern;
using ECommerce.Core.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<AppUser> userManager;
        private readonly IUnitOfWork unitOfWork;
        private readonly IEmailVerification emailVerification;
        private readonly IConfiguration configuration;
        private readonly IMapper mapper;
        private readonly JWT _jwt;

        public AuthService(UserManager<AppUser> _userManager,
            IUnitOfWork _unitOfWork,
            IEmailVerification _emailVerification,
            IConfiguration _configuration,
            IMapper mapper,
            IOptions<JWT> jwt)
        {
            userManager = _userManager;
            unitOfWork = _unitOfWork;
            emailVerification = _emailVerification;
            configuration = _configuration;
            this.mapper = mapper;
            _jwt = jwt.Value;
        }

        public async Task<ResponseModel> Register(RegisterModel modal)
        {
            ResponseModel response = new ResponseModel();
            var UserFound = await userManager.FindByEmailAsync(modal.Email);
            if (UserFound != null)
            {
                response.Message = "Email already exists";
                response.IsSuccess = false;
                return response;
            }
            var code = GenerateSixDigitCodeSecure();
            AppUser user = mapper.Map<AppUser>(modal);

            user.EmailVerificationCode = code;
            user.EmailVerificationCodeExpiry = DateTime.UtcNow.AddHours(1);

            IdentityResult result = await userManager.CreateAsync(user, modal.Password);
            if (!result.Succeeded)
            {
                response.Message = $"Register: ";
                foreach (var item in result.Errors)
                {
                    response.Message += $"{item.Description} , ";
                }
                response.IsSuccess = false;
                return response;
            }


            await userManager.AddToRoleAsync(user, "User"); // <===
            await emailVerification.SendEmailVerificationAsync(
                                    user.Email!,
                                    "Verify your email",
                                    $"Your code is: {code}");


            response.Message = "User Registered. Please check your email for the verification code.";
            response.IsSuccess = true;
            return response;
        }

        private string GenerateSixDigitCodeSecure()
        {
            byte[] bytes = new byte[4];
            int value;
            do
            {
                RandomNumberGenerator.Fill(bytes);
                value = BitConverter.ToInt32(bytes, 0) & int.MaxValue;
            }
            while (value < 100000 || value > 999999);

            return value.ToString().Substring(0, 6);
        }


        public async Task<ResponseModel> ForgotPassword(EmailDTO emailDTO)
        {
            ResponseModel response = new ResponseModel();
            var user = await userManager.FindByEmailAsync(emailDTO.Email);
            if (user == null)
            {
                response.Message="If the email is registered, a password reset link has been sent.";
                response.IsSuccess = true;
                return response;
            }
            var generatToken = await userManager.GeneratePasswordResetTokenAsync(user);
            var TokenBytes = Encoding.UTF8.GetBytes(generatToken);
            var TokenUrlSave = WebEncoders.Base64UrlEncode(TokenBytes);
            var resetLink = "";

            const string MobileClientType = "Mobile";
            if (emailDTO.ClientType.Equals(MobileClientType, StringComparison.OrdinalIgnoreCase))
            {
                resetLink = $"{emailDTO.Scheme}://{emailDTO.Page}?userId={user.Id}&token={TokenUrlSave}";
            }
            else
            {
                var linkURL = emailDTO.LinkURL!.TrimEnd('/');
                resetLink = $"{linkURL}{emailDTO.Page}?userId={user.Id}&token={TokenUrlSave}";
            }

            // ✅ إرسال الإيميل
            await emailVerification.SendEmailVerificationAsync(user.Email!, "Reset Password",
                $@"Click this link to reset your password: 
                        <html>
                          <body style='font-family: Arial, sans-serif; line-height:1.6; background-color:#f9f9f9; padding:20px;'>
                            <div style='max-width:600px; margin:auto; background:#fff; padding:20px; border-radius:8px; box-shadow:0 2px 8px rgba(0,0,0,0.1);'>
             
                              <p style='text-align:center; margin:30px 0;'>
                                <a href='{resetLink}' 
                                   style='background-color:#4CAF50; color:white; padding:12px 24px; text-decoration:none; 
                                          border-radius:6px; font-weight:bold; display:inline-block;'>
                                   Reset Password
                                </a>
                              </p>
                              <p style='font-size:12px; color:#999; text-align:center;'>
                                If you didn’t request this, you can ignore this email.
                              </p>
                            </div>
                          </body>
                        </html>");
            response.Message = "If the email is registered, a password reset link has been sent.";
            response.IsSuccess = true;
            return response;
        }

        public async Task<ResponseModel> ResetPassword( ChangePasswordModel changePassword)
        {
            ResponseModel response = new ResponseModel();
            var User = await userManager.FindByIdAsync(changePassword.UserId);
            if (User == null)
            {
                response.Message="Failed to reset password. Please try again.";
                response.IsSuccess = false;
                return response;
            }
            var tokenFromClient = changePassword.Token; 
            byte[] tokenBytes = WebEncoders.Base64UrlDecode(tokenFromClient);
            string OriginalToken = System.Text.Encoding.UTF8.GetString(tokenBytes);
            var result = await userManager.ResetPasswordAsync(User, OriginalToken, changePassword.Password);
            if (!result.Succeeded)
            {
                response.Message = $"ResetPassword: ";
                foreach (var item in result.Errors)
                {
                    response.Message += $"{item.Description} , ";
                }
                response.IsSuccess = false;
                return response;
            }

            response.Message = "Password has been reset successfully.";
            response.IsSuccess = true;
            
            return response;
        }

        public async Task<ResponseModel> VerifyEmail(VerifyModel model)
        {
            ResponseModel response = new ResponseModel();
            
            var UserFound = await userManager.FindByEmailAsync(model.Email);
            if (UserFound == null)
            {
                response.Message = "User not found.";
                response.IsSuccess=false;
                return response;
            }

            if (UserFound.EmailConfirmed)
            {
                response.Message = "Email is already confirmed.";
                response.IsSuccess = false;
                return response;
            }

            if (UserFound.EmailVerificationCode != model.Code)
            {
                response.Message = "Invalid code or email.";
                response.IsSuccess = false;
                return response;
            }

            if (UserFound.EmailVerificationCodeExpiry < DateTime.UtcNow)
            {
                response.Message = "Verification code has expired.";
                response.IsSuccess = false;
                return response;
            }

            UserFound.EmailConfirmed = true;
            UserFound.EmailVerificationCode = null;
            UserFound.EmailVerificationCodeExpiry = null;
            Cart cart = new Cart() { UserId = UserFound.Id };
            var cartAddResult = await unitOfWork.Carts.AddAsync(cart);

            if (!cartAddResult.IsSuccess)
            {
                response.Message = "Failed to create code verification during verification.";
                response.IsSuccess = false;
                return response;
            }
            await unitOfWork.Completed();
            UserFound.Cart = cart;
            UserFound.CartId = cart.Id;

            var result = await userManager.UpdateAsync(UserFound);
            if (!result.Succeeded)
            {
                response.Message = $"VerifyEmail: ";
                foreach (var item in result.Errors)
                {
                    response.Message += $"{item.Description} , ";
                }
                response.IsSuccess = false;
                return response;
            }

            response.Message = "Email verified successfully.";
            response.IsSuccess = true;
            return response;
        }



        public async Task<AuthModel> Login(TokenRequestModel model)
        {
            var authModel = new AuthModel();

            var user = await userManager.FindByEmailAsync(model.Email);

            if (user == null)
            {
                authModel.Message = "Invalid login attempt";
                return authModel;
            }

            if (!await userManager.CheckPasswordAsync(user, model.Password))
            {
                authModel.Message = "Invalid login attempt";
                return authModel;
            }

            if (!user.EmailConfirmed)
            {
                authModel.Message = "Please confirm your email address.";
                return authModel;
            }

            var jwtSecurityToken = await CreateJwtToken(user);
            var rolesList = await userManager.GetRolesAsync(user);

            authModel.IsAuthenticated = true;
            authModel.Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
            authModel.Email = user.Email;
            authModel.Username = user.UserName;
            authModel.ExpiresOn = jwtSecurityToken.ValidTo;
            authModel.Roles = rolesList.ToList();

            foreach (var token in user.RefreshTokens.Where(t => t.IsActive))
            {
                token.RevokedOn = DateTime.UtcNow;
            }

            var refreshToken = GenerateRefreshToken();
            authModel.RefreshToken = refreshToken.Token;
            authModel.RefreshTokenExpiration = refreshToken.ExpiresOn;
            user.RefreshTokens.Add(refreshToken);
            await userManager.UpdateAsync(user);
            

            return authModel;
        }
     
        #region
        private async Task<JwtSecurityToken> CreateJwtToken(AppUser user)
        {
            var userClaims = await userManager.GetClaimsAsync(user);
            var roles = await userManager.GetRolesAsync(user);
            var roleClaims = new List<Claim>();

            foreach (var role in roles)
                roleClaims.Add(new Claim(ClaimTypes.Role, role));

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email!),
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.Name, user.UserName!)
            }
            .Union(userClaims)
            .Union(roleClaims);

            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _jwt.Issuer,
                audience: _jwt.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddDays(_jwt.Duration),
                signingCredentials: signingCredentials
                );

            return jwtSecurityToken;
        }

        //******************************

        public async Task<AuthModel> RefreshTokenAsync(string token)
        {
            var authModel = new AuthModel();

            var user = await userManager.Users.SingleOrDefaultAsync(u => u.RefreshTokens.Any(t => t.Token == token));

            if (user == null)
            {
                authModel.Message = "Invalid token";
                return authModel;
            }

            var refreshToken = user.RefreshTokens.Single(t => t.Token == token);

            if (!refreshToken.IsActive)
            {
                authModel.Message = "Inactive token";
                return authModel;
            }

            refreshToken.RevokedOn = DateTime.UtcNow;

            var newRefreshToken = GenerateRefreshToken();
            user.RefreshTokens.Add(newRefreshToken);
            await userManager.UpdateAsync(user);

            var jwtToken = await CreateJwtToken(user);
            authModel.IsAuthenticated = true;
            authModel.Token = new JwtSecurityTokenHandler().WriteToken(jwtToken);
            authModel.Email = user.Email;
            authModel.Username = user.UserName;
            var roles = await userManager.GetRolesAsync(user);
            authModel.Roles = roles.ToList();
            authModel.RefreshToken = newRefreshToken.Token;
            authModel.RefreshTokenExpiration = newRefreshToken.ExpiresOn;

            return authModel;
        }


        public async Task<bool> RevokeTokenAsync(string token)
        {
            var user = await userManager.Users.SingleOrDefaultAsync(u => u.RefreshTokens.Any(t => t.Token == token));

            if (user == null)
                return false;

            var refreshToken = user.RefreshTokens.Single(t => t.Token == token);

            if (!refreshToken.IsActive)
                return false;

            refreshToken.RevokedOn = DateTime.UtcNow;

            await userManager.UpdateAsync(user);

            return true;
        }

        private RefreshToken GenerateRefreshToken()
        {
            var randomNumber = new byte[32];

            RandomNumberGenerator.Fill(randomNumber);

            return new RefreshToken
            {
                Token = Convert.ToBase64String(randomNumber),
                ExpiresOn = DateTime.UtcNow.AddDays(10),
                CreatedOn = DateTime.UtcNow
            };
        }

        #endregion
    }
}
