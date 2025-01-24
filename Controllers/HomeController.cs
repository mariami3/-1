using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using proctos.Models;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Security.Claims;
using System.Net;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
namespace proctos.Controllers
{
    public class HomeController : Controller
    {
		private readonly ILogger<HomeController> _logger; 
		private readonly BrandsHopContext _context; 
		
        private static List<cloth> cloths = new List<cloth>
        {
            new cloth { Id = 1, Name = "Nike", Description = "��������� Wmns Dunk High", Price = 19890, Image = "" },
            new cloth { Id = 2, Name = "Stone Island Junior", Description = "������� ��������� Season 80 Crew Neck", Price = 22190, Image = "" },
            new cloth { Id = 3, Name = "Vance", Description = "������� ��������� Arched Pullover Hoodie", Price = 8790, Image = "" },
            new cloth { Id = 4, Name = "RIPNDIP", Description = "������ Lord Nermal", Price = 5190, Image = "" }
        };
		public HomeController(ILogger<HomeController> logger, BrandsHopContext context)
		{
			_logger = logger;
			_context = context;
		}

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

	

        [HttpGet]
        public IActionResult Login()
		{
			return View();
		}

        [HttpGet]
        // ������� ��������
        public IActionResult Aut()
        {
            return View();
        }
        [HttpGet]
        public IActionResult reg()
        {
            return View();
        }




        [HttpPost]
        public async Task<IActionResult> Aut(string email, string password)
        {
            string hashedPassword = HashPassword(password);

            var user = _context.Users.FirstOrDefault(u => u.Email == email && u.PasswordHash == hashedPassword);
            if (user != null)
            {
                var claims = new List<Claim>()
        {
            new Claim(ClaimTypes.Name, user.FullName),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.NameIdentifier, user.IdUser.ToString()),
            new Claim(ClaimTypes.Role, user.RoleId == 1 ? "Customer" : "OtherRole")
        };
                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);

                return RedirectToAction("Catalog", "Home");
            }

            ViewBag.ErrorMessage = "�������� ������� ������";
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> reg(string fullName, string email, string phone, string password)
        {
            if (_context.Users.Any(u => u.Email == email))
            {
                ViewBag.ErrorMessage = "������������ � ����� email ��� ����������";
                return View();
            }

            string hashedPassword = HashPassword(password);

            var user = new User
            {
                LoginUser = email, // ���������� email � �������� ������
                FullName = fullName,
                Email = email,
                Phone = phone,
                PasswordHash = hashedPassword,
                DateJoined = DateTime.Now,
                RoleId = 1
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return RedirectToAction("Aut");
        }


        public IActionResult Privacy()
		{
			return View();
		}

		private string HashPassword(string password)
		{
			using (var sha256 = SHA256.Create())
			{
				var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
				return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
			}
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
		
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}


        // �������� "�������"
        [HttpGet("catalog")] // ������� ��� catalog
        public IActionResult catalog()
        {
            return View(cloths);
        }


        // �������� "�����"
        public IActionResult order()
        {
            return View(cloths);
        }

        public IActionResult AddTofavorites(int id)
        {
            // �������  �� ID
            var cloth = cloths.FirstOrDefault(c => c.Id == id);
            if (cloth != null)
            {
                cloth.IsFavorite = true; // ������������� ���� ����������
            }

            return RedirectToAction("Catalog"); // ������������ � �������
        }

        // �������� "���������"
        public IActionResult RemoveFromfavorites(int id)
        {
            var cloth = cloths.FirstOrDefault(c => c.Id == id);
            if (cloth != null)
            {
                cloth.IsFavorite = false; // ������� ���� ����������
            }

            return RedirectToAction("Favorites"); // ������������ �� �������� ����������
        }

        public IActionResult favorites()
        {
            List<cloth> favoriteCars = cloths.Where(c => c.IsFavorite).ToList(); 
            return View(favoriteCars); 
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(int id, int quantity = 1)
        {
            // ��������� email ������������ �� Claims
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(userEmail))
            {
                return RedirectToAction("Aut", "Home"); // ��������������� �� �����������
            }

            // ����� ����� �� ID
            var product = _context.Products.FirstOrDefault(p => p.IdProduct == id);
            if (product == null)
            {
                ViewBag.ErrorMessage = "����� �� ������";
                return RedirectToAction("Catalog", "Home");
            }

            // ��������� ������� ������ � �������
            var cartItem = _context.CartItems.FirstOrDefault(ci => ci.ProductId == id && ci.UserId == userEmail);
            if (cartItem != null)
            {
                cartItem.Quantity += quantity; // ����������� ����������
            }
            else
            {
                // ������� ����� ������� �������
                cartItem = new CartItem
                {
                    UserId = userEmail,
                    ProductId = id,
                    Quantity = quantity,
                    Price = product.Price
                };
                _context.CartItems.Add(cartItem);
            }

            await _context.SaveChangesAsync(); // ��������� ���������
            return RedirectToAction("Cart", "Cart"); // ������� � �������� �������
        }



        [HttpGet("CatalogWithFilter")] // ������� ��� Catalog
        public IActionResult Catalog(string search, string sortOrder, string filter)
        {
            // ��������� ������ �������
            var filteredCloths = cloths.AsQueryable();

            // �����
            if (!string.IsNullOrEmpty(search))
            {
                filteredCloths = filteredCloths.Where(c => c.Name.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                                                           c.Description.Contains(search, StringComparison.OrdinalIgnoreCase));
            }

            // ����������
            if (!string.IsNullOrEmpty(filter))
            {
                if (filter == "favorites")
                {
                    filteredCloths = filteredCloths.Where(c => c.IsFavorite);
                }
            }
            ViewBag.CategoryOptions = new List<SelectListItem>
            {
                new SelectListItem { Text = "��� ���������", Value = "" },
                new SelectListItem { Text = "������", Value = "1" },
                new SelectListItem { Text = "�����", Value = "2" }
            };

            // ����������
            switch (sortOrder)
            {
                case "price_asc":
                    filteredCloths = filteredCloths.OrderBy(c => c.Price);
                    break;
                case "price_desc":
                    filteredCloths = filteredCloths.OrderByDescending(c => c.Price);
                    break;
                case "name_asc":
                    filteredCloths = filteredCloths.OrderBy(c => c.Name);
                    break;
                case "name_desc":
                    filteredCloths = filteredCloths.OrderByDescending(c => c.Name);
                    break;
            }

            return View(filteredCloths.ToList());
        }


        public IActionResult contacts()
        {
            return View();
        }
         
        // �������� "� ���"
        public IActionResult about()
        {
            return View();
        }

        // �������� "�������"
        public IActionResult Cart()
        {
            return View();
        }


        [HttpGet]
        public IActionResult ProductDetails(int id)
        {
            if (!User.Identity.IsAuthenticated)
            {
                // ���� ������������ �� �����������, �������������� �� �������� �����������
                return RedirectToAction("Aut", "Home");
            }

            var product = _context.Products
                .Where(p => p.IdProduct == id)
                .Include(p => p.Reviews)
                .FirstOrDefault();

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }


        [HttpPost]
        [Authorize]
        public IActionResult AddReview(Review review)
        {
            if (ModelState.IsValid)
            {
                _context.Reviews.Add(review);  // ��������� ����� � ���� ������
                _context.SaveChanges();
                return RedirectToAction("ProductDetails", new { id = review.ProductId });  // �������������� �� �������� ������
            }
            return View(review);  // ���� ����� �� ������ ���������, ���������� � � ��������
        }



    }
}
