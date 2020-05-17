using cw3.Middleware;
using cw3.PartialModels;
using cw3.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Cw3
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        public void ConfigureServices(IServiceCollection services) {
            services.AddDbContext<s19048Context>(options =>
            {
                options.UseSqlServer("Data Source=db-mssql16.pjwstk.edu.pl; Initial Catalog=s19048;User ID=apbds19048;Password=admin");
            });
            services.AddScoped<ILoginService, LoginService>();
            services.AddScoped<IStudentsDbService, SqlServerDbService>();
            services.AddControllers();
            services.AddSwaggerGen(config =>
            {
                config.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Title =
                    "Students App API",
                    Version = "v1"
                });
            });
            services.AddControllers()
                    .AddXmlSerializerFormatters();
        }
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IStudentsDbService service)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            };
            app.UseSwagger();
            app.UseSwaggerUI(config =>
            {
                config.SwaggerEndpoint("/swagger/v1/swagger.json","Students App API");
            });

            //........... middleware uwierzytelnienie

            app.UseMiddleware<LoggingMiddleware>();
            app.Use(async (context, next) => {
                if (!context.Request.Headers.ContainsKey("Index"))
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsync("Nie podano indeksu w nagłówku");
                    return;
                }
                var index = context.Request.Headers["Index"].ToString();

                IStudentsDbService dbService = new SqlServerDbService();
                if (!dbService.CheckIndex(index))
                {
                    await context.Response.WriteAsync("Nie ma takiego studenta w bazie");
                    return;
                }
                await next();
            });
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthentication(); //--
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}