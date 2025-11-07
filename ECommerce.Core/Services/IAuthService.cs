using ECommerce.API.DTOs.Auth;
using ECommerce.Core.Entities;
using ECommerce.Core.Models;

namespace ECommerce.Core.Services
{
    public interface IAuthService
    {
        public Task<ResponseModel> Register(RegisterModel modal);
        public Task<ResponseModel> ResetPassword(ChangePasswordModel changePassword);
        public Task<ResponseModel> ForgotPassword(EmailDTO emailDTO);
        public Task<ResponseModel> VerifyEmail(VerifyModel model);
        public Task<AuthModel> Login(TokenRequestModel model);
        public Task<AuthModel> RefreshTokenAsync(string token);
        public Task<bool> RevokeTokenAsync(string token);

    }
}
