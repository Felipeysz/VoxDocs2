using VoxDocs.DTO;

namespace VoxDocs.Services
{
    public interface IUserBusinessRules
    {
        Task ValidateUniqueUserAsync(DTORegisterUser registerDto);
        Task ValidateUserExistsAsync(Guid userId);
        Task<(int admins, int users)> ValidatePlanLimitAsync(DTORegisterUser registerDto);
        Task ValidatePlanLimitForUpdateAsync(DTOUpdateUser updateDto);
    }
}