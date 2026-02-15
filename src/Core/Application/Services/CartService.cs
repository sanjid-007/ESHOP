using EcommerceApp.Application.DTOs;
using EcommerceApp.Application.Interfaces;


namespace EcommerceApp.Application.Services
{
    public class CartService : ICartService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CartService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<CartDto> GetUserCartAsync(Guid userId)
        {
            var carts = await _unitOfWork.Carts.FindAsync(c => c.UserId == userId);
            var cart = carts.FirstOrDefault();

            if (cart == null)
            {
                cart = new Cart
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    CreatedAt = DateTime.UtcNow
                };
                await _unitOfWork.Carts.AddAsync(cart);
                await _unitOfWork.SaveChangesAsync();
            }

            return await MapToCartDto(cart);
        }

        public async Task<CartDto> AddToCartAsync(Guid userId, AddToCartDto addToCartDto)
        {
            var carts = await _unitOfWork.Carts.FindAsync(c => c.UserId == userId);
            var cart = carts.FirstOrDefault();

            if (cart == null)
            {
                cart = new Cart
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    CreatedAt = DateTime.UtcNow
                };
                await _unitOfWork.Carts.AddAsync(cart);
                await _unitOfWork.SaveChangesAsync();
            }

            var product = await _unitOfWork.Products.GetByIdAsync(addToCartDto.ProductId);
            if (product == null)
                throw new Exception("Product not found");

            if (product.StockQuantity < addToCartDto.Quantity)
                throw new Exception("Insufficient stock");

            var cartItems = await _unitOfWork.CartItems.FindAsync(ci =>
                ci.CartId == cart.Id && ci.ProductId == addToCartDto.ProductId);
            var existingItem = cartItems.FirstOrDefault();

            if (existingItem != null)
            {
                existingItem.Quantity += addToCartDto.Quantity;
                await _unitOfWork.CartItems.UpdateAsync(existingItem);
            }
            else
            {
                var cartItem = new CartItem
                {
                    Id = Guid.NewGuid(),
                    CartId = cart.Id,
                    ProductId = addToCartDto.ProductId,
                    Quantity = addToCartDto.Quantity,
                    AddedAt = DateTime.UtcNow
                };
                await _unitOfWork.CartItems.AddAsync(cartItem);
            }

            cart.UpdatedAt = DateTime.UtcNow;
            await _unitOfWork.SaveChangesAsync();

            return await MapToCartDto(cart);
        }

        public async Task<CartDto> UpdateCartItemAsync(Guid userId, Guid cartItemId, UpdateCartItemDto updateDto)
        {
            var carts = await _unitOfWork.Carts.FindAsync(c => c.UserId == userId);
            var cart = carts.FirstOrDefault();

            if (cart == null)
                throw new Exception("Cart not found");

            var cartItem = await _unitOfWork.CartItems.GetByIdAsync(cartItemId);
            if (cartItem == null || cartItem.CartId != cart.Id)
                throw new Exception("Cart item not found");

            var product = await _unitOfWork.Products.GetByIdAsync(cartItem.ProductId);
            if (product.StockQuantity < updateDto.Quantity)
                throw new Exception("Insufficient stock");

            cartItem.Quantity = updateDto.Quantity;
            await _unitOfWork.CartItems.UpdateAsync(cartItem);

            cart.UpdatedAt = DateTime.UtcNow;
            await _unitOfWork.SaveChangesAsync();

            return await MapToCartDto(cart);
        }

        public async Task<bool> RemoveFromCartAsync(Guid userId, Guid cartItemId)
        {
            var carts = await _unitOfWork.Carts.FindAsync(c => c.UserId == userId);
            var cart = carts.FirstOrDefault();

            if (cart == null)
                return false;

            var cartItem = await _unitOfWork.CartItems.GetByIdAsync(cartItemId);
            if (cartItem == null || cartItem.CartId != cart.Id)
                return false;

            await _unitOfWork.CartItems.DeleteAsync(cartItem);
            cart.UpdatedAt = DateTime.UtcNow;
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<bool> ClearCartAsync(Guid userId)
        {
            var carts = await _unitOfWork.Carts.FindAsync(c => c.UserId == userId);
            var cart = carts.FirstOrDefault();

            if (cart == null)
                return false;

            var cartItems = await _unitOfWork.CartItems.FindAsync(ci => ci.CartId == cart.Id);
            foreach (var item in cartItems)
            {
                await _unitOfWork.CartItems.DeleteAsync(item);
            }

            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        private async Task<CartDto> MapToCartDto(Cart cart)
        {
            var cartItems = await _unitOfWork.CartItems.FindAsync(ci => ci.CartId == cart.Id);
            var cartItemDtos = new System.Collections.Generic.List<CartItemDto>();
            decimal totalAmount = 0;
            int totalItems = 0;

            foreach (var item in cartItems)
            {
                var product = await _unitOfWork.Products.GetByIdAsync(item.ProductId);
                if (product != null)
                {
                    var subtotal = product.Price * item.Quantity;
                    totalAmount += subtotal;
                    totalItems += item.Quantity;

                    cartItemDtos.Add(new CartItemDto
                    {
                        Id = item.Id,
                        ProductId = product.Id,
                        ProductName = product.Name,
                        ProductImageUrl = product.ImageUrl,
                        Price = product.Price,
                        Quantity = item.Quantity,
                        Subtotal = subtotal,
                        AvailableStock = product.StockQuantity
                    });
                }
            }

            return new CartDto
            {
                Id = cart.Id,
                UserId = cart.UserId,
                CartItems = cartItemDtos,
                TotalAmount = totalAmount,
                TotalItems = totalItems
            };
        }
    }
}