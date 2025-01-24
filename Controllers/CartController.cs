using Microsoft.AspNetCore.Mvc;
using proctos.Models;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace proctos.Controllers
{
    public class CartController : Controller
    {
        private readonly BrandsHopContext _context;

        public CartController(BrandsHopContext context)
        {
            _context = context;
        }

        // Отображение содержимого корзины
        public IActionResult Cart()
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email); // Получаем email пользователя
            if (string.IsNullOrEmpty(userEmail))
            {
                return RedirectToAction("Aut", "Home"); // Если пользователь не авторизован
            }

            // Получаем содержимое корзины для текущего пользователя
            var cartItems = _context.CartItems
                                    .Include(ci => ci.Product) // Подгружаем данные о товаре
                                    .Where(ci => ci.UserId == userEmail)
                                    .ToList();

            return View(cartItems); // Передаем данные в представление
        }

        // Обновление количества товара в корзине
        [HttpPost]
        public IActionResult UpdateQuantity(int cartItemId, int quantity)
        {
            var cartItem = _context.CartItems.Find(cartItemId);
            if (cartItem != null && quantity > 0)
            {
                cartItem.Quantity = quantity;
                _context.SaveChanges();
            }

            return RedirectToAction("Cart", "Cart");
        }

        // Удаление товара из корзины
        [HttpPost]
        public IActionResult RemoveFromCart(int cartItemId)
        {
            var cartItem = _context.CartItems.Find(cartItemId);
            if (cartItem != null)
            {
                _context.CartItems.Remove(cartItem);
                _context.SaveChanges(); // Сохраняем изменения в базе данных
            }

            return RedirectToAction("Cart"); // Перенаправляем на страницу корзины после удаления
        }

    }
}
