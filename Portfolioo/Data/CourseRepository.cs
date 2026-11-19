using Npgsql;

public class CourseRepository
{
    private readonly NpgsqlDataSource _db;
    public CourseRepository(NpgsqlDataSource db) => _db = db;

    public async Task<List<Course>> GetAllAsync()
    {
        var list = new List<Course>();
        await using var cmd = _db.CreateCommand("select id, code, title, description from course order by code");
        await using var r = await cmd.ExecuteReaderAsync();
        while (await r.ReadAsync())
            list.Add(new Course
            {
                Id = r.GetInt32(0),
                Code = r.GetString(1),
                Title = r.GetString(2),
                Description = r.IsDBNull(3) ? null : r.GetString(3)
            });
        return list;
    }

    public async Task<Course?> GetAsync(int id)
    {
        await using var cmd = _db.CreateCommand("select id, code, title, description from course where id=@id");
        cmd.Parameters.AddWithValue("id", id);
        await using var r = await cmd.ExecuteReaderAsync();
        if (!await r.ReadAsync()) return null;
        return new Course
        {
            Id = r.GetInt32(0),
            Code = r.GetString(1),
            Title = r.GetString(2),
            Description = r.IsDBNull(3) ? null : r.GetString(3)
        };
    }

    public async Task AddAsync(Course c)
    {
        await using var cmd = _db.CreateCommand("insert into course(code,title,description) values(@c,@t,@d)");
        cmd.Parameters.AddWithValue("c", c.Code);
        cmd.Parameters.AddWithValue("t", c.Title);
        cmd.Parameters.AddWithValue("d", (object?)c.Description ?? DBNull.Value);
        await cmd.ExecuteNonQueryAsync();
    }

    public async Task UpdateAsync(Course c)
    {
        await using var cmd = _db.CreateCommand("update course set code=@c, title=@t, description=@d where id=@id");
        cmd.Parameters.AddWithValue("id", c.Id);
        cmd.Parameters.AddWithValue("c", c.Code);
        cmd.Parameters.AddWithValue("t", c.Title);
        cmd.Parameters.AddWithValue("d", (object?)c.Description ?? DBNull.Value);
        await cmd.ExecuteNonQueryAsync();
    }

    public async Task DeleteAsync(int id)
    {
        await using var cmd = _db.CreateCommand("delete from course where id=@id");
        cmd.Parameters.AddWithValue("id", id);
        await cmd.ExecuteNonQueryAsync();   // 23503 if projects still exist
    }
}