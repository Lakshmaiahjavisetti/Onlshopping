using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Mvc_systemtest.Models;
using static Mvc_systemtest.Models.ApplicationUser;

namespace Mvc_systemtest.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context = new ApplicationDbContext();  // Make it readonly
        private ApplicationUserManager _userManager;

        public AccountController()
        {
            var userStore = new UserStore<ApplicationUser>(_context);
            _userManager = new ApplicationUserManager(userStore);
        }

        public ActionResult Index()
        {
            return View();
        }
        // Registration GET
        public ActionResult Register()
        {
            return View();
        }

        // Registration POST
        [HttpPost]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.Username };
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user.Id, model.Role);
                    return RedirectToAction("Login");
                }
                AddErrors(result);
            }
            return View(model);
        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindAsync(model.Username, model.Password);
                if (user != null)
                {
                    var roles = await _userManager.GetRolesAsync(user.Id);
                    if (roles.Contains("Admin"))
                    {
                        return RedirectToAction("AdminDashboard");
                    }
                    else if (roles.Contains("Seller"))
                    {
                        return RedirectToAction("SellerDashboard");
                    }
                    else if (roles.Contains("Buyer"))
                    {
                        return RedirectToAction("Cart");
                    }
                }
                ModelState.AddModelError("", "Invalid login attempt.");
            }
            return View(model);
        }

        public ActionResult AdminDashboard()
        {
            var users = _context.Users.ToList(); // Get all users
            return View(users);
        }

        public ActionResult DeactivateUser(int userId)
        {
            var user = _context.Users.Find(userId);
            if (user != null)
            {
                user.IsActive = false;
                _context.SaveChanges();
            }
            return RedirectToAction("AdminDashboard");
        }

        public ActionResult SellerDashboard()
        {
            var sellerId = GetCurrentSellerId(); // Method to fetch the current seller ID
            var products = _context.Products.Where(p => p.SellerId == sellerId).ToList();
            return View(products);
        }

        public ActionResult UploadProduct()
        {
            return View();
        }

        [HttpPost]
        public ActionResult UploadProduct(ProductViewModel model)
        {
            if (ModelState.IsValid)
            {
                var sellerId = GetCurrentSellerId();
                var product = new Product
                {
                    SellerId = sellerId,
                    ProductName = model.ProductName,
                    ProductDescription = model.ProductDescription,
                    Price = model.Price,
                    ImagePath = model.ImagePath
                };
                _context.Products.Add(product);
                _context.SaveChanges();
                return RedirectToAction("SellerDashboard");
            }
            return View(model);
        }

        public ActionResult Cart()
        {
            var userId = GetCurrentUserId(); // Method to get the current user ID
            var cartItems = _context.Cart.Where(c => c.UserId == userId).ToList();
            return View(cartItems);
        }

        public ActionResult AddToCart(int productId)
        {
            var userId = GetCurrentUserId();
            var cartItem = new Cart
            {
                UserId = userId,
                ProductId = productId,
                Quantity = 1 // Default to 1, can allow changing quantity
            };
            _context.Cart.Add(cartItem);
            _context.SaveChanges();
            return RedirectToAction("Cart");
        }

        // Helper methods
        private string GetCurrentUserId()
        {
            return User.Identity.GetUserId();  // Assuming you're using ASP.NET Identity
        }

        private int GetCurrentSellerId()
        {
            // Implement logic to get the current seller's ID
            // Assuming the user has a seller profile or it's stored in the user model
            return 1; // Placeholder for actual logic
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }
    }
}
