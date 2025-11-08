using System;
using System.Collections.Generic;

namespace _2.Models;

public partial class Movie
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public DateOnly? ReleaseDate { get; set; }

    public string? Description { get; set; }

    public string Language { get; set; } = null!;

    public int? ProducerId { get; set; }

    public int? DirectorId { get; set; }

    public virtual Director? Director { get; set; }

    public virtual Producer? Producer { get; set; }

    public virtual ICollection<Genre> Genres { get; set; } = new List<Genre>();

    public virtual ICollection<Star> Stars { get; set; } = new List<Star>();
}
