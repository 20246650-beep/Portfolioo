using System.ComponentModel.DataAnnotations;

public class Course
{
    public int Id { get; set; }

    [Required, RegularExpression(@"^[A-Z]{2,8}\d{1,4}$", ErrorMessage = "Use a code like DBMWEB1 or IT101.")]
    public string Code { get; set; } = "";

    [Required, StringLength(100)]
    public string Title { get; set; } = "";

    public string? Description { get; set; }
}
