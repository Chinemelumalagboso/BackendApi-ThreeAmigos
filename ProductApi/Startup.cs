using Microsoft.EntityFrameworkCore;
using ProductApi.Data;

namespace ProductApi
{
    public class Startup
    {
        public IConfiguration Configuration { get; } // Add this property to store the configuration

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration; // Assign the injected configuration
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            // Use the Configuration property to get the connection string
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
