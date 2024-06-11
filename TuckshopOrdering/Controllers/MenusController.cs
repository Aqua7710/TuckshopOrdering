using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using TuckshopOrdering.Areas.Identity.Data;
using TuckshopOrdering.Migrations;
using TuckshopOrdering.Models;

namespace TuckshopOrdering.Controllers
{
    public class MenusController : Controller
    {
        private readonly TuckshopOrderingSystem _context;
        private readonly IWebHostEnvironment _hostEnviroment;

        public MenusController(TuckshopOrderingSystem context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            this._hostEnviroment = hostEnvironment;
        }

        // GET: Menus
        public async Task<IActionResult> Index(int categoryId)
        {
            TempData["CurrentCategoryID"] = categoryId;

            var categories = await _context.Category.ToListAsync();
            var customises = await _context.Customise.ToListAsync();
            var foodOrders = await _context.FoodOrder.ToListAsync();
            var menu = await _context.Menu.Include(m => m.Category)
                                          .Include(m => m.Customise)
                                          .Include(m => m.FoodOrders)
                                          .ToListAsync();

            if (categoryId != 0) // category filter
            {
                menu = await _context.Menu.Include(m => m.Category).Where(o => o.CategoryID == categoryId)
                                          .Include(m => m.Customise)
                                          .Include(m => m.FoodOrders)
                                          .ToListAsync();
            }

            MenuViewModel mvm = new MenuViewModel()
            {
                _Menu = menu,
                _Category = categories,
                _FoodOrder = foodOrders,
                _Customise = customises
            };

            return View(mvm);
        }

        // GET: Menus/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Menu == null)
            {
                return NotFound();
            }

            var menu = await _context.Menu
                .Include(m => m.Category)
                .Include(m => m.Customise)
                .FirstOrDefaultAsync(m => m.MenuID == id);
            if (menu == null)
            {
                return NotFound();
            }

            return View(menu);
        }

        // GET: Menus/Create
        public IActionResult Create()
        {
            ViewData["CategoryID"] = new SelectList(_context.Category, "CategoryID", "CategoryName");
            ViewData["CustomiseID"] = new SelectList(_context.Customise, "CustomiseID", "CustomiseID");
            ViewData["OrderID"] = new SelectList(_context.Order, "OrderID", "OrderID");
            return View();
        }

        // POST: Menus/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MenuID,foodName,price,imageFile,CategoryID,CustomiseID,homePageDisplay,OrderID")] Menu menu)
        {
            if (!ModelState.IsValid)
            {
                // saves image to wwwroot/images
                string wwwRootPath = _hostEnviroment.WebRootPath;
                string fileName = Path.GetFileNameWithoutExtension(menu.imageFile.FileName);
                string extension = Path.GetExtension(menu.imageFile.FileName);
                menu.imageName = fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                string path = Path.Combine(wwwRootPath + "/Images", fileName);
                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    await menu.imageFile.CopyToAsync(fileStream);
                }
                // Insert record
                _context.Add(menu);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryID"] = new SelectList(_context.Category, "CategoryID", "CategoryID", menu.CategoryID);
            ViewData["CustomiseID"] = new SelectList(_context.Customise, "CustomiseID", "CustomiseID", menu.CustomiseID);

            return View(menu);
        }

        // GET: Menus/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Menu == null)
            {
                return NotFound();
            }

            var menu = await _context.Menu.FindAsync(id);
            if (menu == null)
            {
                return NotFound();
            }
            ViewData["CategoryID"] = new SelectList(_context.Category, "CategoryID", "CategoryID", menu.CategoryID);
            ViewData["CustomiseID"] = new SelectList(_context.Customise, "CustomiseID", "CustomiseID", menu.CustomiseID);
            return View(menu);
        }

        // POST: Menus/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MenuID,foodName,price,imageName,CategoryID,CustomiseID,homePageDisplay")] Menu menu)
        {
            if (id != menu.MenuID)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                // saves image to wwwroot/images
                string wwwRootPath = _hostEnviroment.WebRootPath;
                string fileName = Path.GetFileNameWithoutExtension(menu.imageFile.FileName);
                string extension = Path.GetExtension(menu.imageFile.FileName);
                menu.imageName = fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                string path = Path.Combine(wwwRootPath + "/Images", fileName);
                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    await menu.imageFile.CopyToAsync(fileStream);
                }
                // Insert record
                _context.Add(menu);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));

                try
                {
                    _context.Update(menu);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MenuExists(menu.MenuID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryID"] = new SelectList(_context.Category, "CategoryID", "CategoryID", menu.CategoryID);
            ViewData["CustomiseID"] = new SelectList(_context.Customise, "CustomiseID", "CustomiseID", menu.CustomiseID);
            return View(menu);
        }

        // GET: Menus/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Menu == null)
            {
                return NotFound();
            }

            var menu = await _context.Menu
                .Include(m => m.Category)
                .Include(m => m.Customise)
                .FirstOrDefaultAsync(m => m.MenuID == id);

            if (menu == null)
            {
                return NotFound();
            }

            return View(menu);
        }

        // POST: Menus/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Menu == null)
            {
                return Problem("Entity set 'TuckshopOrderingSystem.Menu'  is null.");
            }
            var menu = await _context.Menu.FindAsync(id);

            // Delete image file from wwwroot folder

            var imagePath = Path.Combine(_hostEnviroment.WebRootPath, "Images", menu.imageName);
            if (System.IO.File.Exists(imagePath))
                System.IO.File.Exists(imagePath);

            //delete file
            _context.Menu.Remove(menu);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MenuExists(int id)
        {
            return (_context.Menu?.Any(e => e.MenuID == id)).GetValueOrDefault();
        }

        [HttpPost]
        public async Task<IActionResult> AddToOrder(int menuItemID)
        {
            var menuItem = await _context.Menu.FindAsync(menuItemID);

            if (menuItem == null)
            {
                return NotFound();
            }

            var existingOrder = await _context.FoodOrder.FirstOrDefaultAsync(o => o.MenuID == menuItemID);

            if (existingOrder != null)
            {
                existingOrder.quantity += 1; // item already exists, so increase quantity
            }
            else
            {
                var foodOrder = new FoodOrder
                {
                    MenuID = menuItemID,
                    quantity = 1,
                    studentName = "Will",
                    
                };

                _context.FoodOrder.Add(foodOrder);
            }

            await _context.SaveChangesAsync();

            /*if(currentCategoryID > 0)
            {
                currentCategoryID = menuItem.CategoryID;
            }
            else if(currentCategoryID == 0)
            {
                currentCategoryID = 0;
            }

            return RedirectToAction("Index", new { categoryId = currentCategoryID});*/

            int categoryID = TempData.ContainsKey("CurrentCategoryID") ? (int)TempData["CurrentCategoryID"] : 0;

            return RedirectToAction("Index", new { categoryId = categoryID });
        }

        [HttpPost]
        public async Task<IActionResult> DecreaseQuantity(int foodOrderID)
        {
            var foodOrder = await _context.FoodOrder.FindAsync(foodOrderID);
            //var menuItem = await _context.Menu.FindAsync(categoryID);

            if (foodOrder != null && foodOrder.quantity > 1)
            {
                foodOrder.quantity -= 1;
                await _context.SaveChangesAsync();
            }

            int categoryID = TempData.ContainsKey("CurrentCategoryID") ? (int)TempData["CurrentCategoryID"] : 0;

            return RedirectToAction("Index", new { categoryId = categoryID });
        }

        [HttpPost]
        public async Task<IActionResult> IncreaseQuantity(int foodOrderID)
        {
            var foodOrder = await _context.FoodOrder.FindAsync(foodOrderID);

            if (foodOrder != null)
            {
                foodOrder.quantity += 1;
                await _context.SaveChangesAsync();
            }

            // filter page

            int categoryID = TempData.ContainsKey("CurrentCategoryID") ? (int)TempData["CurrentCategoryID"] : 0;

            return RedirectToAction("Index", new { categoryId = categoryID });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteItem(int foodOrderID)
        {
            var foodOrder = await _context.FoodOrder.FindAsync(foodOrderID);

            if (foodOrder != null)
            {
                _context.FoodOrder.Remove(foodOrder);
                await _context.SaveChangesAsync();
            }

            int categoryID = TempData.ContainsKey("CurrentCategoryID") ? (int)TempData["CurrentCategoryID"] : 0;

            return RedirectToAction("Index", new { categoryId = categoryID });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteAllItems()
        {
            var foodOrder = _context.FoodOrder;

            _context.FoodOrder.RemoveRange(foodOrder);
            await _context.SaveChangesAsync();

            //int categoryID = TempData.ContainsKey("CurrentCategoryID") ? (int)TempData["CurrentCategoryID"] : 0;

            return RedirectToAction("Index", new { categoryId = 0 });
        }

        [HttpPost]
        public async Task<IActionResult> CompleteOrder()
        {
            var order = new Order
            {

            };
            return RedirectToAction("Index", new { categoryId = 0 });
        }

        /*HttpPost]
        public async Task<IActionResult> Checkout(int foodOrderID)
        {
            var foodOrder = await _context.Menu.FindAsync(foodOrderID);

            if (foodOrder == null)
            {
                return NotFound();
            }

                var order = new Order
                {
                    FoodOrderID = foodOrderID,
                    OrderDateTime = DateTime.Now
                };

            _context.Order.Add(order);

            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }*/
    }

}