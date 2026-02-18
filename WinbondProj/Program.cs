using Microsoft.EntityFrameworkCore;
using WinbondProj.Data;

namespace WinbondProj;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddControllers();

        // 配置 EF Core + SQLite
        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseSqlite("Data Source=filesystem.db"));

        // 註冊服務
        builder.Services.AddSingleton<Services.FileFactory>();
        builder.Services.AddSingleton<Services.ItemMapper>();
        builder.Services.AddScoped<Services.IQueryService, Services.QueryService>();
        builder.Services.AddScoped<Services.ICommandService, Services.CommandService>();
        builder.Services.AddScoped<Services.ITagService, Services.TagService>();

        // 配置 CORS (允許 Vue 前端存取)
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowVue", policy =>
            {
                policy.WithOrigins("http://localhost:5173", "http://localhost:5174")
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            });
        });

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

        // 初始化資料庫
        using (var scope = app.Services.CreateScope())
        {
            var services = scope.ServiceProvider;
            var context = services.GetRequiredService<AppDbContext>();
            DbInitializer.Initialize(context);
        }

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseCors("AllowVue");

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}