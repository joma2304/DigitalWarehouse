using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DigitalWarehouse.Data;
using DigitalWarehouse.Models;
using Microsoft.AspNetCore.Authorization;

namespace DigitalWarehouse.Controllers
{
    [Authorize(Roles = "Admin, Worker")] //Skyddad för admin och worker
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductController(ApplicationDbContext context)
        {
            _context = context;
        }

// GET: Product
public async Task<IActionResult> Index(string searchString, string sortOrder)
{
    ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
    ViewData["PriceSortParm"] = sortOrder == "Price" ? "price_desc" : "Price";
    ViewData["DescriptionSortParm"] = sortOrder == "Description" ? "description_desc" : "Description";
    ViewData["CategorySortParm"] = sortOrder == "Category" ? "category_desc" : "Category";
    ViewData["AmountSortParm"] = sortOrder == "Amount" ? "amount_desc" : "Amount";

    var products = _context.Products.Include(p => p.Category).AsQueryable();

    if (!string.IsNullOrEmpty(searchString))
    {
        products = products.Where(p => p.Name.ToLower().Contains(searchString.ToLower()) ||
                                        p.Description.ToLower().Contains(searchString.ToLower()) ||
                                        p.Category.Name.ToLower().Contains(searchString.ToLower())); // Går att söka efter namn, beskrivning och kategori
    }

    // Sorteringar
    switch (sortOrder)
    {
        case "name_desc":
            products = products.OrderByDescending(p => p.Name);
            break;
        case "Price":
            products = products.OrderBy(p => p.Price);
            break;
        case "price_desc":
            products = products.OrderByDescending(p => p.Price);
            break;
        case "Description":
            products = products.OrderBy(p => p.Description);
            break;
        case "description_desc":
            products = products.OrderByDescending(p => p.Description);
            break;
        case "Category":
            products = products.OrderBy(p => p.Category.Name);
            break;
        case "category_desc":
            products = products.OrderByDescending(p => p.Category.Name);
            break;
        case "Amount":
            products = products.OrderBy(p => p.Amount);
            break;
        case "amount_desc":
            products = products.OrderByDescending(p => p.Amount);
            break;
        default:
            products = products.OrderBy(p => p.Name);
            break;
    }

    return View(await products.ToListAsync());
}



        // GET: Product/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productModel = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (productModel == null)
            {
                return NotFound();
            }

            return View(productModel);
        }

        // GET: Product/Create
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name");
            return View();
        }

        // POST: Product/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Description,Price,Amount,CategoryId")] ProductModel productModel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(productModel);
                await _context.SaveChangesAsync();

                // Skapa en lagersaldoändring
                var stockChange = new StockChangeModel
                {
                    ProductId = productModel.Id,
                    ChangeAmount = productModel.Amount, // Antal från början när en produkt skapats
                    ChangeDate = DateTime.UtcNow
                };

                _context.StockChanges.Add(stockChange);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", productModel.CategoryId);
            return View(productModel);
        }

        // GET: Product/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productModel = await _context.Products.FindAsync(id);
            if (productModel == null)
            {
                return NotFound();
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", productModel.CategoryId);
            return View(productModel);
        }

        // POST: Product/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,Price,Amount,CategoryId")] ProductModel productModel)
        {
            if (id != productModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingProduct = await _context.Products.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
                    if (existingProduct == null)
                    {
                        return NotFound();
                    }

                    int difference = productModel.Amount - existingProduct.Amount; //skillnad mellan nuvarande amount och det nya amount

                    _context.Update(productModel);
                    await _context.SaveChangesAsync();

                    // Skapa en lagersaldoändring om Amount har förändrats för produkt
                    if (difference != 0)
                    {
                        var stockChange = new StockChangeModel
                        {
                            ProductId = productModel.Id,
                            ChangeAmount = difference, //skillnaden i amount blir förändningen
                            ChangeDate = DateTime.UtcNow
                        };

                        _context.StockChanges.Add(stockChange);
                        await _context.SaveChangesAsync();
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductModelExists(productModel.Id))
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
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", productModel.CategoryId);
            return View(productModel);
        }

        // GET: Product/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productModel = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (productModel == null)
            {
                return NotFound();
            }

            return View(productModel);
        }

        [Authorize(Roles = "Admin")]
        // POST: Product/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var productModel = await _context.Products.FindAsync(id);
            if (productModel != null)
            {
                _context.Products.Remove(productModel);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductModelExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }
    }
}
