﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CNPM_ktxUtc2Store.Data;
using CNPM_ktxUtc2Store.Models;
using Microsoft.AspNetCore.Authorization;
using X.PagedList;
using CNPM_ktxUtc2Store.Areas.Admin.dto;
using System.Net.WebSockets;
using Microsoft.AspNetCore.Identity;

namespace CNPM_ktxUtc2Store.Areas.Saler.Controllers
{
    [Area("Saler")]
    [Authorize(Roles = "Saler")]
    public class productsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly UserManager<applicationUser> _usermanagement;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public productsController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment, UserManager<applicationUser> usermanagement, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _usermanagement = usermanagement;
            _webHostEnvironment = webHostEnvironment;
        }
        private string GetUserId()
        {

            var pricipal = _httpContextAccessor.HttpContext.User;
            string userId = _usermanagement.GetUserId(pricipal);

            return userId;
        }
        // GET: Admin/products
        public IActionResult Index(string name)
        {
            productDto productDto = new productDto();
            var product = from p in _context.products.Where(x=>x.qty_inStock>0) select p;
           
            foreach(var item in product)
            {
                
                productDto.products.Add(item);
                var productvariation = _context.productVariations.Include(x=>x.variation).Where(x => x.productId == item.Id).ToList();
                foreach (var i in productvariation)
                {
                    productDto.productVariation.Add(i);
                }
            }
            if (!string.IsNullOrEmpty(name))
            {
                    product = product.Where(x => x.productName.Contains(name) );
                foreach (var item in product)
                {
                    productDto.products.Add(item);
                }

            }
            return View(productDto);
        }
        // GET: Admin/products/Details/5
        public IActionResult Details(int? id)
        {

            var product = new product();
            product = _context.products.Find(id);
             
            var variation = (from v in _context.variation
                             join c in _context.categories
                             on v.categoryId equals c.Id
                             where(c.Id==product.categoryId )
                             select new variation
                             {
                                 Id = v.Id,
                                 name = v.name,
                                value=v.value,
                                category=c
                             }).ToList();
            return View(variation);
        }
        // GET: Admin/products/Create
        public IActionResult Create()
        {
            ViewData["categoryId"] = new SelectList(_context.categories, "Id", "categoryName");
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(product product)
        {
            if (product.oldprice.HasValue && product.price.HasValue)
            {
                if (product.oldprice.Value > product.price.Value)
                {
                    ModelState.AddModelError("", " Giá nhập nên bé hơn giá bán");
                }
            }
            if (ModelState.IsValid)
            {
                var userId = GetUserId();
                var user = await _context.applicationUsers.FindAsync(userId);
                string uni=uploadImage(product);
                product.imageUrl = uni;
                product.ApplicationUsers.Add(user);
                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["categoryId"] = new SelectList(_context.categories, "Id", "Id", product.categoryId);
            return View(product);

        }
        //Get Admin/product/AddVariation/5
        public  IActionResult AddVariation(int? id)
        {
            var product =  _context.products.Find(id);
            var variation=_context.variation.Where(x=>x.categoryId==product.categoryId).ToList();
            productvariationDto dto = new productvariationDto();
            foreach(var item in variation)
            {
                dto.Variations.Add(item);
            }
            var productvariation = _context.productVariations.Where(x => x.productId == product.Id).ToList();
            foreach(var pv in productvariation)
            {
                dto.productVariations.Add(pv);
            }
            return View(dto);  
        }
       
        // GET: Admin/products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.products == null)
            {
                return NotFound();
            }
            var product = await _context.products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, product product)
        {
            if (id != product.Id)
            {
                return NotFound();
            }
            var pr = _context.products.Find(id);
            pr.price = product.price;   
            pr.description=product.description;
            pr.productName=product.productName;
            pr.oldprice=product.oldprice;
            pr.qty_inStock=product.qty_inStock;
                    _context.products.Update(pr);
                    await _context.SaveChangesAsync();
          
            return View(product);
        }

        // GET: Admin/products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.products == null)
            {
                return NotFound();
            }

            var product = await _context.products
                .Include(p => p.category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Admin/products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.products == null)
            {
                return Problem("Entity set 'ApplicationDbContext.products'  is null.");
            }
            var product = await _context.products.FindAsync(id);
            if (product != null)
            {
                _context.products.Remove(product);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool productExists(int id)
        {
            return (_context.products?.Any(e => e.Id == id)).GetValueOrDefault();
        }
        private string uploadImage(product model)
        {
            string uniqueFileName = string.Empty;
            if (model.image != null)
            {
                string uploadFoder = Path.Combine(_webHostEnvironment.WebRootPath, "images/");
                uniqueFileName = Guid.NewGuid().ToString() + "_" + model.image.FileName;
                string filePath = Path.Combine(uploadFoder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    model.image.CopyTo(fileStream);
                }
            }
            return uniqueFileName;
        }
    }
}
