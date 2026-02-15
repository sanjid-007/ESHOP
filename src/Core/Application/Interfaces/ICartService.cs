using EcommerceApp.Application.DTOs;
using EcommerceApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace EcommerceApp.Application.Interfaces
{
    public interface ICartService
    {
        Task<CartDto> GetUserCartAsync(Guid userId);
        Task<CartDto> AddToCartAsync(Guid userId, AddToCartDto addToCartDto);
        Task<CartDto> UpdateCartItemAsync(Guid userId, Guid cartItemId, UpdateCartItemDto updateDto);
        Task<bool> RemoveFromCartAsync(Guid userId, Guid cartItemId);
        Task<bool> ClearCartAsync(Guid userId);

    }
}
