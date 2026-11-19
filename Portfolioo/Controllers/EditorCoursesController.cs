using Microsoft.AspNetCore.Mvc;
using Npgsql;
using Portfolioo.Models;

public class EditorCoursesController : Controller
{
    private readonly CourseRepository _courses;
    private readonly ProjectRepository _projects;
    public EditorCoursesController(CourseRepository c, ProjectRepository p) { _courses = c; _projects = p; }

    public async Task<IActionResult> Index() => View(await _courses.GetAllAsync());

    public async Task<IActionResult> Details(int id)
    {
        var c = await _courses.GetAsync(id);
        if (c == null) return NotFound();
        ViewBag.Projects = await _projects.GetByCourseAsync(id);
        return View(c);
    }

    public IActionResult Create() => View(new Course());

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Course c)
    {
        if (ModelState.IsValid)
        {
            try { await _courses.AddAsync(c); return RedirectToAction(nameof(Index)); }
            catch (PostgresException ex) { ModelState.AddModelError("", DbErrors.Message(ex)); }
        }
        return View(c);
    }

    public async Task<IActionResult> Edit(int id)
    {
        var c = await _courses.GetAsync(id);
        if (c == null) return NotFound();
        return View(c);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Course c)
    {
        c.Id = id;
        if (ModelState.IsValid)
        {
            try { await _courses.UpdateAsync(c); return RedirectToAction(nameof(Index)); }
            catch (PostgresException ex) { ModelState.AddModelError("", DbErrors.Message(ex)); }
        }
        return View(c);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        try { await _courses.DeleteAsync(id); }
        catch (PostgresException ex) { TempData["Error"] = DbErrors.Message(ex); }
        return RedirectToAction(nameof(Index));
    }
}