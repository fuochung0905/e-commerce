﻿using CNPM_ktxUtc2Store.Areas.Admin.dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CNPM_ktxUtc2Store.Areas.Admin.Components
{
    public class processViewComponent: ViewComponent
    {
        private readonly ApplicationDbContext _context;
        public processViewComponent(ApplicationDbContext dbContext)
        {
            _context = dbContext;
        }
        public IViewComponentResult Invoke()
        {
            var applicationDbContext = _context.orders.Where(x => x.IsDelete == true).Where(x=>x.IsComplete==false).Include(o => o.applicationUser).Include(o => o.status).ToList();
            duyetDon a = new duyetDon();
            foreach (var item in applicationDbContext)
            {
                var orderDetail = _context.orderDetails.Include(x => x.product).Where(x => x.Id == item.Id).ToList();
                foreach (var ite in orderDetail)
                {
                    a.orderDetail = ite;
                }

                a.orderList.Add(item);
            }
            return View(a);
        }
    }
}
