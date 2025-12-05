using Microsoft.EntityFrameworkCore;
using TieChef.Data;
using TieChef.Repositories;
using FluentValidation;
using FluentValidation.AspNetCore;
using TieChef.Validators;

var builder = WebApplication.CreateBuilder(args);

// Додати сервіси до контейнера.

builder.Services.AddControllersWithViews();
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<StaffDTOValidator>();

// Реєстрація репозиторіїв
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IStaffRepository, StaffRepository>();
builder.Services.AddScoped<IReceiptRepository, ReceiptRepository>();
builder.Services.AddScoped<IDiningTableRepository, DiningTableRepository>();
builder.Services.AddScoped<IDishRepository, DishRepository>();

// Дізнайтеся більше про налаштування Swagger/OpenAPI за посиланням https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Підключення до PostgreSQL
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Redis підключення - закоментовано для тестування CRUD без кешу
// builder.Services.AddStackExchangeRedisCache(options =>
// {
//     options.Configuration = builder.Configuration.GetConnectionString("Redis");
// });

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    // Ініціалізація БД
    using (var scope = app.Services.CreateScope())
    {
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        // context.Database.EnsureCreated(); // Ми будемо використовувати міграції замість цього
    }
    
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "TieChef API v1");
        c.RoutePrefix = "swagger";
    });
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapControllers();

app.Run();

