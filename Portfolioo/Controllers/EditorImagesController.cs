using Microsoft.AspNetCore.Mvc;
using Npgsql;
using System.Data;

public class EditorImagesController : Controller
{
    private readonly ProjectImageRepository _images;
    public EditorImagesController(ProjectImageRepository i) => _images = i;

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Add(int projectId, string url)
    {
        if (!Rules.ImageUrl.IsMatch(url ?? ""))
            TempData["Error"] = "Not a valid image URL.";
        else
        {
            try { await _images.AddAsync(projectId, url!); }
            catch (PostgresException ex) { TempData["Error"] = DbErrors.Message(ex); }
        }
        return RedirectToAction("Edit", "EditorProjects", new { id = projectId });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id, int projectId)
    {
        try { await _images.DeleteAsync(id); }
        catch (PostgresException ex) { TempData["Error"] = DbErrors.Message(ex); }
        return RedirectToAction("Edit", "EditorProjects", new { id = projectId });
    }
}