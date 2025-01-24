using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using proctos.Models;

namespace proctos.Controllers
{

    public class AdminController : Controller
    {
        private readonly BrandsHopContext _context;

        public AdminController(BrandsHopContext context)
        {
            _context = context;
        }

        // Главная страница панели администратора
        public IActionResult Index()
        {
            return View();
        }

        // *** CRUD for Users ***
        public IActionResult Users()
        {
            var users = _context.Users.Include(u => u.Role).ToList();
            return View(users);
        }

        public IActionResult EditUser(int id)
        {
            var user = _context.Users.Find(id);
            ViewBag.Roles = _context.Roles.ToList();
            return View(user);
        }

        [HttpPost]
        public IActionResult EditUser(User user)
        {
            if (string.IsNullOrWhiteSpace(user.LoginUser))
            {
                ModelState.AddModelError("LoginUser", "Логин пользователя обязателен.");
                ViewBag.Roles = _context.Roles.ToList();
                return View(user); // Возвращаем форму с ошибкой
            }

            // Получаем текущего пользователя из базы данных
            var existingUser = _context.Users.Find(user.IdUser);
            if (existingUser == null)
            {
                return NotFound("Пользователь не найден.");
            }

            // Обновляем изменяемые поля
            existingUser.LoginUser = user.LoginUser;
            existingUser.Email = user.Email;
            existingUser.FullName = user.FullName;
            existingUser.Phone = user.Phone;
            existingUser.RoleId = user.RoleId;

            // Если пароль был передан, обновляем его
            if (!string.IsNullOrWhiteSpace(user.PasswordHash))
            {
                existingUser.PasswordHash = user.PasswordHash; // Здесь вы можете хэшировать пароль
            }

            _context.Users.Update(existingUser);
            _context.SaveChanges();
            return RedirectToAction("Users");
        }


        public IActionResult DeleteUser(int id)
        {
            var user = _context.Users.Find(id);
            if (user != null)
            {
                // Удаляем все связанные детали заказов
                var orderDetails = _context.OrderDetails.Where(od => od.Order.UsersId == user.IdUser).ToList();
                foreach (var orderDetail in orderDetails)
                {
                    _context.OrderDetails.Remove(orderDetail); // Удаляем детали заказа
                }

                // Удаляем все заказы пользователя
                var orders = _context.Orders.Where(o => o.UsersId == user.IdUser).ToList();
                foreach (var order in orders)
                {
                    _context.Orders.Remove(order); // Удаляем заказ
                }

                // Удаляем самого пользователя
                _context.Users.Remove(user);
                _context.SaveChanges();
            }
            return RedirectToAction("Users");
        }



        public IActionResult AddUser()
        {
            ViewBag.Roles = _context.Roles.ToList();
            return View();
        }

        [HttpPost]
        public IActionResult AddUser(User user)
        {
            if (ModelState.IsValid)
            {
                // You can add a default value or validation for LoginUser here if necessary
                if (string.IsNullOrEmpty(user.LoginUser))
                {
                    ModelState.AddModelError("LoginUser", "Логин обязателен.");
                    ViewBag.Roles = _context.Roles.ToList();
                    return View(user);
                }

                // Optionally hash the password before saving
                _context.Users.Add(user);
                _context.SaveChanges();
                return RedirectToAction("Users");
            }

            // Return the view with error messages if model validation fails
            ViewBag.Roles = _context.Roles.ToList();
            return View(user);
        }




        // *** CRUD for Roles ***
        public IActionResult Roles()
        {
            var roles = _context.Roles.ToList();
            return View(roles);
        }

        public IActionResult EditRoles(int id)
        {
            var role = _context.Roles.Find(id);
            return View(role);
        }

        [HttpPost]
        public IActionResult EditRoles(Role role)
        {
            _context.Roles.Update(role);
            _context.SaveChanges();
            return RedirectToAction("Roles");
        }

        public IActionResult DeleteRole(int id)
        {
            var role = _context.Roles.Find(id);
            if (role != null)
            {
                _context.Roles.Remove(role);
                _context.SaveChanges();
            }
            return RedirectToAction("Roles");
        }

        public IActionResult AddRole()
        {
            return View(new Role());
        }

        [HttpPost]
        public IActionResult AddRole(Role role)
        {
            _context.Roles.Add(role);
            _context.SaveChanges();
            return RedirectToAction("Roles");
        }

        // *** CRUD for Products ***
        public IActionResult Products()
        {
            var products = _context.Products
                .Include(p => p.CategoryOfProduct)
                .Include(p => p.CategoryOfGender)
                .Include(p => p.Size)
                .Include(p => p.Colour)
                .Include(p => p.Material)
                .ToList();
            return View(products);
        }

        public IActionResult EditProduct(int id)
        {
            var product = _context.Products.Find(id);
            if (product == null)
            {
                return NotFound();
            }

            ViewBag.Categories = new SelectList(_context.CategoryOfProducts, "IdCategoryOfProduct", "NameCategoryOfProduct", product.CategoryOfProductId);
            ViewBag.Genders = _context.CategoryOfGenders.ToList();
            ViewBag.Sizes = _context.Sizes.ToList();
            ViewBag.Colours = _context.Colours.ToList();
            ViewBag.Materials = _context.Materials.ToList();

            return View(product);
        }


        [HttpPost]
        public IActionResult EditProduct(Product product)
        {
            _context.Products.Update(product);
            _context.SaveChanges();
            return RedirectToAction("Products");
        }

        public IActionResult DeleteProduct(int id)
        {
            var product = _context.Products.Find(id);
            if (product != null)
            {
                _context.Products.Remove(product);
                _context.SaveChanges();
            }
            return RedirectToAction("Products");
        }

        public IActionResult AddProduct()
        {
            ViewBag.Categories = _context.CategoryOfProducts.ToList();
            ViewBag.Genders = _context.CategoryOfGenders.ToList();
            ViewBag.Sizes = _context.Sizes.ToList();
            ViewBag.Colours = _context.Colours.ToList();
            ViewBag.Materials = _context.Materials.ToList();
            return View(new Product());
        }

        [HttpPost]
        public IActionResult AddProduct(Product product)
        {
            if (ModelState.IsValid)
            {
                _context.Products.Add(product);
                _context.SaveChanges();
                return RedirectToAction("Products");
            }

            // Логирование ошибок
            foreach (var modelState in ModelState.Values)
            {
                foreach (var error in modelState.Errors)
                {
                    Console.WriteLine(error.ErrorMessage); // Используйте логгер вместо Console.WriteLine
                }
            }

            // Если ModelState не прошел, возвращаем форму с ошибками
            ViewBag.Categories = _context.CategoryOfProducts.ToList();
            ViewBag.Genders = _context.CategoryOfGenders.ToList();
            ViewBag.Sizes = _context.Sizes.ToList();
            ViewBag.Colours = _context.Colours.ToList();
            ViewBag.Materials = _context.Materials.ToList();

            return View(product);
        }



        // *** CRUD for CategoryOfGender ***
        public IActionResult CategoryOfGenders()
        {
            var categories = _context.CategoryOfGenders.ToList();
            return View(categories);
        }

        public IActionResult AddCategoryOfGender()
        {
            return View(new CategoryOfGender());
        }

        [HttpPost]
        public IActionResult AddCategoryOfGender(CategoryOfGender category)
        {
            if (ModelState.IsValid)
            {
                _context.CategoryOfGenders.Add(category);
                _context.SaveChanges();
                return RedirectToAction("CategoryOfGenders");
            }
            return View(category);
        }

        public IActionResult EditCategoryOfGender(int id)
        {
            var category = _context.CategoryOfGenders.Find(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        [HttpPost]
        public IActionResult EditCategoryOfGender(CategoryOfGender category)
        {
            if (ModelState.IsValid)
            {
                _context.CategoryOfGenders.Update(category);
                _context.SaveChanges();
                return RedirectToAction("CategoryOfGenders");
            }
            return View(category);
        }

        public IActionResult DeleteCategoryOfGender(int id)
        {
            var category = _context.CategoryOfGenders.Find(id);
            if (category != null)
            {
                _context.CategoryOfGenders.Remove(category);
                _context.SaveChanges();
            }
            return RedirectToAction("CategoryOfGenders");
        }

        // *** CRUD for CategoryOfProduct ***
        public IActionResult CategoryOfProducts()
        {
            var categories = _context.CategoryOfProducts.ToList();
            return View(categories);
        }

        public IActionResult AddCategoryOfProduct()
        {
            return View(new CategoryOfProduct());
        }

        [HttpPost]
        public IActionResult AddCategoryOfProduct(CategoryOfProduct category)
        {
            if (ModelState.IsValid)
            {
                _context.CategoryOfProducts.Add(category);
                _context.SaveChanges();
                return RedirectToAction("CategoryOfProducts");
            }
            return View(category);
        }

        public IActionResult EditCategoryOfProduct(int id)
        {
            var category = _context.CategoryOfProducts.Find(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        [HttpPost]
        public IActionResult EditCategoryOfProduct(CategoryOfProduct category)
        {
            if (ModelState.IsValid)
            {
                _context.CategoryOfProducts.Update(category);
                _context.SaveChanges();
                return RedirectToAction("CategoryOfProducts");
            }
            return View(category);
        }

        public IActionResult DeleteCategoryOfProduct(int id)
        {
            var category = _context.CategoryOfProducts.Find(id);
            if (category != null)
            {
                _context.CategoryOfProducts.Remove(category);
                _context.SaveChanges();
            }
            return RedirectToAction("CategoryOfProducts");
        }

        // *** CRUD for Size ***
        public IActionResult Sizes()
        {
            var sizes = _context.Sizes.ToList();
            return View(sizes);
        }

        public IActionResult AddSize()
        {
            return View(new Size());
        }

        [HttpPost]
        public IActionResult AddSize(Size size)
        {
            if (ModelState.IsValid)
            {
                _context.Sizes.Add(size);
                _context.SaveChanges();
                return RedirectToAction("Sizes");
            }
            return View(size);
        }

        public IActionResult EditSize(int id)
        {
            var size = _context.Sizes.Find(id);
            if (size == null)
            {
                return NotFound();
            }
            return View(size);
        }

        [HttpPost]
        public IActionResult EditSize(Size size)
        {
            if (ModelState.IsValid)
            {
                _context.Sizes.Update(size);
                _context.SaveChanges();
                return RedirectToAction("Sizes");
            }
            return View(size);
        }

        public IActionResult DeleteSize(int id)
        {
            var size = _context.Sizes.Find(id);
            if (size != null)
            {
                _context.Sizes.Remove(size);
                _context.SaveChanges();
            }
            return RedirectToAction("Sizes");
        }

        // *** CRUD for Colour ***
        public IActionResult Colours()
        {
            var colours = _context.Colours.ToList();
            return View(colours);
        }

        public IActionResult AddColour()
        {
            return View(new Colour());
        }

        [HttpPost]
        public IActionResult AddColour(Colour colour)
        {
            if (ModelState.IsValid)
            {
                _context.Colours.Add(colour);
                _context.SaveChanges();
                return RedirectToAction("Colours");
            }
            return View(colour);
        }

        public IActionResult EditColour(int id)
        {
            var colour = _context.Colours.Find(id);
            if (colour == null)
            {
                return NotFound();
            }
            return View(colour);
        }

        [HttpPost]
        public IActionResult EditColour(Colour colour)
        {
            if (ModelState.IsValid)
            {
                _context.Colours.Update(colour);
                _context.SaveChanges();
                return RedirectToAction("Colours");
            }
            return View(colour);
        }

        public IActionResult DeleteColour(int id)
        {
            var colour = _context.Colours.Find(id);
            if (colour != null)
            {
                _context.Colours.Remove(colour);
                _context.SaveChanges();
            }
            return RedirectToAction("Colours");

        }
        // *** CRUD for Materials ***
        public IActionResult Materials()
        {
            var materials = _context.Materials.ToList();
            return View(materials);
        }

        public IActionResult AddMaterial()
        {
            return View(new Material());
        }

        [HttpPost]
        public IActionResult AddMaterial(Material material)
        {
            if (ModelState.IsValid)
            {
                _context.Materials.Add(material);
                _context.SaveChanges();
                return RedirectToAction("Materials");
            }
            return View(material);
        }

        public IActionResult EditMaterial(int id)
        {
            var material = _context.Materials.Find(id);
            if (material == null)
            {
                return NotFound();
            }
            return View(material);
        }

        [HttpPost]
        public IActionResult EditMaterial(Material material)
        {
            if (ModelState.IsValid)
            {
                _context.Materials.Update(material);
                _context.SaveChanges();
                return RedirectToAction("Materials");
            }
            return View(material);
        }

        public IActionResult DeleteMaterial(int id)
        {
            var material = _context.Materials.Find(id);
            if (material != null)
            {
                _context.Materials.Remove(material);
                _context.SaveChanges();
            }
            return RedirectToAction("Materials");
        }
        public IActionResult Logout()
        {
            // Очистить аутентификационные cookies
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            // Перенаправить пользователя на главную страницу или страницу входа
            return RedirectToAction("Index", "Home");
        }

        // Действие для отображения страницы Manage Dashboard
        public IActionResult ManageDashboard()
        {
            return View("ManageDashboard"); // Укажите точное имя представления
        }


    }
}