using EcommerceApp.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace EcommerceApp.Application.Interfaces
{
    public interface IAuthService
    {
        Task<LoginResponseDto> RegisterAsync(RegisterDto registerDto);
        Task<LoginResponseDto> LoginAsync(LoginDto loginDto);
        Task<UserDto> GetCurrentUserAsync(string userId);
    }
}
