using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CNPM_ktxUtc2Store.Data.Components
{
    public class completeViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<applicationUser> _usermanagement;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public completeViewComponent(ApplicationDbContext dbContext, UserManager<applicationUser> usermanagement, IHttpContextAccessor httpContextAccessor)
        {
            _context = dbContext;
            _usermanagement = usermanagement;
            _httpContextAccessor = httpContextAccessor;
        }
        public IViewComponentResult Invoke()
        {
            var userId = GetUserId();
            var order =  _context.orders.Where(x => x.applicationUserId == userId).Where(x => x.IsDelete == true).Where(x => x.IsComplete == true).Where(x => x.isHuy == false).ToList();
            myOrderDto myOrderDto = new myOrderDto();
            foreach (var orderItem in order)
            {
                var orderdetail =  _context.orderDetails.Include(x => x.order).ThenInclude(o => o.status).Include(x => x.product).Where(x => x.orderId == orderItem.Id).ToList();
                foreach (var item in orderdetail)
                {
                    myOrderDto.orderDetails.Add(item);
                }
            }
            return View(myOrderDto);
        }
        private string GetUserId()
        {

            var pricipal = _httpContextAccessor.HttpContext.User;
            string userId = _usermanagement.GetUserId(pricipal);

            return userId;
        }
    }
}
