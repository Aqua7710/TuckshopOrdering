using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TuckshopOrdering.Areas.Identity.Data;
using TuckshopOrdering.Models;

namespace TuckshopOrdering.Controllers
{
    public class FoodOrdersController : Controller
    {
        private readonly TuckshopOrderingSystem _context;

        public FoodOrdersController(TuckshopOrderingSystem context)
        {
            _context = context;
        }

        // GET: FoodOrders
        public async Task<IActionResult> Index()
        {
            var tuckshopOrderingSystem = _context.FoodOrder.Include(f => f.Menu).Include(f => f.Order);
            return View(await tuckshopOrderingSystem.ToListAsync());
        }

        // GET: FoodOrders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.FoodOrder == null)
            {
                return NotFound();
            }

            var foodOrder = await _context.FoodOrder
                .Include(f => f.Menu).Include(f => f.Order)
                .FirstOrDefaultAsync(m => m.FoodOrderID == id);
            if (foodOrder == null)
            {
                return NotFound();
            }

            return View(foodOrder);
        }

        // GET: FoodOrders/Create
        public IActionResult Create()
        {
            ViewData["MenuID"] = new SelectList(_context.Menu, "MenuID", "MenuID");
            ViewData["OrderID"] = new SelectList(_context.Menu, "OrderID", "OrderID");
            return View();
        }

        // POST: FoodOrders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FoodOrderID,MenuID,quantity,studentName,roomNumber,OrderID")] FoodOrder foodOrder)
        {
            if (ModelState.IsValid)
            {
                _context.Add(foodOrder);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["MenuID"] = new SelectList(_context.Menu, "MenuID", "MenuID", foodOrder.MenuID);
            ViewData["OrderID"] = new SelectList(_context.Menu, "OrderID", "OrderID", foodOrder.Order.OrderID);
            return View(foodOrder);
        }

        // GET: FoodOrders/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.FoodOrder == null)
            {
                return NotFound();
            }

            var foodOrder = await _context.FoodOrder.FindAsync(id);
            if (foodOrder == null)
            {
                return NotFound();
            }
            ViewData["MenuID"] = new SelectList(_context.Menu, "MenuID", "MenuID", foodOrder.MenuID);
            ViewData["OrderID"] = new SelectList(_context.Menu, "OrderID", "OrderID", foodOrder.Order.OrderID);
            return View(foodOrder);
        }

        // POST: FoodOrders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("FoodOrderID,MenuID,quantity,studentName,roomNumber,OrderID")] FoodOrder foodOrder)
        {
            if (id != foodOrder.FoodOrderID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(foodOrder);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FoodOrderExists(foodOrder.FoodOrderID))
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
            ViewData["MenuID"] = new SelectList(_context.Menu, "MenuID", "MenuID", foodOrder.MenuID);
            ViewData["OrderID"] = new SelectList(_context.Menu, "OrderID", "OrderID", foodOrder.Order.OrderID);
            return View(foodOrder);
        }

        // GET: FoodOrders/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.FoodOrder == null)
            {
                return NotFound();
            }

            var foodOrder = await _context.FoodOrder
                .Include(f => f.Menu).Include(f => f.Order)
                .FirstOrDefaultAsync(m => m.FoodOrderID == id);
            if (foodOrder == null)
            {
                return NotFound();
            }

            return View(foodOrder);
        }

        // POST: FoodOrders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.FoodOrder == null)
            {
                return Problem("Entity set 'TuckshopOrderingSystem.FoodOrder'  is null.");
            }
            var foodOrder = await _context.FoodOrder.FindAsync(id);
            if (foodOrder != null)
            {
                _context.FoodOrder.Remove(foodOrder);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FoodOrderExists(int id)
        {
          return (_context.FoodOrder?.Any(e => e.FoodOrderID == id)).GetValueOrDefault();
        }
    }
}
