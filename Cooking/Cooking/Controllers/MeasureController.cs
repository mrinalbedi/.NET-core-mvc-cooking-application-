using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Cooking.Models;

namespace Cooking.Controllers
{
    public class MeasureController : Controller
    {
        private readonly CookingContext _context;

        public MeasureController(CookingContext context)
        {
            _context = context;
        }

        // GET: Measure
        public async Task<IActionResult> Index(string CategoryCode)
        {
            if(!string.IsNullOrEmpty(CategoryCode))
            {
                Response.Cookies.Append("CategoryCode", CategoryCode);
            }
            else if(Request.Query["CategoryCode"].Any())
            {
                Response.Cookies.Append("CategoryCode", Request.Query["CategoryCode"].ToString());
            }
            else if(Request.Cookies["CategoryCode"]!=null)
            {
                CategoryCode= Request.Cookies["CategoryCode"].ToString();
            }
            else
            {
                TempData["message"] = "Please select a Category Code to display its measures";
                return RedirectToAction("Index", "Category");
            }

            var CategoryRecord = _context.Category.Where(b => b.CategoryCode == CategoryCode)
                                    .FirstOrDefault();
            var BaseMeasure = _context.Measure.Where(x => x.CategoryCode == CategoryRecord.CategoryCode && x.RatioToBase == 1)
                                .FirstOrDefault();

            //var CategoryRecord = from c in _context.Category
            //                     join cm in _context.Measure on
            //                    c.MeasureCode == cm.MeasureCode
            //                     select c;

            if (!string.IsNullOrEmpty(BaseMeasure.Name))
            {
                ViewData["CategoryName"] = CategoryRecord.Name;
                ViewData["MeasureName"] = BaseMeasure.Name;
            }
            else
            {
                ViewData["CategoryName"] = CategoryRecord.Name;
                ViewData["MeasureName"] = "N/A";
            }
            

            var cookingContext = _context.Measure.Include(m => m.CategoryCodeNavigation)
                .Where(m=>m.CategoryCode==CategoryCode)
                .OrderBy(x=>x.Name);
            return View(await cookingContext.ToListAsync());
        }

        // GET: Measure/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var measure = await _context.Measure
                .Include(m => m.CategoryCodeNavigation)
                .FirstOrDefaultAsync(m => m.MeasureCode == id);
            if (measure == null)
            {
                return NotFound();
            }

            return View(measure);
        }

        // GET: Measure/Create
        public IActionResult Create()
        {
            string Catcode = string.Empty;

            if(Request.Cookies["CategoryCode"]!=null)
            {
                Catcode = Request.Cookies["CategoryCode"].ToString();
            }

            var Category = _context.Category.Where(m => m.CategoryCode == Catcode).FirstOrDefault();
            ViewData["CategoryName"] = Category.Name;

           // ViewData["CategoryCode"] = new SelectList(_context.Category, "CategoryCode", "CategoryCode");
            return View();
        }

        // POST: Measure/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MeasureCode,Name,CategoryCode,RatioToBase")] Measure measure)
        {
            string Catcode = string.Empty;
            if (Request.Cookies["CategoryCode"] != null)
            {
                Catcode = Request.Cookies["CategoryCode"].ToString();
            }
            //var Category = _context.Category.Where(m => m.CategoryCode == Catcode).FirstOrDefault();
            //ViewData["CategoryName"] = Category.Name;

            measure.CategoryCode = Catcode;

            if (ModelState.IsValid)
            {
                _context.Add(measure);
                await _context.SaveChangesAsync();
                if(measure.RatioToBase==1)
                {
                    var name = _context.Category.Where(x => x.CategoryCode == measure.CategoryCode).FirstOrDefault();
                    name.MeasureCode = measure.MeasureCode;
                    _context.Update(measure);
                    _context.SaveChanges();
                    //ViewData["MeasureName"] = measure.Name;
                }
                TempData["message"] = "Your  measure code has been added in the database successfully";
                return RedirectToAction(nameof(Index));
            }
            // ViewData["CategoryCode"] = new SelectList(_context.Category, "CategoryCode", "CategoryCode", measure.CategoryCode);
            
            return View(measure);
        }

        // GET: Measure/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var measure = await _context.Measure.FindAsync(id);
            if (measure == null)
            {
                return NotFound();
            }
            ViewData["CategoryCode"] = new SelectList(_context.Category, "CategoryCode", "CategoryCode", measure.CategoryCode);
            return View(measure);
        }

        // POST: Measure/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("MeasureCode,Name,CategoryCode,RatioToBase")] Measure measure)
        {
            if (id != measure.MeasureCode)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(measure);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MeasureExists(measure.MeasureCode))
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
            ViewData["CategoryCode"] = new SelectList(_context.Category, "CategoryCode", "CategoryCode", measure.CategoryCode);
            return View(measure);
        }

        // GET: Measure/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var measure = await _context.Measure
                .Include(m => m.CategoryCodeNavigation)
                .FirstOrDefaultAsync(m => m.MeasureCode == id);
            if (measure == null)
            {
                return NotFound();
            }

            return View(measure);
        }

        // POST: Measure/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var measure = await _context.Measure.FindAsync(id);
            _context.Measure.Remove(measure);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MeasureExists(string id)
        {
            return _context.Measure.Any(e => e.MeasureCode == id);
        }
    }
}
