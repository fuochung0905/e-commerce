using CNPM_ktxUtc2Store.Areas.Admin.dto;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CNPM_ktxUtc2Store.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class LoginController : Controller
    {
        private readonly SignInManager<applicationUser> _signInManager;
        private readonly ApplicationDbContext _context;
        public LoginController(SignInManager<applicationUser> signInManager, ApplicationDbContext context)
        {
            _signInManager = signInManager;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Index(Login model)
        {
            var useritem = await _context.applicationUsers.Where(x => x.Email == model.Email).ToListAsync();
            var roleitem = await _context.Roles.Where(x => x.Name == "Admin").ToListAsync();
            bool isUser = false;
            foreach (var user in useritem)
            {
                foreach (var role in roleitem)
                {
                    var userrole = await _context.UserRoles.Where(x => x.UserId == user.Id).Where(x => x.RoleId == role.Id).ToListAsync();
                    foreach (var item in userrole)
                    {
                        if (item != null)
                        {
                            isUser = true;
                        }
                    }
                }
            }
            if (isUser == true)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, true, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "dashboard");
                }
                else
                {
                    return RedirectToAction("Index", "Login");
                }
            }
            return RedirectToAction("Index", "Login");

        }
    }
}
