using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DigitalWarehouse.Data;
using DigitalWarehouse.Models;

namespace DigitalWarehouse.Controllers
{
    public class StockChangeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StockChangeController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: StockChange
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.StockChanges.Include(s => s.Product);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: StockChange/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var stockChangeModel = await _context.StockChanges
                .Include(s => s.Product)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (stockChangeModel == null)
            {
                return NotFound();
            }

            return View(stockChangeModel);
        }

        // GET: StockChange/Create
        public IActionResult Create()
        {
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Name");
            return View();
        }

        // POST: StockChange/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ProductId,ChangeAmount,ChangeDate")] StockChangeModel stockChangeModel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(stockChangeModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Name", stockChangeModel.ProductId);
            return View(stockChangeModel);
        }

        // GET: StockChange/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var stockChangeModel = await _context.StockChanges.FindAsync(id);
            if (stockChangeModel == null)
            {
                return NotFound();
            }
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Name", stockChangeModel.ProductId);
            return View(stockChangeModel);
        }

        // POST: StockChange/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ProductId,ChangeAmount,ChangeDate")] StockChangeModel stockChangeModel)
        {
            if (id != stockChangeModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(stockChangeModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StockChangeModelExists(stockChangeModel.Id))
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
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Name", stockChangeModel.ProductId);
            return View(stockChangeModel);
        }

        // GET: StockChange/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var stockChangeModel = await _context.StockChanges
                .Include(s => s.Product)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (stockChangeModel == null)
            {
                return NotFound();
            }

            return View(stockChangeModel);
        }

        // POST: StockChange/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var stockChangeModel = await _context.StockChanges.FindAsync(id);
            if (stockChangeModel != null)
            {
                _context.StockChanges.Remove(stockChangeModel);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool StockChangeModelExists(int id)
        {
            return _context.StockChanges.Any(e => e.Id == id);
        }
    }
}
