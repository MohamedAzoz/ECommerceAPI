using AutoMapper;
using ECommerce.Core.DTOs.Address;
using ECommerce.Core.Entities;
using ECommerce.Core.Interfaces;
using ECommerce.Core.Result_Pattern;
using ECommerce.Core.Services;

namespace ECommerce.Infrastructure.Services
{
    public class AddressService : IAddressService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public AddressService(IUnitOfWork _unitOfWork, IMapper _mapper)
        {
            unitOfWork = _unitOfWork;
            mapper = _mapper;
        }

        public async Task<Result<ICollection<Address>>> GetAll()
        {
            var result = await unitOfWork.Addresses.GetAllAsync();
            if (!result.IsSuccess)
            {
                return Result<ICollection<Address>>.Failure("An error occurred while retrieving addresses.",500);
            }
            return Result<ICollection<Address>>.Success(result.Value);
        }

        public async Task<Result<Address>> GetById(int id)
        {
            var result = await unitOfWork.Addresses.FindAsync((x) => x.Id == id);
            if (!result.IsSuccess)
            {
                if (result.Error!=null)
                {
                    return Result<Address>.Failure(result.Error);
                }
                    return Result<Address>.Failure("Address not found", 404);
            }
            return Result<Address>.Success(result.Value);
        }

        public async Task<Result<Address>> GetByUserId(string userId)
        {
           var result = await unitOfWork.Addresses.FindAsync((x) => x.UserId == userId);
            if (!result.IsSuccess)
            {
                if (result.Error != null)
                {
                    return Result<Address>.Failure(result.Error);
                }
                return Result<Address>.Failure("Address not found.", 404);
            }
            return Result<Address>.Success(result.Value);
        }

        public async Task<Result<Address>> AddAddress(AddressDto addressDto)
        {
            Address address = mapper.Map<Address>(addressDto);
            var result = await unitOfWork.Addresses.AddAsync(address);
            if (!result.IsSuccess)
            {
                return Result<Address>.Failure(result.Error,500);
            }
            await unitOfWork.Completed();
            return Result<Address>.Success(result.Value);
        }

        public async Task<Result<Address>> Update(AddressDto addressDto)
        {
            var result = (await unitOfWork.Addresses.FindAsync((x) =>
                                        x.UserId == addressDto.UserId &&
                                        x.City == addressDto.City &&
                                        x.Street == addressDto.Street));
            if (!result.IsSuccess)
            {
                return Result<Address>.Failure("Address not found matching the criteria.",404);
            }
            Address address = result.Value;
            var resultUpdate = unitOfWork.Addresses.Update(address);
            if (!resultUpdate.IsSuccess)
            {
                return Result<Address>.Failure(resultUpdate.Error,500);
            }
            await unitOfWork.Completed();
            return Result<Address>.Success(resultUpdate.Value);
        }

        public async Task<Result> Delete(int id)
        {
            var result = await unitOfWork.Addresses.FindAsync((x) => x.Id == id);
            if (!result.IsSuccess)
            {
                if (result.Error!=null)
                {
                    return Result.Failure(result.Error);
                }
                return Result.Failure("Address not found.",404);
            }
            Address address = result.Value;
            var Delresult = unitOfWork.Addresses.Delete(address);
            if (!Delresult.IsSuccess)
            {
                return Result.Failure(Delresult.Error,500);
            }
            await unitOfWork.Completed();
            return Result.Success();
        }

    }
}
