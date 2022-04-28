#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CoursesApi.Models;
using System.ComponentModel.DataAnnotations;

namespace CoursesApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TermsController : ControllerBase
    {
        private readonly CoursesContext _context;

        public TermsController(CoursesContext context)
        {
            _context = context;
        }

        // GET: api/Terms
        [HttpGet("{StudentId}")]
        public async Task<ActionResult<IEnumerable<Term>>> GetTerms(long StudentId)
        {
            return await _context.Terms
                .Where(c => c.StudentId == StudentId)
                .OrderBy(t => t.StartDate)
                .ToListAsync();
        }

        // GET: api/Terms/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Term>> GetTerm(long id)
        {
            var course = await _context.Terms.FindAsync(id);

            if (course == null)
            {
                return NotFound();
            }

            return course;
        }

        // PUT: api/Terms/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTerm(long id, Term term)
        {
            if (id != term.Id)
            {
                return BadRequest();
            }

            _context.Entry(term).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CourseTerm(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Term
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Term>> PostTerm(Term term)
        {
            if (!await IsTermValid(term))
            {
                return BadRequest(ModelState);
            }

            _context.Terms.Add(term);

            UpdateActualEndDate(term);

            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTerm), new { id = term.Id }, term);
        }

        // DELETE: api/Terms/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTerm(long id)
        {
            var course = await _context.Terms.FindAsync(id);
            if (course == null)
            {
                return NotFound();
            }

            _context.Terms.Remove(course);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CourseTerm(long id)
        {
            return _context.Terms.Any(e => e.Id == id);
        }

        private async void UpdateActualEndDate(Term term)
        {
            if (term.Type == Term.TermType.Holiday)
            {

                var overlapped = await _context.Terms.Where(c =>
                    c.Type == Term.TermType.Course
                    && ((c.StartDate >= term.StartDate && c.StartDate <= term.EndDate)
                        || (c.EndDate >= term.StartDate && c.EndDate <= term.EndDate)))
                    .ToArrayAsync();

                foreach (var course in overlapped)
                {
                    course.Holidays = 
                }
            }
        }

        private async Task<bool> IsTermValid(Term term)
        {
            bool isOverlapped = await _context.Terms.AnyAsync(c =>
                c.Type == term.Type
                && ((c.StartDate >= term.StartDate && c.StartDate <= term.EndDate)
                    || (c.EndDate >= term.StartDate && c.EndDate <= term.EndDate)));

            if (isOverlapped)
            {
                ModelState.AddModelError(nameof(Term.StartDate), $"Overlapped {term.Type}s");
            }

            return !isOverlapped;
        }
    }
}
