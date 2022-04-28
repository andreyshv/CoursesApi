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
using System.Diagnostics;

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

            await UpdateCourses(term);

            _context.Entry(term).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await TermExists(id))
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

            await UpdateCourses(term);

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

            await UpdateCourses(course, true);

            await _context.SaveChangesAsync();

            return NoContent();
        }

        private async Task<bool> TermExists(long id)
        {
            return await _context.Terms.AnyAsync(e => e.Id == id);
        }

        private async Task<List<Term>> GetAffectedCourses(Term holiday, bool deleted)
        {
            IQueryable<Term> affected = null;

            // get courses intersected with new holiday term
            if (!deleted)
            {
                affected = _context.Courses
                    .Where(Term.FilterByDate(holiday));
            }

            if (holiday.Id != 0)
            {
                // get saved holiday term by Id
                // and select courses intersected with the holiday
                var query =
                    from h in _context.Terms.Where(h => h.Id == holiday.Id)
                    from c in _context.Courses.Where(Term.FilterByDate(h))
                    select c;

                // get united courses query
                if (affected == null)
                {
                    affected = query;
                }
                else
                {
                    affected = affected.Union(query.AsEnumerable());
                }
            }

            return await affected.ToListAsync();
        }

        private async Task UpdateCourseHolidays(Term course, Term holiday = null)
        {
            Debug.Assert(course.Type == Term.TermType.Course);

            // find holidays that affects the course
            var ranges = await _context.Holidays
                .Where(Term.FilterByDate(course))
                .Select(h => new { h.StartDate, h.EndDate })
                .ToListAsync();

            if (holiday != null && holiday.Id == 0)
            {
                // new holiday term
                ranges.Add(new { holiday.StartDate, holiday.EndDate });
            }

            int holidays = ranges
                .Select(r => course.Intersect(r.StartDate, r.EndDate))
                .Sum();

            course.SetHolidays(holidays);
        }

        private async Task UpdateCourses(Term term, bool deleted = false)
        {
            //await _context.SaveChangesAsync();

            if (term.Type == Term.TermType.Holiday)
            {
                // holiday added, changed or deleted

                // get all courses affected by holiday
                foreach (var course in await GetAffectedCourses(term, deleted))
                {
                    await UpdateCourseHolidays(course, term);
                }
            } 
            else if (!deleted)
            {
                // course added or changed

                await UpdateCourseHolidays(term);
            }
        }

        private async Task<bool> IsTermValid(Term term)
        {
            bool isOverlapped = await _context.Terms
                .Where(c => c.Type == term.Type)
                .AnyAsync(Term.FilterByDate(term));

            if (isOverlapped)
            {
                ModelState.AddModelError(nameof(Term.StartDate), $"Overlapped {term.Type}s");
            }

            return !isOverlapped;
        }
    }
}
