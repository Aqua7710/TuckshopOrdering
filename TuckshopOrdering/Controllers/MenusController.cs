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
            // stores the passed category id in a tempdata dictionary which is used to pass the value across action methods
            TempData["CurrentCategoryID"] = categoryId; 

            // retrieves all categories from database and stores them in a list
            var categories = await _context.Category.ToListAsync(); 

            // retrieves all food orders that are NOT completed and also retrieves their associated order and stores them in a list
            var foodOrders = await _context.FoodOrder
                                            .Include(n => n.Order)
                                            .Where(n => n.Order.Status != "Completed")
                                            .ToListAsync();

            // retrieves all menu items from the database and also retrieves their associated category and food orders and stores them in a list
            var menu = await _context.Menu.Include(m => m.Category)
                                          .Include(m => m.FoodOrders)
                                          .ToListAsync();

            // if the passed category id is NOT null and not zero, them run the if statment code 
            if (categoryId != 0) 
            {
                // retrieves all menu items from database where the category ID property matches the passed category ID and stores it in a list
                menu = await _context.Menu.Include(m => m.Category).Where(o => o.CategoryID == categoryId)
                                          .Include(m => m.FoodOrders)
                                          .ToListAsync();
            }


            // if the passed search string value is not null or empty, then run the if statemnt code
            if (!String.IsNullOrEmpty(searchString))
            {
                // retrieves all menu items from database where the food name contains the search string variable and stores the results in a list
                menu = await _context.Menu.Where(o => o.foodName.Contains(searchString)).ToListAsync();
            }

            // a viewmodel to pass the menu, categories, and food orders to the view 
            MenuViewModel mvm = new MenuViewModel()
            {
                _Menu = menu,
                _Category = categories,
                _FoodOrder = foodOrders,
            };

            // stores the order ID in a viewbag in order to pass it to the view and display it
            ViewBag.OrderID = orderId;

            // depending on which category ID is selected, a particular case will run and set the viewbag based off the selected category ID in order to display it in the view
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

            // returns view with populated view model 
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

        // custom action method called if the 'add' button is pressed 
        [HttpPost]
        public async Task<IActionResult> AddToOrder(int menuItemID, string studentName, string customiseMessage)
        {
            // fimds the menu item in the database based on the passed parameter value 
            var menuItem = await _context.Menu.FindAsync(menuItemID);

            // error message if menu item passed does not exist
            if (menuItem == null)
            {
                return NotFound();
            }

            // Retrieves the current order ID in the session, if it exists 
            var orderId = HttpContext.Session.GetInt32("OrderId");
            Order order;

            // checks if an order already exists 
            if (orderId.HasValue) 
            {
                // if it does exist, find the order in the database using the order id that's been stored in the session
                order = await _context.Order.FindAsync(orderId.Value);

                // error message if order does not exist
                if (order == null)
                {
                    return NotFound();
                }
            }
            else // in this case, an order does not exist and we are creating a new order
            { 
                // create new order with initial details (these details will change)
                order = new Order 
                {
                    OrderDate = DateTime.Now, // order date set to current date
                    studentName = "Jonathan Santos", // initial student name (will change)
                    roomNumber = 1, // initial room number (will change)
                    orderComplete = "false", // marks the order as incomplete 
                    FoodOrders = new List<FoodOrder>() // initializes an empty list for the food orders
                };

                // add the new order to the database and save changes
                _context.Order.Add(order);
                await _context.SaveChangesAsync();

                // Store the new orderId in session for future reference 
                HttpContext.Session.SetInt32("OrderId", order.OrderID);
            }

            // checks if the food item (menu ID) that is trying to be added already exists in the current order 
            var existingOrder = await _context.FoodOrder.FirstOrDefaultAsync(o => o.MenuID == menuItemID && o.OrderID == order.OrderID);

            // if the item already exists in the order
            if (existingOrder != null)
            {
                // Item already exists, so increase quantity
                existingOrder.quantity += 1; 
            }
            else // if the item does not exist within the order 
            {
                // creates a new food order record
                var foodOrder = new FoodOrder
                {
                    MenuID = menuItemID, // sets the menu ID to the passed menu item 
                    quantity = 1, // sets the initial quantity to 1 
                    OrderID = order.OrderID // associates the food order with the current order 
                };

                // adds the new food order to the database AND to the food orders list which was initialized earlier 
                _context.FoodOrder.Add(foodOrder);
                order.FoodOrders.Add(foodOrder);
            }

            // saves changes to the database 
            await _context.SaveChangesAsync();

            // retrieves the current category ID from the temp data, if it exists, or defaults to 0
            int categoryID = TempData.ContainsKey("CurrentCategoryID") ? (int)TempData["CurrentCategoryID"] : 0;

            // redirects to the index action passing the category ID as a parameter (in order to return to the filtered page when the page refreshes instead of defaulting to the non-filtered view)
            return RedirectToAction("Index", new { categoryId = categoryID });
        }

        // custom action method called if the 'decrease quantity' button is pressed
        [HttpPost]
        public async Task<IActionResult> DecreaseQuantity(int foodOrderID, int? orderId)
        {
            // finds the food order in the database based on the passed parameter value 
            var foodOrder = await _context.FoodOrder.FindAsync(foodOrderID);

            // if the food order exists AND the quantity of the passed food order record is greater than 1...
            if (foodOrder != null && foodOrder.quantity > 1)
            {
                // decrease the quantity by 1 and saves the change to the database
                foodOrder.quantity -= 1;
                await _context.SaveChangesAsync();
            }

            // retrieves the current category ID from the temp data, if it exists, or defaults to 0
            int categoryID = TempData.ContainsKey("CurrentCategoryID") ? (int)TempData["CurrentCategoryID"] : 0;

            // redirects to the index action passing the category ID as a parameter (in order to return to the filtered page when the page refreshes instead of defaulting to the non-filtered view)
            return RedirectToAction("Index", new { categoryId = categoryID, orderId });
        }

        // custom action method called if the 'increase quantity' button is pressed 
        [HttpPost]
        public async Task<IActionResult> IncreaseQuantity(int foodOrderID, int? orderId)
        {
            // finds the food order in the database based on the passed parameter value
            var foodOrder = await _context.FoodOrder.FindAsync(foodOrderID);

            // if the food order exists...
            if (foodOrder != null)
            {
                // increase the quantity of the passed food order by 1 and save the change to the database 
                foodOrder.quantity += 1;
                await _context.SaveChangesAsync();
            }

            // retrieves the current category ID from the temp data, if it exists, or defaults to 0
            int categoryID = TempData.ContainsKey("CurrentCategoryID") ? (int)TempData["CurrentCategoryID"] : 0;

            // redirects to the index action passing the category ID as a parameter (in order to return to the filtered page when the page refreshes instead of defaulting to the non-filtered view)
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
        public async Task<IActionResult> CompleteOrder(string studentName, int roomNumber, DateTime collectionDate, string email, string note, TuckshopOrdering.Models.EmailEntity model)
        {
            // Complete the order
            var orderId = HttpContext.Session.GetInt32("OrderId");
            if (!orderId.HasValue)
            {
                return RedirectToAction("Index");
            }

            var order = await _context.Order
                .Include(o => o.FoodOrders)
                .ThenInclude(fo => fo.Menu)
                .FirstOrDefaultAsync(o => o.OrderID == orderId.Value);

            if (order == null)
            {
                return NotFound();
            }

            order.Status = "Completed"; // Mark the order as completed
            order.studentName = studentName; // User input
            order.roomNumber = roomNumber; // User input
            order.PickupDate = collectionDate == DateTime.MinValue ? DateTime.Now : collectionDate; // Set collection date
            order.email = email; // User input
            order.orderComplete = "false"; // Order has not been made
            order.note = note; // User input

            // Calculate total price
            decimal totalPrice = order.FoodOrders.Sum(fo => fo.quantity * fo.Menu.price);

            // Generate the email body
            string mailBody = "<h1 style='text-align: center;'>Thank you for your order!</h1>";
            mailBody += $"<p><strong>Student Name:</strong> {studentName}</p>";
            mailBody += $"<p><strong>Room Number:</strong> {roomNumber}</p>";
            mailBody += "<table style='width: 100%; border-collapse: collapse;'>";
            mailBody += "<tr><th style='border: 1px solid #dddddd; text-align: left; padding: 8px;'>Item</th><th style='border: 1px solid #dddddd; text-align: left; padding: 8px;'>Quantity</th><th style='border: 1px solid #dddddd; text-align: left; padding: 8px;'>Price</th></tr>";

            foreach (var item in order.FoodOrders)
            {
                mailBody += $"<tr><td style='border: 1px solid #dddddd; text-align: left; padding: 8px;'>{item.Menu.foodName}</td><td style='border: 1px solid #dddddd; text-align: left; padding: 8px;'>{item.quantity}</td><td style='border: 1px solid #dddddd; text-align: left; padding: 8px;'>${item.quantity * item.Menu.price:F2}</td></tr>";
            }

            mailBody += "</table>";
            mailBody += $"<p><strong>Total Price:</strong> ${totalPrice:F2}</p>";

            // Optional: Add note if available
            if (!string.IsNullOrEmpty(note))
            {
                mailBody += $"<p><strong>Note:</strong> {note}</p>";
            }

            // Email configuration
            string mainTitle = "Thank you for your order!";
            string mailSubject = "Tuckshop Order Confirmation";
            string fromMail = "email";
            string mailPassword = "Password";

            if(!email.IsNullOrEmpty()) 
            {
                MailMessage message = new MailMessage(new MailAddress(fromMail, mainTitle), new MailAddress(email))
                {
                    Subject = mailSubject,
                    Body = mailBody,
                    IsBodyHtml = true
                };

                SmtpClient smtp = new SmtpClient("smtp.office365.com", 587)
                {
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(fromMail, mailPassword)
                };

                smtp.Send(message);
            }


            // Save changes
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