using CNPM_ktxUtc2Store.Areas.Admin.dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CNPM_ktxUtc2Store.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _context;
        public UserController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> user()
        {
            UserDto userDto = new UserDto();
            var Salerrolers = _context.Roles.Where(x => x.Name == "User").ToList();
            foreach (var item in Salerrolers)
            {
                foreach (var userrole in Salerrolers)
                {
                    var salers = await _context.UserRoles.Where(x => x.RoleId == item.Id).ToListAsync();
                    foreach (var user in salers)
                    {
                        var saler = await _context.Users.Where(x => x.Id == user.UserId).OrderBy(x => x.isRole).ToListAsync();
                        foreach (var i in saler)
                        {
                            userDto.Users.Add(i);
                        }
                    }
                }
            }
            return View(userDto);
        }
    }
}
