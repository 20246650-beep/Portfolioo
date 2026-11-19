using Npgsql;

public class ProjectRepository
{
    private readonly NpgsqlDataSource _db;
    public ProjectRepository(NpgsqlDataSource db) => _db = db;

    public async Task<List<Project>> GetByCourseAsync(int courseId)
    {
        var list = new List<Project>();
        await using var cmd = _db.CreateCommand(
            "select id, course_id, title, description, year, video_url from project where course_id=@c order by year desc");
        cmd.Parameters.AddWithValue("c", courseId);
        await using var r = await cmd.ExecuteReaderAsync();
        while (await r.ReadAsync()) list.Add(Map(r));
        return list;
    }

    public async Task<Project?> GetAsync(int id)
    {
        Project? p;
        await using (var cmd = _db.CreateCommand(
            "select id, course_id, title, description, year, video_url from project where id=@id"))
        {
            cmd.Parameters.AddWithValue("id", id);
            await using var r = await cmd.ExecuteReaderAsync();
            if (!await r.ReadAsync()) return null;
            p = Map(r);
        }
        await using var ic = _db.CreateCommand("select id, project_id, url from project_image where project_id=@id order by id");
        ic.Parameters.AddWithValue("id", id);
        await using var ir = await ic.ExecuteReaderAsync();
        while (await ir.ReadAsync())
            p.Images.Add(new ProjectImage { Id = ir.GetInt32(0), ProjectId = ir.GetInt32(1), Url = ir.GetString(2) });
        p.ImageUrls = string.Join("\n", p.Images.Select(i => i.Url));
        return p;
    }

    // ONE TRANSACTION: project + all its images
    public async Task<int> AddWithImagesAsync(Project p, List<string> urls)
    {
        await using var conn = await _db.OpenConnectionAsync();
        await using var tx = await conn.BeginTransactionAsync();
        try
        {
            int id;
            await using (var cmd = new NpgsqlCommand(
                @"insert into project(course_id,title,description,year,video_url)
                  values(@c,@t,@d,@y,@v) returning id", conn, tx))
            {
                cmd.Parameters.AddWithValue("c", p.CourseId);
                cmd.Parameters.AddWithValue("t", p.Title);
                cmd.Parameters.AddWithValue("d", (object?)p.Description ?? DBNull.Value);
                cmd.Parameters.AddWithValue("y", p.Year);
                cmd.Parameters.AddWithValue("v", (object?)p.VideoUrl ?? DBNull.Value);
                id = (int)(await cmd.ExecuteScalarAsync())!;
            }
            foreach (var url in urls)
            {
                await using var ic = new NpgsqlCommand("insert into project_image(project_id,url) values(@p,@u)", conn, tx);
                ic.Parameters.AddWithValue("p", id);
                ic.Parameters.AddWithValue("u", url);
                await ic.ExecuteNonQueryAsync();   // duplicate URL -> 23505 -> rollback
            }
            await tx.CommitAsync();
            return id;
        }
        catch { await tx.RollbackAsync(); throw; }
    }

    // ONE TRANSACTION: update project, replace its images
    public async Task UpdateWithImagesAsync(Project p, List<string> urls)
    {
        await using var conn = await _db.OpenConnectionAsync();
        await using var tx = await conn.BeginTransactionAsync();
        try
        {
            await using (var cmd = new NpgsqlCommand(
                @"update project set course_id=@c, title=@t, description=@d, year=@y, video_url=@v where id=@id", conn, tx))
            {
                cmd.Parameters.AddWithValue("id", p.Id);
                cmd.Parameters.AddWithValue("c", p.CourseId);
                cmd.Parameters.AddWithValue("t", p.Title);
                cmd.Parameters.AddWithValue("d", (object?)p.Description ?? DBNull.Value);
                cmd.Parameters.AddWithValue("y", p.Year);
                cmd.Parameters.AddWithValue("v", (object?)p.VideoUrl ?? DBNull.Value);
                await cmd.ExecuteNonQueryAsync();
            }
            await using (var del = new NpgsqlCommand("delete from project_image where project_id=@id", conn, tx))
            {
                del.Parameters.AddWithValue("id", p.Id);
                await del.ExecuteNonQueryAsync();
            }
            foreach (var url in urls)
            {
                await using var ic = new NpgsqlCommand("insert into project_image(project_id,url) values(@p,@u)", conn, tx);
                ic.Parameters.AddWithValue("p", p.Id);
                ic.Parameters.AddWithValue("u", url);
                await ic.ExecuteNonQueryAsync();
            }
            await tx.CommitAsync();
        }
        catch { await tx.RollbackAsync(); throw; }
    }

    public async Task DeleteAsync(int id)
    {
        await using var cmd = _db.CreateCommand("delete from project where id=@id");
        cmd.Parameters.AddWithValue("id", id);
        await cmd.ExecuteNonQueryAsync();
    }

    private static Project Map(NpgsqlDataReader r) => new()
    {
        Id = r.GetInt32(0),
        CourseId = r.GetInt32(1),
        Title = r.GetString(2),
        Description = r.IsDBNull(3) ? null : r.GetString(3),
        Year = r.GetInt32(4),
        VideoUrl = r.IsDBNull(5) ? null : r.GetString(5)
    };
    public async Task<List<Project>> SearchAsync(string term)
    {
        var list = new List<Project>();
        await using var cmd = _db.CreateCommand(
            @"select id, course_id, title, description, year, video_url from project
          where title ilike @t or description ilike @t order by year desc");
        cmd.Parameters.AddWithValue("t", "%" + term + "%");
        await using var r = await cmd.ExecuteReaderAsync();
        while (await r.ReadAsync()) list.Add(Map(r));
        return list;
    }
}