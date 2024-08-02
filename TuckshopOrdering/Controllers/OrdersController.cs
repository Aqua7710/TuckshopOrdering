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
    [Authorize]
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
            var orders = from o in _context.Order.Include(o => o.FoodOrders).ThenInclude(fo => fo.Menu)
                         select o;

            var todayOrders = from o in _context.Order.Include(o => o.FoodOrders).Where(o => o.PickupDate == DateTime.Now) select o;

            var todaysOrders = orders.Where(o => o.PickupDate == DateTime.Now);

            // filtering query

            if (!String.IsNullOrEmpty(searchString))
            {
                // Check if the search string can be parsed to an integer
                if (int.TryParse(searchString, out int roomNumber))
                {
                    orders = orders.Where(o => o.roomNumber == roomNumber);
                }
                else
                {
                    orders = orders.Where(o => o.studentName.Contains(searchString));
                }
            }

            // sorting query 

            orders = orders.OrderBy(o => o.PickupDate).ThenBy(o => o.roomNumber);

            var foodOrders = new Dictionary<int, List<FoodOrder>>();

            foreach (var order in orders)
            {
                foodOrders[order.OrderID] = order.FoodOrders.ToList();
            }

            OrderViewModel ovm = new OrderViewModel()
            {
                Orders = await orders.ToListAsync(),
                FoodOrders = foodOrders
            };

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
