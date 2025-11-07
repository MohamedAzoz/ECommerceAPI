using ECommerce.Core.DTOs.Address;
using ECommerce.Core.Entities;
using ECommerce.Core.Result_Pattern;

namespace ECommerce.Core.Services
{
    public interface IAddressService
    {
        public Task<Result<Address>> AddAddress(AddressDto addressDto);
        public Task<Result<Address>> Update(AddressDto addressDto);
        public Task<Result<ICollection<Address>>> GetAll();
        public Task<Result<Address>> GetByUserId(string userId);
        public Task<Result<Address>> GetById(int id);
        public Task<Result> Delete(int id);
    }
}
