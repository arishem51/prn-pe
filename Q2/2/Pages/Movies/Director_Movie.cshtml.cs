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

    public async Task OnGetAsync()
    {
        Directors = await _context.Directors
            .OrderBy(d => d.FullName)
            .ToListAsync();

        Movies = await _context.Movies
            .Include(m => m.Director)
            .Include(m => m.Stars)
            .OrderBy(m => m.Title)
            .ToListAsync();
    }
}

