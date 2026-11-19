using Npgsql;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();

builder.Services.AddSingleton(
    NpgsqlDataSource.Create(builder.Configuration.GetConnectionString("DefaultConnection")!));

builder.Services.AddScoped<CourseRepository>();
builder.Services.AddScoped<ProjectRepository>();
builder.Services.AddScoped<ProjectImageRepository>();

var app = builder.Build();

app.UseExceptionHandler("/Home/Error");                       // any unexpected crash
app.UseStatusCodePagesWithReExecute("/Home/NotFoundPage");    // 404 and other empty error codes

app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.MapControllerRoute("default", "{controller=Courses}/{action=Index}/{id?}");
app.Run();