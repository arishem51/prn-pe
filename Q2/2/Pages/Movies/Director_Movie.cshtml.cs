using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using _2.Models;

namespace _2.Pages.Movies;

public class Director_MovieModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public Director_MovieModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public List<Director> Directors { get; set; } = new();
    public List<Movie> Movies { get; set; } = new();

    public async Task OnGetAsync(int? directorId = null)
    {
        Directors = await _context.Directors
            .OrderBy(d => d.FullName)
            .ToListAsync();

        var query = _context.Movies
            .Include(m => m.Director)
            .Include(m => m.Stars)
            .AsQueryable();

        if (directorId.HasValue)
        {
            query = query.Where(m => m.DirectorId == directorId);
        }

        Movies = await query.OrderBy(m => m.Title).ToListAsync();
    }

    public async Task<IActionResult> OnPostDeleteAsync(int? id, int? directorId = null)
    {
        if (id == null)
        {
            return NotFound();
        }

        var movie = await _context.Movies.FindAsync(id);
        if (movie != null)
        {
            _context.Movies.Remove(movie);
            await _context.SaveChangesAsync();
        }

        // Preserve director filter if it was active
        if (directorId.HasValue)
        {
            return RedirectToPage("/Movies/Director_Movie", new { directorId = directorId.Value });
        }

        return RedirectToPage("/Movies/Director_Movie");
    }
}

