using Microsoft.AspNetCore.Mvc;
using Npgsql;

public class EditorProjectsController : Controller
{
    private readonly ProjectRepository _projects;
    private readonly CourseRepository _courses;
    public EditorProjectsController(ProjectRepository p, CourseRepository c) { _projects = p; _courses = c; }

    public async Task<IActionResult> Create(int? courseId)
    {
        ViewBag.Courses = await _courses.GetAllAsync();
        return View(new Project { Year = DateTime.Now.Year, CourseId = courseId ?? 0 });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Project p)
    {
        var urls = Rules.SplitUrls(p.ImageUrls);

        // server-side validation
        foreach (var u in urls)
            if (!Rules.ImageUrl.IsMatch(u))
                ModelState.AddModelError(nameof(p.ImageUrls), $"Not a valid image URL: {u}");
        if (!string.IsNullOrWhiteSpace(p.VideoUrl) && !Rules.Video.IsMatch(p.VideoUrl))
            ModelState.AddModelError(nameof(p.VideoUrl), "Paste a valid YouTube link.");

        if (ModelState.IsValid)
        {
            try
            {
                var id = await _projects.AddWithImagesAsync(p, urls);
                return RedirectToAction("Details", "Projects", new { id });
            }
            catch (PostgresException ex)
            {
                ModelState.AddModelError("", DbErrors.Message(ex) + " Nothing was saved.");
            }
        }
        ViewBag.Courses = await _courses.GetAllAsync();
        return View(p);
    }

    public async Task<IActionResult> Edit(int id)
    {
        var p = await _projects.GetAsync(id);
        if (p == null) return NotFound();
        ViewBag.Courses = await _courses.GetAllAsync();
        return View(p);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Project p)
    {
        p.Id = id;
        var urls = Rules.SplitUrls(p.ImageUrls);

        foreach (var u in urls)
            if (!Rules.ImageUrl.IsMatch(u))
                ModelState.AddModelError(nameof(p.ImageUrls), $"Not a valid image URL: {u}");
        if (!string.IsNullOrWhiteSpace(p.VideoUrl) && !Rules.Video.IsMatch(p.VideoUrl))
            ModelState.AddModelError(nameof(p.VideoUrl), "Paste a valid YouTube link.");

        if (ModelState.IsValid)
        {
            try
            {
                await _projects.UpdateWithImagesAsync(p, urls);
                return RedirectToAction("Details", "Projects", new { id });
            }
            catch (PostgresException ex)
            {
                ModelState.AddModelError("", DbErrors.Message(ex) + " Nothing was saved.");
            }
        }
        var current = await _projects.GetAsync(id);
        p.Images = current?.Images ?? new();
        ViewBag.Courses = await _courses.GetAllAsync();
        return View(p);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var p = await _projects.GetAsync(id);
        try { await _projects.DeleteAsync(id); }
        catch (PostgresException ex) { TempData["Error"] = DbErrors.Message(ex); }
        return RedirectToAction("Details", "EditorCourses", new { id = p?.CourseId ?? 0 });
    }
}