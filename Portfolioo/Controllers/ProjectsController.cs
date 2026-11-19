using Microsoft.AspNetCore.Mvc;

public class ProjectsController : Controller
{
    private readonly ProjectRepository _projects;
    public ProjectsController(ProjectRepository p) => _projects = p;

    public async Task<IActionResult> Details(int id)
    {
        var p = await _projects.GetAsync(id);
        if (p == null) return NotFound();
        return View(p);
    }
    public async Task<IActionResult> Search(string q)
    {
        ViewBag.Query = q;
        return View(string.IsNullOrWhiteSpace(q) ? new List<Project>() : await _projects.SearchAsync(q));
    }
}