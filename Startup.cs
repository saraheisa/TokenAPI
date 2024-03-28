using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("AllowAllOrigins", builder =>
            {
                builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            });
        });

        ConfigureDatabase(services);
        ConfigureControllers(services);
        ConfigureSwagger(services);
        ConfigureJwtAuthentication(services);
        ConfigureTransientServices(services);

    }

    private void ConfigureDatabase(IServiceCollection services)
    {
        services.AddDbContext<TokenDbContext>(options =>
            options.UseMySQL(Configuration.GetConnectionString("DefaultConnection") ?? ""));
    }

    private void ConfigureControllers(IServiceCollection services)
    {
        services.AddControllersWithViews();
    }

    private void ConfigureSwagger(IServiceCollection services)
    {
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "TokenAPI", Version = "v1" });
        });
    }

    private void ConfigureJwtAuthentication(IServiceCollection services)
    {
        string jwtKey = Configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT secret key is missing or empty. Please provide a valid JWT secret key.");

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
                };
            });
    }

    private void ConfigureTransientServices(IServiceCollection services)
    {
        services.AddTransient<BNBChainService>();
        services.AddTransient<JWTTokenService>();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "TokenAPI v1");
            c.RoutePrefix = string.Empty;
        });

        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseCors("AllowAllOrigins");

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}
