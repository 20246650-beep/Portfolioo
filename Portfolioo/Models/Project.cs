using Portfolioo.Models;
using System.ComponentModel.DataAnnotations;

public class Project
{
    public int Id { get; set; }

    [Required]
    public int CourseId { get; set; }

    [Required, StringLength(100)]
    public string Title { get; set; } = "";

    public string? Description { get; set; }

    public int Year { get; set; }

    [StringLength(300)]
    public string? VideoUrl { get; set; }

    public List<ProjectImage> Images { get; set; } = new();

    // The textarea on the form: one image URL per line
    public string ImageUrls { get; set; } = "";
}