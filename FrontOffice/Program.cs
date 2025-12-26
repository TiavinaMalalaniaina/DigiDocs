using FrontOffice.Data;
using FrontOffice.Data.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllersWithViews();

// Injection des dépendances
builder.Services.AddSingleton<DbConnectionFactory>(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    return new DbConnectionFactory(config);
});
builder.Services.AddScoped<UserRepository>();
builder.Services.AddSession();
builder.Services.AddScoped<DocumentRepository>();


var app = builder.Build();

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseSession();
app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}")
    .WithStaticAssets();

app.Run();
