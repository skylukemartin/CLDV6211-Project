namespace cldv6211proj
{
    using Microsoft.EntityFrameworkCore;
    using Shared.Data;
    using Shared.Services;

    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add HttpContext
            builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession(Options =>
            {
                Options.IdleTimeout = TimeSpan.FromMinutes(120);
            });

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            // ref: https://learn.microsoft.com/en-us/ef/core/miscellaneous/connection-strings
            // TODO: Use a more secure way of storing connection string (before someone finds this repo and nukes the db)
            builder.Services.AddDbContext<SharedDbContext>(options =>
                options.UseSqlServer(
                    builder.Configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly("Shared")
                )
            );
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IProductService, ProductService>();
            builder.Services.AddScoped<IOrderService, OrderService>();

            var app = builder.Build();

            DbInitializer.Initialize( // Make sure db has been seeded
                app.Services.CreateScope().ServiceProvider.GetRequiredService<SharedDbContext>()
            );

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseSession();
            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}"
            );

            app.Run();
        }
    }
}
