
using Microsoft.AspNetCore.Mvc;

public class CoursesController : Controller
{
    private readonly CourseRepository _courses;
    private readonly ProjectRepository _projects;
    public CoursesController(CourseRepository c, ProjectRepository p) { _courses = c; _projects = p; }

    public async Task<IActionResult> Index() => View(await _courses.GetAllAsync());

    public async Task<IActionResult> Details(int id)
    {
        var c = await _courses.GetAsync(id);
        if (c == null) return NotFound();
        ViewBag.Projects = await _projects.GetByCourseAsync(id);
        return View(c);
    }
}