using Npgsql;
using Portfolioo.Models;

public class ProjectImageRepository
{
    private readonly NpgsqlDataSource _db;
    public ProjectImageRepository(NpgsqlDataSource db) => _db = db;

    public async Task<List<ProjectImage>> GetByProjectAsync(int projectId)
    {
        var list = new List<ProjectImage>();
        await using var cmd = _db.CreateCommand("select id, project_id, url from project_image where project_id=@p order by id");
        cmd.Parameters.AddWithValue("p", projectId);
        await using var r = await cmd.ExecuteReaderAsync();
        while (await r.ReadAsync())
            list.Add(new ProjectImage { Id = r.GetInt32(0), ProjectId = r.GetInt32(1), Url = r.GetString(2) });
        return list;
    }

    public async Task AddAsync(int projectId, string url)
    {
        await using var cmd = _db.CreateCommand("insert into project_image(project_id,url) values(@p,@u)");
        cmd.Parameters.AddWithValue("p", projectId);
        cmd.Parameters.AddWithValue("u", url);
        await cmd.ExecuteNonQueryAsync();
    }

    public async Task DeleteAsync(int id)
    {
        await using var cmd = _db.CreateCommand("delete from project_image where id=@id");
        cmd.Parameters.AddWithValue("id", id);
        await cmd.ExecuteNonQueryAsync();
    }
}