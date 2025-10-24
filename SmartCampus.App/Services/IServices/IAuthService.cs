using SmartCampus.App.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartCampus.App.Services.IServices
{
    public interface IAuthService
    {
        Task<string?> LoginAsync(LoginDto dto);
        Task LogoutAsync();
    }
}
