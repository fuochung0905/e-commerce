using CNPM_ktxUtc2Store.Controllers.Constants;
using CNPM_ktxUtc2Store.Service;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CNPM_ktxUtc2Store.Controllers
{
    public class WSalerController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<applicationUser> _usermanagement;
        private readonly IHttpContextAccessor _httpContextAccessor;
       
        public WSalerController(ApplicationDbContext context, UserManager<applicationUser> usermanagement, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _usermanagement = usermanagement;
            _httpContextAccessor = httpContextAccessor;
        }
        private string GetUserId()
        {
            var pricipal = _httpContextAccessor.HttpContext.User;
            string userId = _usermanagement.GetUserId(pricipal);
            return userId;
        }
        public IActionResult wantToSaler()
        {
            var userId = GetUserId();
            var user = _context.applicationUsers.Find(userId);
            wSalerDto model = new wSalerDto();
            model.applicationUser = user;
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> wantToSaler(wSalerDto model)
        {
            if (model.isRole == false)
            {
                return Content("Cần đồng ý điều khoản");
            }
            var userId= GetUserId();
            var user = _context.applicationUsers.Find(userId);
            user.isRole = true;
             _context.applicationUsers.Update(user);
            await _context.SaveChangesAsync();
            await _usermanagement.AddToRoleAsync(user, Roles.Saler.ToString());
            return RedirectToAction("Index", "Home");
        }
    }
}
