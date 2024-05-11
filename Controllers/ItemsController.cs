﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ExpensesCalculator.Data;
using ExpensesCalculator.Models;

namespace ExpensesCalculator.Controllers
{
    public class ItemsController : Controller
    {
        private readonly ExpensesContext _context;

        public ItemsController(ExpensesContext context)
        {
            _context = context;
        }

        // GET: Items
        public async Task<IActionResult> Index()
        {
            return View(await _context.Items.ToListAsync());
        }

        // GET: Items/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var item = await _context.Items
                .FirstOrDefaultAsync(m => m.Id == id);
            if (item == null)
            {
                return NotFound();
            }

            return View(item);
        }

        // GET: Items/Create
        [HttpGet]
        public IActionResult Create(int checkId)
        {
            ViewData["CheckId"] = checkId;
            return View();
        }

        // POST: Items/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Description,Price,Id")] Item item, int checkId)
        {
            if (ModelState.IsValid)
            {
                var check = await _context.Checks.Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.Id == checkId);

                if (check is null)
                {
                    return NotFound();
                }

                _context.Items.Add(item);
                check.Items.Add(item);
                check.Sum += item.Price;
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(item);
        }

        // GET: Items/Edit/5
        public async Task<IActionResult> Edit(int? id, int checkId)
        {
            if (id is null)
            {
                return NotFound();
            }

            var item = await _context.Items.FindAsync(id);
            if (item is null)
            {
                return NotFound();
            }

            ViewData["CheckId"] = checkId;
            return View(item);
        }

        // POST: Items/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Name,Description,Price,Id")] Item item, int checkId)
        {
            if (id != item.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var check = await _context.Checks
                        .FirstOrDefaultAsync(m => m.Id == checkId);

                    if (check is null)
                    {
                        return NotFound();
                    }

                    var oldItem = await _context.Items.AsNoTracking().FirstOrDefaultAsync(i => i.Id == item.Id);

                    if (oldItem is null)
                    {
                        return NotFound();
                    }

                    check.Sum -= oldItem.Price;
                    _context.Update(item);
                    check.Sum += item.Price;
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ItemExists(item.Id))
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
            return View(item);
        }

        // GET: Items/Delete/5
        public async Task<IActionResult> Delete(int? id, int checkId)
        {
            if (id == null)
            {
                return NotFound();
            }

            var item = await _context.Items
                .FirstOrDefaultAsync(m => m.Id == id);

            if (item == null)
            {
                return NotFound();
            }

            ViewData["CheckId"] = checkId;
            return View(item);
        }

        // POST: Items/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id, int checkId)
        {
            var item = await _context.Items.FindAsync(id);
            if (item is not null)
            {
                var check = await _context.Checks
                .FirstOrDefaultAsync(c => c.Id == checkId);

                if (check is null)
                {
                    return NotFound();
                }

                check.Sum -= item.Price;
                _context.Items.Remove(item);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ItemExists(int id)
        {
            return _context.Items.Any(e => e.Id == id);
        }
    }
}
