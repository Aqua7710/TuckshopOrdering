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
    public class CustomisesController : Controller
    {
        private readonly TuckshopOrderingSystem _context;

        public CustomisesController(TuckshopOrderingSystem context)
        {
            _context = context;
        }

        // GET: Customises
        public async Task<IActionResult> Index()
        {
              return _context.Customise != null ? 
                          View(await _context.Customise.ToListAsync()) :
                          Problem("Entity set 'TuckshopOrderingSystem.Customise'  is null.");
        }

        // GET: Customises/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Customise == null)
            {
                return NotFound();
            }

            var customise = await _context.Customise
                .FirstOrDefaultAsync(m => m.CustomiseID == id);
            if (customise == null)
            {
                return NotFound();
            }

            return View(customise);
        }

        // GET: Customises/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Customises/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CustomiseID,CustomiseName")] Customise customise)
        {
            if (ModelState.IsValid)
            {
                _context.Add(customise);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(customise);
        }

        // GET: Customises/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Customise == null)
            {
                return NotFound();
            }

            var customise = await _context.Customise.FindAsync(id);
            if (customise == null)
            {
                return NotFound();
            }
            return View(customise);
        }

        // POST: Customises/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CustomiseID,CustomiseName")] Customise customise)
        {
            if (id != customise.CustomiseID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(customise);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CustomiseExists(customise.CustomiseID))
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
            return View(customise);
        }

        // GET: Customises/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Customise == null)
            {
                return NotFound();
            }

            var customise = await _context.Customise
                .FirstOrDefaultAsync(m => m.CustomiseID == id);
            if (customise == null)
            {
                return NotFound();
            }

            return View(customise);
        }

        // POST: Customises/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Customise == null)
            {
                return Problem("Entity set 'TuckshopOrderingSystem.Customise'  is null.");
            }
            var customise = await _context.Customise.FindAsync(id);
            if (customise != null)
            {
                _context.Customise.Remove(customise);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CustomiseExists(int id)
        {
          return (_context.Customise?.Any(e => e.CustomiseID == id)).GetValueOrDefault();
        }
    }
}
