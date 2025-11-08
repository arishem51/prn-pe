# CRUD Operations Guide: Blazor and Razor Pages

This guide provides comprehensive examples and best practices for implementing Create, Read, Update, and Delete (CRUD) operations in both **Razor Pages** and **Blazor** applications.

---

## Table of Contents

1. [Razor Pages CRUD](#razor-pages-crud)

   - [Setup](#razor-pages-setup)
   - [Read (List)](#read-list)
   - [Create](#create)
   - [Update (Edit)](#update-edit)
   - [Delete](#delete)
   - [Details (View)](#details-view)

2. [Blazor CRUD](#blazor-crud)

   - [Setup](#blazor-setup)
   - [Read (List)](#blazor-read-list)
   - [Create](#blazor-create)
   - [Update (Edit)](#blazor-update-edit)
   - [Delete](#blazor-delete)
   - [Details (View)](#blazor-details-view)

3. [Best Practices](#best-practices)
4. [Common Patterns](#common-patterns)

---

## Razor Pages CRUD

Razor Pages is a page-based programming model that makes building web UI easier and more productive. Each page consists of a `.cshtml` file (the view) and a `.cshtml.cs` file (the page model).

### Razor Pages Setup

**1. Configure DbContext in `Program.cs`:**

```csharp
using Microsoft.EntityFrameworkCore;
using YourProject.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddRazorPages();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Configure pipeline
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.MapRazorPages();

app.Run();
```

**2. Create a Model (Example: Movie):**

```csharp
public class Movie
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public DateTime? ReleaseDate { get; set; }
    public string? Description { get; set; }
    public string Language { get; set; } = null!;
    public int? DirectorId { get; set; }
    public virtual Director? Director { get; set; }
}
```

---

### Read (List)

**Page Model (`Movies/Index.cshtml.cs`):**

```csharp
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using YourProject.Models;

namespace YourProject.Pages.Movies;

public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public IndexModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public IList<Movie> Movies { get; set; } = new List<Movie>();

    public async Task OnGetAsync()
    {
        Movies = await _context.Movies
            .Include(m => m.Director)
            .OrderBy(m => m.Title)
            .ToListAsync();
    }
}
```

**Razor View (`Movies/Index.cshtml`):**

```html
@page @model YourProject.Pages.Movies.IndexModel @{ ViewData["Title"] =
"Movies"; }

<h2>Movies</h2>

<p>
  <a asp-page="Create">Create New</a>
</p>

<table class="table">
  <thead>
    <tr>
      <th>Title</th>
      <th>Release Date</th>
      <th>Language</th>
      <th>Director</th>
      <th>Actions</th>
    </tr>
  </thead>
  <tbody>
    @foreach (var movie in Model.Movies) {
    <tr>
      <td>@movie.Title</td>
      <td>@(movie.ReleaseDate?.ToString("MM/dd/yyyy") ?? "N/A")</td>
      <td>@movie.Language</td>
      <td>@(movie.Director?.FullName ?? "N/A")</td>
      <td>
        <a asp-page="./Edit" asp-route-id="@movie.Id">Edit</a> |
        <a asp-page="./Details" asp-route-id="@movie.Id">Details</a> |
        <a asp-page="./Delete" asp-route-id="@movie.Id">Delete</a>
      </td>
    </tr>
    }
  </tbody>
</table>
```

---

### Create

**Page Model (`Movies/Create.cshtml.cs`):**

```csharp
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using YourProject.Models;

namespace YourProject.Pages.Movies;

public class CreateModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public CreateModel(ApplicationDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public Movie Movie { get; set; } = new();

    public SelectList Directors { get; set; } = null!;

    public async Task<IActionResult> OnGetAsync()
    {
        // Populate dropdown for Directors
        Directors = new SelectList(
            await _context.Directors.OrderBy(d => d.FullName).ToListAsync(),
            nameof(Director.Id),
            nameof(Director.FullName)
        );
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            // Re-populate dropdown on validation error
            Directors = new SelectList(
                await _context.Directors.OrderBy(d => d.FullName).ToListAsync(),
                nameof(Director.Id),
                nameof(Director.FullName)
            );
            return Page();
        }

        _context.Movies.Add(Movie);
        await _context.SaveChangesAsync();

        return RedirectToPage("./Index");
    }
}
```

**Razor View (`Movies/Create.cshtml`):**

```html
@page @model YourProject.Pages.Movies.CreateModel @{ ViewData["Title"] = "Create
Movie"; }

<h2>Create Movie</h2>

<hr />
<div class="row">
  <div class="col-md-4">
    <form method="post">
      <div asp-validation-summary="ModelOnly" class="text-danger"></div>

      <div class="form-group">
        <label asp-for="Movie.Title" class="control-label"></label>
        <input asp-for="Movie.Title" class="form-control" />
        <span asp-validation-for="Movie.Title" class="text-danger"></span>
      </div>

      <div class="form-group">
        <label asp-for="Movie.ReleaseDate" class="control-label"></label>
        <input asp-for="Movie.ReleaseDate" type="date" class="form-control" />
        <span asp-validation-for="Movie.ReleaseDate" class="text-danger"></span>
      </div>

      <div class="form-group">
        <label asp-for="Movie.Description" class="control-label"></label>
        <textarea
          asp-for="Movie.Description"
          class="form-control"
          rows="3"
        ></textarea>
        <span asp-validation-for="Movie.Description" class="text-danger"></span>
      </div>

      <div class="form-group">
        <label asp-for="Movie.Language" class="control-label"></label>
        <input asp-for="Movie.Language" class="form-control" />
        <span asp-validation-for="Movie.Language" class="text-danger"></span>
      </div>

      <div class="form-group">
        <label asp-for="Movie.DirectorId" class="control-label"></label>
        <select
          asp-for="Movie.DirectorId"
          class="form-control"
          asp-items="Model.Directors"
        >
          <option value="">-- Select Director --</option>
        </select>
        <span asp-validation-for="Movie.DirectorId" class="text-danger"></span>
      </div>

      <div class="form-group mt-3">
        <input type="submit" value="Create" class="btn btn-primary" />
        <a asp-page="Index" class="btn btn-secondary">Cancel</a>
      </div>
    </form>
  </div>
</div>

@section Scripts { @{await
Html.RenderPartialAsync("_ValidationScriptsPartial");} }
```

---

### Update (Edit)

**Page Model (`Movies/Edit.cshtml.cs`):**

```csharp
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using YourProject.Models;

namespace YourProject.Pages.Movies;

public class EditModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public EditModel(ApplicationDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public Movie Movie { get; set; } = new();

    public SelectList Directors { get; set; } = null!;

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        Movie = await _context.Movies.FindAsync(id);

        if (Movie == null)
        {
            return NotFound();
        }

        // Populate dropdown
        Directors = new SelectList(
            await _context.Directors.OrderBy(d => d.FullName).ToListAsync(),
            nameof(Director.Id),
            nameof(Director.FullName)
        );

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            Directors = new SelectList(
                await _context.Directors.OrderBy(d => d.FullName).ToListAsync(),
                nameof(Director.Id),
                nameof(Director.FullName)
            );
            return Page();
        }

        _context.Attach(Movie).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!MovieExists(Movie.Id))
            {
                return NotFound();
            }
            throw;
        }

        return RedirectToPage("./Index");
    }

    private bool MovieExists(int id)
    {
        return _context.Movies.Any(e => e.Id == id);
    }
}
```

**Razor View (`Movies/Edit.cshtml`):**

```html
@page "{id:int}" @model YourProject.Pages.Movies.EditModel @{ ViewData["Title"]
= "Edit Movie"; }

<h2>Edit Movie</h2>

<hr />
<div class="row">
  <div class="col-md-4">
    <form method="post">
      <div asp-validation-summary="ModelOnly" class="text-danger"></div>

      <input type="hidden" asp-for="Movie.Id" />

      <div class="form-group">
        <label asp-for="Movie.Title" class="control-label"></label>
        <input asp-for="Movie.Title" class="form-control" />
        <span asp-validation-for="Movie.Title" class="text-danger"></span>
      </div>

      <div class="form-group">
        <label asp-for="Movie.ReleaseDate" class="control-label"></label>
        <input asp-for="Movie.ReleaseDate" type="date" class="form-control" />
        <span asp-validation-for="Movie.ReleaseDate" class="text-danger"></span>
      </div>

      <div class="form-group">
        <label asp-for="Movie.Description" class="control-label"></label>
        <textarea
          asp-for="Movie.Description"
          class="form-control"
          rows="3"
        ></textarea>
        <span asp-validation-for="Movie.Description" class="text-danger"></span>
      </div>

      <div class="form-group">
        <label asp-for="Movie.Language" class="control-label"></label>
        <input asp-for="Movie.Language" class="form-control" />
        <span asp-validation-for="Movie.Language" class="text-danger"></span>
      </div>

      <div class="form-group">
        <label asp-for="Movie.DirectorId" class="control-label"></label>
        <select
          asp-for="Movie.DirectorId"
          class="form-control"
          asp-items="Model.Directors"
        >
          <option value="">-- Select Director --</option>
        </select>
        <span asp-validation-for="Movie.DirectorId" class="text-danger"></span>
      </div>

      <div class="form-group mt-3">
        <input type="submit" value="Save" class="btn btn-primary" />
        <a asp-page="./Index" class="btn btn-secondary">Cancel</a>
      </div>
    </form>
  </div>
</div>

@section Scripts { @{await
Html.RenderPartialAsync("_ValidationScriptsPartial");} }
```

---

### Delete

**Page Model (`Movies/Delete.cshtml.cs`):**

```csharp
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using YourProject.Models;

namespace YourProject.Pages.Movies;

public class DeleteModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public DeleteModel(ApplicationDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public Movie Movie { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        Movie = await _context.Movies
            .Include(m => m.Director)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (Movie == null)
        {
            return NotFound();
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        Movie = await _context.Movies.FindAsync(id);

        if (Movie != null)
        {
            _context.Movies.Remove(Movie);
            await _context.SaveChangesAsync();
        }

        return RedirectToPage("./Index");
    }
}
```

**Razor View (`Movies/Delete.cshtml`):**

```html
@page "{id:int}" @model YourProject.Pages.Movies.DeleteModel @{
ViewData["Title"] = "Delete Movie"; }

<h2>Delete Movie</h2>

<h3>Are you sure you want to delete this?</h3>
<div>
  <h4>Movie</h4>
  <hr />
  <dl class="row">
    <dt class="col-sm-2">@Html.DisplayNameFor(model => model.Movie.Title)</dt>
    <dd class="col-sm-10">@Html.DisplayFor(model => model.Movie.Title)</dd>
    <dt class="col-sm-2">
      @Html.DisplayNameFor(model => model.Movie.ReleaseDate)
    </dt>
    <dd class="col-sm-10">
      @(Model.Movie.ReleaseDate?.ToString("MM/dd/yyyy") ?? "N/A")
    </dd>
    <dt class="col-sm-2">
      @Html.DisplayNameFor(model => model.Movie.Language)
    </dt>
    <dd class="col-sm-10">@Html.DisplayFor(model => model.Movie.Language)</dd>
    <dt class="col-sm-2">
      @Html.DisplayNameFor(model => model.Movie.Director)
    </dt>
    <dd class="col-sm-10">@(Model.Movie.Director?.FullName ?? "N/A")</dd>
  </dl>

  <form method="post">
    <input type="hidden" asp-for="Movie.Id" />
    <input type="submit" value="Delete" class="btn btn-danger" />
    <a asp-page="./Index" class="btn btn-secondary">Cancel</a>
  </form>
</div>
```

---

### Details (View)

**Page Model (`Movies/Details.cshtml.cs`):**

```csharp
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using YourProject.Models;

namespace YourProject.Pages.Movies;

public class DetailsModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public DetailsModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public Movie Movie { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        Movie = await _context.Movies
            .Include(m => m.Director)
            .Include(m => m.Stars)
            .Include(m => m.Genres)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (Movie == null)
        {
            return NotFound();
        }

        return Page();
    }
}
```

**Razor View (`Movies/Details.cshtml`):**

```html
@page "{id:int}" @model YourProject.Pages.Movies.DetailsModel @{
ViewData["Title"] = "Movie Details"; }

<h2>Movie Details</h2>

<div>
  <h4>Movie</h4>
  <hr />
  <dl class="row">
    <dt class="col-sm-2">@Html.DisplayNameFor(model => model.Movie.Title)</dt>
    <dd class="col-sm-10">@Html.DisplayFor(model => model.Movie.Title)</dd>
    <dt class="col-sm-2">
      @Html.DisplayNameFor(model => model.Movie.ReleaseDate)
    </dt>
    <dd class="col-sm-10">
      @(Model.Movie.ReleaseDate?.ToString("MM/dd/yyyy") ?? "N/A")
    </dd>
    <dt class="col-sm-2">
      @Html.DisplayNameFor(model => model.Movie.Description)
    </dt>
    <dd class="col-sm-10">
      @Html.DisplayFor(model => model.Movie.Description)
    </dd>
    <dt class="col-sm-2">
      @Html.DisplayNameFor(model => model.Movie.Language)
    </dt>
    <dd class="col-sm-10">@Html.DisplayFor(model => model.Movie.Language)</dd>
    <dt class="col-sm-2">
      @Html.DisplayNameFor(model => model.Movie.Director)
    </dt>
    <dd class="col-sm-10">@(Model.Movie.Director?.FullName ?? "N/A")</dd>
    <dt class="col-sm-2">Stars</dt>
    <dd class="col-sm-10">
      @string.Join(", ", Model.Movie.Stars.Select(s => s.FullName))
    </dd>
  </dl>
</div>
<div>
  <a asp-page="./Edit" asp-route-id="@Model.Movie.Id">Edit</a> |
  <a asp-page="./Index">Back to List</a>
</div>
```

---

## Blazor CRUD

Blazor is a framework for building interactive web UIs using C# instead of JavaScript. It supports both **Blazor Server** (server-side) and **Blazor WebAssembly** (client-side).

### Blazor Setup

**1. Configure Services in `Program.cs` (Blazor Server):**

```csharp
using Microsoft.EntityFrameworkCore;
using YourProject.Data;
using YourProject.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register services
builder.Services.AddScoped<MovieService>();

var app = builder.Build();

// Configure pipeline
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.MapRazorPages();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
```

**2. Create a Service (`Services/MovieService.cs`):**

```csharp
using Microsoft.EntityFrameworkCore;
using YourProject.Data;
using YourProject.Models;

namespace YourProject.Services;

public class MovieService
{
    private readonly ApplicationDbContext _context;

    public MovieService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Movie>> GetMoviesAsync()
    {
        return await _context.Movies
            .Include(m => m.Director)
            .Include(m => m.Stars)
            .OrderBy(m => m.Title)
            .ToListAsync();
    }

    public async Task<Movie?> GetMovieByIdAsync(int id)
    {
        return await _context.Movies
            .Include(m => m.Director)
            .Include(m => m.Stars)
            .Include(m => m.Genres)
            .FirstOrDefaultAsync(m => m.Id == id);
    }

    public async Task<List<Director>> GetDirectorsAsync()
    {
        return await _context.Directors
            .OrderBy(d => d.FullName)
            .ToListAsync();
    }

    public async Task<bool> CreateMovieAsync(Movie movie)
    {
        try
        {
            _context.Movies.Add(movie);
            await _context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> UpdateMovieAsync(Movie movie)
    {
        try
        {
            _context.Attach(movie).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> DeleteMovieAsync(int id)
    {
        try
        {
            var movie = await _context.Movies.FindAsync(id);
            if (movie != null)
            {
                _context.Movies.Remove(movie);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }
        catch
        {
            return false;
        }
    }
}
```

---

### Blazor Read (List)

**Component (`Pages/Movies/MovieList.razor`):**

```razor
@page "/movies"
@using YourProject.Models
@using YourProject.Services
@inject MovieService MovieService
@inject NavigationManager Navigation

<h3>Movies</h3>

<p>
    <button class="btn btn-primary" @onclick="NavigateToCreate">Create New</button>
</p>

@if (movies == null)
{
    <p><em>Loading...</em></p>
}
else
{
    <table class="table">
        <thead>
            <tr>
                <th>Title</th>
                <th>Release Date</th>
                <th>Language</th>
                <th>Director</th>
                <th>Actions</th>
            </tr>
        </thead>
        <tbody>
            @foreach (var movie in movies)
            {
                <tr>
                    <td>@movie.Title</td>
                    <td>@(movie.ReleaseDate?.ToString("MM/dd/yyyy") ?? "N/A")</td>
                    <td>@movie.Language</td>
                    <td>@(movie.Director?.FullName ?? "N/A")</td>
                    <td>
                        <button class="btn btn-sm btn-info" @onclick="() => NavigateToDetails(movie.Id)">Details</button>
                        <button class="btn btn-sm btn-warning" @onclick="() => NavigateToEdit(movie.Id)">Edit</button>
                        <button class="btn btn-sm btn-danger" @onclick="() => DeleteMovie(movie.Id)">Delete</button>
                    </td>
                </tr>
            }
        </tbody>
    </table>
}

@code {
    private List<Movie>? movies;

    protected override async Task OnInitializedAsync()
    {
        movies = await MovieService.GetMoviesAsync();
    }

    private void NavigateToCreate()
    {
        Navigation.NavigateTo("/movies/create");
    }

    private void NavigateToDetails(int id)
    {
        Navigation.NavigateTo($"/movies/details/{id}");
    }

    private void NavigateToEdit(int id)
    {
        Navigation.NavigateTo($"/movies/edit/{id}");
    }

    private async Task DeleteMovie(int id)
    {
        if (await MovieService.DeleteMovieAsync(id))
        {
            movies = await MovieService.GetMoviesAsync();
            StateHasChanged();
        }
    }
}
```

---

### Blazor Create

**Component (`Pages/Movies/MovieCreate.razor`):**

```razor
@page "/movies/create"
@using YourProject.Models
@using YourProject.Services
@inject MovieService MovieService
@inject NavigationManager Navigation

<h3>Create Movie</h3>

<EditForm Model="@movie" OnValidSubmit="@HandleValidSubmit">
    <DataAnnotationsValidator />
    <ValidationSummary />

    <div class="form-group">
        <label>Title</label>
        <InputText @bind-Value="movie.Title" class="form-control" />
        <ValidationMessage For="@(() => movie.Title)" />
    </div>

    <div class="form-group">
        <label>Release Date</label>
        <InputDate @bind-Value="releaseDate" class="form-control" />
    </div>

    <div class="form-group">
        <label>Description</label>
        <InputTextArea @bind-Value="movie.Description" class="form-control" rows="3" />
    </div>

    <div class="form-group">
        <label>Language</label>
        <InputText @bind-Value="movie.Language" class="form-control" />
        <ValidationMessage For="@(() => movie.Language)" />
    </div>

    <div class="form-group">
        <label>Director</label>
        <InputSelect @bind-Value="movie.DirectorId" class="form-control">
            <option value="">-- Select Director --</option>
            @foreach (var director in directors)
            {
                <option value="@director.Id">@director.FullName</option>
            }
        </InputSelect>
    </div>

    <div class="form-group mt-3">
        <button type="submit" class="btn btn-primary">Create</button>
        <button type="button" class="btn btn-secondary" @onclick="Cancel">Cancel</button>
    </div>
</EditForm>

@code {
    private Movie movie = new();
    private DateOnly? releaseDate;
    private List<Director> directors = new();

    protected override async Task OnInitializedAsync()
    {
        directors = await MovieService.GetDirectorsAsync();
    }

    private async Task HandleValidSubmit()
    {
        if (releaseDate.HasValue)
        {
            movie.ReleaseDate = releaseDate.Value;
        }

        if (await MovieService.CreateMovieAsync(movie))
        {
            Navigation.NavigateTo("/movies");
        }
    }

    private void Cancel()
    {
        Navigation.NavigateTo("/movies");
    }
}
```

---

### Blazor Update (Edit)

**Component (`Pages/Movies/MovieEdit.razor`):**

```razor
@page "/movies/edit/{Id:int}"
@using YourProject.Models
@using YourProject.Services
@inject MovieService MovieService
@inject NavigationManager Navigation

<h3>Edit Movie</h3>

@if (movie == null)
{
    <p><em>Loading...</em></p>
}
else
{
    <EditForm Model="@movie" OnValidSubmit="@HandleValidSubmit">
        <DataAnnotationsValidator />
        <ValidationSummary />

        <div class="form-group">
            <label>Title</label>
            <InputText @bind-Value="movie.Title" class="form-control" />
            <ValidationMessage For="@(() => movie.Title)" />
        </div>

        <div class="form-group">
            <label>Release Date</label>
            <InputDate @bind-Value="releaseDate" class="form-control" />
        </div>

        <div class="form-group">
            <label>Description</label>
            <InputTextArea @bind-Value="movie.Description" class="form-control" rows="3" />
        </div>

        <div class="form-group">
            <label>Language</label>
            <InputText @bind-Value="movie.Language" class="form-control" />
            <ValidationMessage For="@(() => movie.Language)" />
        </div>

        <div class="form-group">
            <label>Director</label>
            <InputSelect @bind-Value="movie.DirectorId" class="form-control">
                <option value="">-- Select Director --</option>
                @foreach (var director in directors)
                {
                    <option value="@director.Id">@director.FullName</option>
                }
            </InputSelect>
        </div>

        <div class="form-group mt-3">
            <button type="submit" class="btn btn-primary">Save</button>
            <button type="button" class="btn btn-secondary" @onclick="Cancel">Cancel</button>
        </div>
    </EditForm>
}

@code {
    [Parameter]
    public int Id { get; set; }

    private Movie? movie;
    private DateOnly? releaseDate;
    private List<Director> directors = new();

    protected override async Task OnInitializedAsync()
    {
        directors = await MovieService.GetDirectorsAsync();
        movie = await MovieService.GetMovieByIdAsync(Id);

        if (movie?.ReleaseDate != null)
        {
            releaseDate = movie.ReleaseDate.Value;
        }
    }

    private async Task HandleValidSubmit()
    {
        if (movie != null)
        {
            if (releaseDate.HasValue)
            {
                movie.ReleaseDate = releaseDate.Value;
            }

            if (await MovieService.UpdateMovieAsync(movie))
            {
                Navigation.NavigateTo("/movies");
            }
        }
    }

    private void Cancel()
    {
        Navigation.NavigateTo("/movies");
    }
}
```

---

### Blazor Delete

**Component (`Pages/Movies/MovieDelete.razor`):**

```razor
@page "/movies/delete/{Id:int}"
@using YourProject.Models
@using YourProject.Services
@inject MovieService MovieService
@inject NavigationManager Navigation

<h3>Delete Movie</h3>

<h4>Are you sure you want to delete this?</h4>

@if (movie == null)
{
    <p><em>Loading...</em></p>
}
else
{
    <div>
        <h4>Movie</h4>
        <hr />
        <dl class="row">
            <dt class="col-sm-2">Title</dt>
            <dd class="col-sm-10">@movie.Title</dd>

            <dt class="col-sm-2">Release Date</dt>
            <dd class="col-sm-10">@(movie.ReleaseDate?.ToString("MM/dd/yyyy") ?? "N/A")</dd>

            <dt class="col-sm-2">Language</dt>
            <dd class="col-sm-10">@movie.Language</dd>

            <dt class="col-sm-2">Director</dt>
            <dd class="col-sm-10">@(movie.Director?.FullName ?? "N/A")</dd>
        </dl>

        <div class="mt-3">
            <button class="btn btn-danger" @onclick="HandleDelete">Delete</button>
            <button class="btn btn-secondary" @onclick="Cancel">Cancel</button>
        </div>
    </div>
}

@code {
    [Parameter]
    public int Id { get; set; }

    private Movie? movie;

    protected override async Task OnInitializedAsync()
    {
        movie = await MovieService.GetMovieByIdAsync(Id);
    }

    private async Task HandleDelete()
    {
        if (await MovieService.DeleteMovieAsync(Id))
        {
            Navigation.NavigateTo("/movies");
        }
    }

    private void Cancel()
    {
        Navigation.NavigateTo("/movies");
    }
}
```

---

### Blazor Details (View)

**Component (`Pages/Movies/MovieDetails.razor`):**

```razor
@page "/movies/details/{Id:int}"
@using YourProject.Models
@using YourProject.Services
@inject MovieService MovieService
@inject NavigationManager Navigation

<h3>Movie Details</h3>

@if (movie == null)
{
    <p><em>Loading...</em></p>
}
else
{
    <div>
        <h4>Movie</h4>
        <hr />
        <dl class="row">
            <dt class="col-sm-2">Title</dt>
            <dd class="col-sm-10">@movie.Title</dd>

            <dt class="col-sm-2">Release Date</dt>
            <dd class="col-sm-10">@(movie.ReleaseDate?.ToString("MM/dd/yyyy") ?? "N/A")</dd>

            <dt class="col-sm-2">Description</dt>
            <dd class="col-sm-10">@movie.Description</dd>

            <dt class="col-sm-2">Language</dt>
            <dd class="col-sm-10">@movie.Language</dd>

            <dt class="col-sm-2">Director</dt>
            <dd class="col-sm-10">@(movie.Director?.FullName ?? "N/A")</dd>

            <dt class="col-sm-2">Stars</dt>
            <dd class="col-sm-10">@string.Join(", ", movie.Stars.Select(s => s.FullName))</dd>
        </dl>
    </div>
    <div>
        <button class="btn btn-warning" @onclick="NavigateToEdit">Edit</button>
        <button class="btn btn-secondary" @onclick="NavigateToIndex">Back to List</button>
    </div>
}

@code {
    [Parameter]
    public int Id { get; set; }

    private Movie? movie;

    protected override async Task OnInitializedAsync()
    {
        movie = await MovieService.GetMovieByIdAsync(Id);
    }

    private void NavigateToEdit()
    {
        Navigation.NavigateTo($"/movies/edit/{Id}");
    }

    private void NavigateToIndex()
    {
        Navigation.NavigateTo("/movies");
    }
}
```

---

## Best Practices

### 1. **Use Services/Repository Pattern**

- Separate data access logic from UI components
- Makes code more testable and maintainable
- Easier to swap data sources

### 2. **Error Handling**

```csharp
// Razor Pages
try
{
    await _context.SaveChangesAsync();
}
catch (DbUpdateException ex)
{
    ModelState.AddModelError("", "Unable to save changes. " + ex.Message);
    return Page();
}

// Blazor
try
{
    await MovieService.CreateMovieAsync(movie);
}
catch (Exception ex)
{
    // Log error and show user-friendly message
}
```

### 3. **Validation**

- Use Data Annotations on models
- Client-side validation with `DataAnnotationsValidator` in Blazor
- Server-side validation in Razor Pages with `ModelState`

### 4. **Async Operations**

- Always use `async/await` for database operations
- Use `ToListAsync()`, `FirstOrDefaultAsync()`, etc.

### 5. **Include Related Data**

```csharp
// Use Include() to load related entities
Movies = await _context.Movies
    .Include(m => m.Director)
    .Include(m => m.Stars)
    .ToListAsync();
```

### 6. **Pagination**

```csharp
// For large datasets
public async Task OnGetAsync(int pageIndex = 0, int pageSize = 10)
{
    Movies = await _context.Movies
        .Skip(pageIndex * pageSize)
        .Take(pageSize)
        .ToListAsync();

    TotalCount = await _context.Movies.CountAsync();
}
```

### 7. **Security**

- Always validate user input
- Use parameterized queries (EF Core handles this)
- Implement authorization checks
- Use HTTPS in production

---

## Common Patterns

### 1. **Search/Filter Pattern**

**Razor Pages:**

```csharp
public string? SearchString { get; set; }

public async Task OnGetAsync(string? searchString)
{
    SearchString = searchString;

    var movies = from m in _context.Movies select m;

    if (!string.IsNullOrEmpty(SearchString))
    {
        movies = movies.Where(m => m.Title.Contains(SearchString));
    }

    Movies = await movies.ToListAsync();
}
```

**Blazor:**

```razor
<input @bind="searchString" @bind:event="oninput" placeholder="Search..." />

@code {
    private string searchString = "";
    private List<Movie>? filteredMovies;

    private void FilterMovies()
    {
        if (string.IsNullOrEmpty(searchString))
        {
            filteredMovies = movies;
        }
        else
        {
            filteredMovies = movies?.Where(m =>
                m.Title.Contains(searchString, StringComparison.OrdinalIgnoreCase)
            ).ToList();
        }
    }
}
```

### 2. **Confirmation Dialog (Blazor)**

```razor
@if (showDeleteConfirm)
{
    <div class="modal">
        <div class="modal-content">
            <p>Are you sure you want to delete this item?</p>
            <button @onclick="ConfirmDelete">Yes</button>
            <button @onclick="CancelDelete">No</button>
        </div>
    </div>
}
```

### 3. **Loading States**

```razor
@if (isLoading)
{
    <p>Loading...</p>
}
else
{
    <!-- Content -->
}
```

---

## Summary

- **Razor Pages**: Page-based model, good for traditional web apps, uses `PageModel` and `.cshtml` files
- **Blazor**: Component-based model, good for interactive UIs, uses `.razor` files and services
- Both support full CRUD operations with Entity Framework Core
- Always use async/await for database operations
- Implement proper error handling and validation
- Use services to separate concerns

For more information, refer to:

- [Razor Pages Documentation](https://learn.microsoft.com/en-us/aspnet/core/razor-pages/)
- [Blazor Documentation](https://learn.microsoft.com/en-us/aspnet/core/blazor/)
