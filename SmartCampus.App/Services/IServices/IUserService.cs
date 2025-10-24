using SmartCampus.App.DTOs;
using SmartCampus.Core.Entities;

namespace SmartCampus.App.Services.IServices
{
    public interface IUserService
    {
        Task<AppUser> CreateUserAsync(CreateUserDto dto);
    }
}
