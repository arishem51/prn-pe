namespace Client;

public class Start
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool? Male { get; set; }
    public string? Nationality { get; set; }
    public DateTime? Dob { get; set; }

    public override string ToString()
    {
        var parts = new List<string> { Name };
        if (Dob.HasValue)
            parts.Add($"DOB: {Dob.Value:yyyy-MM-dd}");
        if (!string.IsNullOrEmpty(Description))
            parts.Add($"Desc: {Description}");
        if (Male.HasValue)
            parts.Add($"Male: {Male.Value}");
        if (!string.IsNullOrEmpty(Nationality))
            parts.Add($"Nationality: {Nationality}");
        return string.Join(" | ", parts);
    }
}

