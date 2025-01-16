using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StudentApplication.Context;
using StudentApplication.Models;

namespace StudentApplication.Controllers
{

    [Authorize(Roles = "Admin,Manager")]
    public class StudentsController : Controller
    {
        private readonly AppDbContext _context;

        public StudentsController(AppDbContext context)
        {
            _context = context;
        }

        [AllowAnonymous]
        // GET: Students
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.Students.Include(s => s.School);
            return View(await appDbContext.ToListAsync());
        }

        // GET: Students/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Students
                .Include(s => s.School)
                .FirstOrDefaultAsync(m => m.StudentId == id);
            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        // GET: Students/Create
        public IActionResult Create()
        {
          
            ViewBag.SchoolID = new SelectList(_context.Schools, "SchoolID", "SchoolName");
            return View();
        }

        // POST: Students/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("StudentId,StudentName,Age,BirthDate,SchoolID")] Student student)
        {
            try
            {
                _context.Add(student);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch {
                ViewBag.SchoolID = new SelectList(_context.Schools, "SchoolID", "SchoolName");
                return View(student);
            }
            
        }

        // GET: Students/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Students.FindAsync(id);
            if (student == null)
            {
                return NotFound();
            }
            ViewBag.SchoolID = new SelectList(_context.Schools, "SchoolID", "SchoolName");
            return View(student);
        }

        // POST: Students/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("StudentId,StudentName,Age,BirthDate,SchoolID")] Student student)
        {
            if (id != student.StudentId)
            {
                return NotFound();
            }

            try
            {
                _context.Update(student);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StudentExists(student.StudentId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            catch (Exception)
            {
                return View(student);
            }

            return RedirectToAction(nameof(Index));
        }




        // GET: Students/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Students
                .Include(s => s.School)
                .FirstOrDefaultAsync(m => m.StudentId == id);
            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        // POST: Students/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var student = await _context.Students.FindAsync(id);
            if (student != null)
            {
                _context.Students.Remove(student);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool StudentExists(int id)
        {
            return _context.Students.Any(e => e.StudentId == id);
        }
        public ActionResult Search(string name, int? schoolid)
        {
            // Start with all students
            var result = _context.Students.AsQueryable();

            // Filter by name if provided
            if (!string.IsNullOrEmpty(name))
            {
                result = result.Where(s => s.StudentName.Contains(name)); // Adjust for case sensitivity if needed
            }

            // Filter by school ID if provided
            if (schoolid.HasValue)
            {
                result = result.Where(s => s.SchoolID == schoolid.Value);
            }

            // Repopulate the ViewBag for the dropdown in the view
            ViewBag.SchoolID = new SelectList(_context.Schools, "SchoolID", "SchoolName");

            // Return the filtered result to the Index view
            return View("Index", result.ToList()); // Convert to List for the view
        }

    }
}
