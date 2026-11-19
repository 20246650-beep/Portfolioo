using Npgsql;

public static class DbErrors
{
    public static string Message(PostgresException e) => e.SqlState switch
    {
        "23503" => "That record is still used by another record (or the referenced record does not exist).",
        "23505" => "That value already exists. Codes and image URLs must be unique.",
        "23514" => "A value is outside the allowed range.",
        _ => "The database refused the change."
    };
}