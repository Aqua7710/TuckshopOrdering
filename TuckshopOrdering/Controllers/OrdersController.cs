using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TuckshopOrdering.Areas.Identity.Data;
using TuckshopOrdering.Models;

namespace TuckshopOrdering.Controllers
{
    public class OrdersController : Controller
    {
        private readonly TuckshopOrderingSystem _context;

        public OrdersController(TuckshopOrderingSystem context)
        {
            _context = context;
        }

        // GET: Orders
        public async Task<IActionResult> Index(string searchString)
        {
            // Retrieve all orders from the database, including their associated FoodOrders and Menu items
            var orders = from o in _context.Order
                         .Include(o => o.FoodOrders) // Include related FoodOrders
                         .ThenInclude(fo => fo.Menu) // Include Menu details within each FoodOrder
                         select o;

            // filtering to get todays orders
            var todaysOrders = orders.Where(o => o.PickupDate == DateTime.Now);

            // Check if a search string is provided by the user
            if (!String.IsNullOrEmpty(searchString))
            {
                // If the search string can be parsed as an integer, assume it is a room number
                if (int.TryParse(searchString, out int roomNumber))
                {
                    // Filter orders where the room number matches the search
                    orders = orders.Where(o => o.roomNumber == roomNumber);
                }
                else
                {
                    // Otherwise, assume the search string is a student's name and perform a case-insensitive search
                    orders = orders.Where(o => o.studentName.Contains(searchString));
                }
            }

            // Order the resulting list of orders first by PickupDate, then by room number
            orders = orders.OrderBy(o => o.PickupDate).ThenBy(o => o.roomNumber);

            // Create a dictionary to hold each order's associated FoodOrders list, with the OrderID as the key
            var foodOrders = new Dictionary<int, List<FoodOrder>>();

            // Iterate over each order to map its OrderID to its list of FoodOrders
            foreach (var order in orders)
            {
                // Store the FoodOrders for the current order in the dictionary
                foodOrders[order.OrderID] = order.FoodOrders.ToList();
            }

            // Create a view model to pass both the orders and their associated FoodOrders to the view
            OrderViewModel ovm = new OrderViewModel()
            {
                Orders = await orders.ToListAsync(),    // Convert the query result into a list asynchronously
                FoodOrders = foodOrders                 // Pass the dictionary of FoodOrders
            };

            // Return the view with the populated view model
            return View(ovm);
        }

        // GET: Orders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Order == null)
            {
                return NotFound();
            }

            var order = await _context.Order
                .FirstOrDefaultAsync(m => m.OrderID == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // GET: Orders/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Orders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("OrderID,OrderDate,PickupDate")] Order order)
        {
            if (ModelState.IsValid)
            {
                _context.Add(order);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(order);
        }

        // GET: Orders/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Order == null)
            {
                return NotFound();
            }

            var order = await _context.Order.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            return View(order);
        }

        // POST: Orders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("OrderID,OrderDate,PickupDate")] Order order)
        {
            if (id != order.OrderID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(order);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(order.OrderID))
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
            return View(order);
        }

        // GET: Orders/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Order == null)
            {
                return NotFound();
            }

            var order = await _context.Order
                .FirstOrDefaultAsync(m => m.OrderID == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Order == null)
            {
                return Problem("Entity set 'TuckshopOrderingSystem.Order'  is null.");
            }
            var order = await _context.Order.FindAsync(id);
            if (order != null)
            {
                _context.Order.Remove(order);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrderExists(int id)
        {
          return (_context.Order?.Any(e => e.OrderID == id)).GetValueOrDefault();
        }

		[HttpPost]
		public async Task<IActionResult> OrderComplete(int orderID)
		{
			var order = await _context.Order.FindAsync(orderID);
			if (order == null)
			{
				return NotFound();
			}

			order.orderComplete = "true";
			_context.Update(order);
			await _context.SaveChangesAsync();

			return RedirectToAction("Index");
		}


	}
}
