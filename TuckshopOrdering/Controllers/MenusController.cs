using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MimeKit;
using MimeKit.Text;
using TuckshopOrdering.Areas.Identity.Data;
using TuckshopOrdering.Migrations;
using TuckshopOrdering.Models;
using MailKit.Security;
using System.Net.Mail;
using System.Net;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.AspNetCore.Authorization;

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
        public async Task<IActionResult> Index(int categoryId, string searchString, int? orderId = null)
        {
            TempData["CurrentCategoryID"] = categoryId; 

            var categories = await _context.Category.ToListAsync();
            var foodOrders = await _context.FoodOrder
                                            .Include(n => n.Order)
                                            .Where(n => n.Order.Status != "Completed")
                                            .ToListAsync();

            var menu = await _context.Menu.Include(m => m.Category)
                                          .Include(m => m.FoodOrders)
                                          .ToListAsync();

            if (categoryId != 0) // category filter
            {
                menu = await _context.Menu.Include(m => m.Category).Where(o => o.CategoryID == categoryId)
                                          .Include(m => m.FoodOrders)
                                          .ToListAsync();
            }

             // filtering query

            if (!String.IsNullOrEmpty(searchString))
            {
                menu = await _context.Menu.Where(o => o.foodName.Contains(searchString)).ToListAsync();
            }

            MenuViewModel mvm = new MenuViewModel()
            {
                _Menu = menu,
                _Category = categories,
                _FoodOrder = foodOrders,
            };

            ViewBag.OrderID = orderId;
            switch (categoryId)
            {
                case 1:
                    ViewBag.SelectedCategoryID = "Hot Food";
                    break;
                case 2:
                    ViewBag.SelectedCategoryID = "Snacks";
                    break;
                case 3:
                    ViewBag.SelectedCategoryID = "Sandwiches";
                    break;
                case 4:
                    ViewBag.SelectedCategoryID = "Drinks & Iceblocks";
                    break;
            }
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
                .FirstOrDefaultAsync(m => m.MenuID == id);
            if (menu == null)
            {
                return NotFound();
            }

            return View(menu);
        }

        // GET: Menus/Create
        [Authorize]
        public IActionResult Create()
        {
            ViewData["CategoryID"] = new SelectList(_context.Category, "CategoryID", "CategoryName");
            ViewData["OrderID"] = new SelectList(_context.Order, "OrderID", "OrderID");
            return View();
        }

        // POST: Menus/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MenuID,foodName,price,imageFile,CategoryID,homePageDisplay,OrderID")] Menu menu)
        {
            if (!ModelState.IsValid)
            {
                // saves image to wwwroot/images
                string wwwRootPath = _hostEnviroment.WebRootPath;
                string fileName = Path.GetFileNameWithoutExtension(menu.imageFile.FileName);
                string extension = Path.GetExtension(menu.imageFile.FileName);
                menu.imageName = fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                string path = Path.Combine(wwwRootPath + "/menuImages", fileName);
                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    await menu.imageFile.CopyToAsync(fileStream);
                }
                // Insert record
                _context.Add(menu);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryID"] = new SelectList(_context.Category, "CategoryID", "CategoryName", menu.CategoryID);
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
            return View(menu);
        }

		// POST: Menus/Edit/5
		// To protect from overposting attacks, enable the specific properties you want to bind to.
		// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(int id, [Bind("MenuID,foodName,price,imageName,CategoryID,homePageDisplay,imageFile")] Menu menu)
		{
			if (id != menu.MenuID)
			{
				return NotFound();
			}

			if (ModelState.IsValid)
			{
				try
				{
					if (menu.imageFile != null)
					{
						// Save the new image
						string wwwRootPath = _hostEnviroment.WebRootPath;
						string fileName = Path.GetFileNameWithoutExtension(menu.imageFile.FileName);
						string extension = Path.GetExtension(menu.imageFile.FileName);
						fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
						string path = Path.Combine(wwwRootPath + "/menuImages", fileName);

						// Delete the old image
						if (!string.IsNullOrEmpty(menu.imageName))
						{
							var oldImagePath = Path.Combine(wwwRootPath, "Images", menu.imageName);
							if (System.IO.File.Exists(oldImagePath))
							{
								System.IO.File.Delete(oldImagePath);
							}
						}

						// Save the new image
						using (var fileStream = new FileStream(path, FileMode.Create))
						{
							await menu.imageFile.CopyToAsync(fileStream);
						}
						menu.imageName = fileName;
					}

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
        public async Task<IActionResult> AddToOrder(int menuItemID, string studentName, string customiseMessage)
        {

            var menuItem = await _context.Menu.FindAsync(menuItemID);
            if (menuItem == null)
            {
                return NotFound();
            }

            // Retrieve or create order
            var orderId = HttpContext.Session.GetInt32("OrderId");
            Order order;

            if (orderId.HasValue)
            {
                order = await _context.Order.FindAsync(orderId.Value);
                if (order == null)
                {
                    return NotFound();
                }
            }
            else
            {
                order = new Order
                {
                    OrderDate = DateTime.Now,
                    studentName = "Jonathan Santos",
                    roomNumber = 1,
                    orderComplete = "false",
                    FoodOrders = new List<FoodOrder>()
                };
                _context.Order.Add(order);
                await _context.SaveChangesAsync();

                // Store the new orderId in session
                HttpContext.Session.SetInt32("OrderId", order.OrderID);
            }

            var existingOrder = await _context.FoodOrder.FirstOrDefaultAsync(o => o.MenuID == menuItemID && o.OrderID == order.OrderID);

            if (existingOrder != null)
            {
                existingOrder.quantity += 1; // Item already exists, so increase quantity
            }
            else
            {
                var foodOrder = new FoodOrder
                {
                    MenuID = menuItemID,
                    quantity = 1,
                    OrderID = order.OrderID
                };

                _context.FoodOrder.Add(foodOrder);
                order.FoodOrders.Add(foodOrder);
            }

            await _context.SaveChangesAsync();

            int categoryID = TempData.ContainsKey("CurrentCategoryID") ? (int)TempData["CurrentCategoryID"] : 0;

            return RedirectToAction("Index", new { categoryId = categoryID });
        }


        [HttpPost]
        public async Task<IActionResult> DecreaseQuantity(int foodOrderID, int? orderId)
        {
            var foodOrder = await _context.FoodOrder.FindAsync(foodOrderID);
            //var menuItem = await _context.Menu.FindAsync(categoryID);

            if (foodOrder != null && foodOrder.quantity > 1)
            {
                foodOrder.quantity -= 1;
                await _context.SaveChangesAsync();
            }

            int categoryID = TempData.ContainsKey("CurrentCategoryID") ? (int)TempData["CurrentCategoryID"] : 0;

            return RedirectToAction("Index", new { categoryId = categoryID, orderId });
        }

        [HttpPost]
        public async Task<IActionResult> IncreaseQuantity(int foodOrderID, int? orderId)
        {
            var foodOrder = await _context.FoodOrder.FindAsync(foodOrderID);

            if (foodOrder != null)
            {
                foodOrder.quantity += 1;
                await _context.SaveChangesAsync();
            }

            // filter page

            int categoryID = TempData.ContainsKey("CurrentCategoryID") ? (int)TempData["CurrentCategoryID"] : 0;

            return RedirectToAction("Index", new { categoryId = categoryID, orderId });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteItem(int foodOrderID, int? orderId)
        {
            var foodOrder = await _context.FoodOrder.FindAsync(foodOrderID);

            if (foodOrder != null)
            {
                _context.FoodOrder.Remove(foodOrder);
                await _context.SaveChangesAsync();
            }

            int categoryID = TempData.ContainsKey("CurrentCategoryID") ? (int)TempData["CurrentCategoryID"] : 0;

            return RedirectToAction("Index", new { categoryId = categoryID, orderId });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteAllItems()
        {
            var orderId = HttpContext.Session.GetInt32("OrderId");
            if (!orderId.HasValue)
            {
                return RedirectToAction("Index");
            }

            var order = await _context.Order.Include(o => o.FoodOrders).FirstOrDefaultAsync(o => o.OrderID == orderId.Value);
            if (order == null)
            {
                return NotFound("Order not found.");
            }

            _context.FoodOrder.RemoveRange(order.FoodOrders); // Remove all associated food orders
            _context.Order.Remove(order); // Remove the current order

            await _context.SaveChangesAsync();

            // Clear the session
            HttpContext.Session.Remove("OrderId");

            return RedirectToAction("Index", new { categoryId = 0 });
        }

        [HttpPost]
        public async Task<IActionResult> CompleteOrder(string studentName, int roomNumber, DateTime collectionDate, string? email, string note, TuckshopOrdering.Models.EmailEntity model)
        {
            // Complete the order
            var orderId = HttpContext.Session.GetInt32("OrderId");
            if (!orderId.HasValue)
            {
                return RedirectToAction("Index");
            }

            var order = await _context.Order.FindAsync(orderId.Value);
            if (order == null)
            {
                return NotFound();
            }

            order.Status = "Completed"; // ordering has been done
            order.studentName = studentName; // user input
            order.roomNumber = roomNumber; // user input
            if(collectionDate == DateTime.MinValue)
            {
                order.PickupDate = DateTime.Now;
            }
            else
            {
                order.PickupDate = collectionDate;
            }
            order.email = email; // user input
            order.orderComplete = "false"; // order has not been made
            order.note = note; // user input

            // email

            //decimal totalPrice = order.FoodOrders.Sum(fo => fo.quantity * fo.Menu.price);


            // Generate the email body
          
            string mailBody = "<h1 style='text-align: center;'>Thank you for your order!</h1>";
            string mainTitle = "Thank you for you order!";
            string mailSubject = "Tuckshop Order";
            string fromMail = "test@gmail.com";
            string mailPassword = "test";

            if (!email.IsNullOrEmpty())
            {
                MailMessage message = new MailMessage(new MailAddress(fromMail, mainTitle), new MailAddress(email));

                message.Subject = mailSubject;
                message.Body = mailBody;
                message.IsBodyHtml = true;

                SmtpClient smtp = new SmtpClient(fromMail, 587);
                smtp.Host = "smtp.office365.com";
                smtp.Port = 587;
                smtp.EnableSsl = true;
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;

                System.Net.NetworkCredential credential = new System.Net.NetworkCredential();
                credential.UserName = fromMail;
                credential.Password = mailPassword;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = credential;

                smtp.Send(message);
            }            

            // save changes
            _context.Update(order);
            await _context.SaveChangesAsync();

            // Clear the session
            HttpContext.Session.Remove("OrderId");

			return View("OrderPlaced");
        }

		[HttpPost]
		public async Task<IActionResult> DeleteOrderOnExit()
		{
			var orderId = HttpContext.Session.GetInt32("OrderId");

			if (orderId.HasValue)
			{
				var order = await _context.Order.Include(o => o.FoodOrders)
												.FirstOrDefaultAsync(o => o.OrderID == orderId.Value);

				if (order != null)
				{
					_context.FoodOrder.RemoveRange(order.FoodOrders);
					_context.Order.Remove(order);
					await _context.SaveChangesAsync();
				}

				HttpContext.Session.Remove("OrderId");
			}

			return Ok();
		}

	}
}