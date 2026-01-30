using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using LMS.Data;
using LMS.Models;
using LMS.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.DataProtection;
using System.IO;

namespace LMS
{
  public class Startup
  {
    public Startup(IConfiguration configuration)
    {
      Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
      // Prefer Railway MYSQL_URL env var if present; otherwise fall back to configured connection string
      string identityConn;
      var mysqlUrlEnv = Environment.GetEnvironmentVariable("MYSQL_URL");
      if (!string.IsNullOrEmpty(mysqlUrlEnv))
      {
        identityConn = ParseMysqlUrl(mysqlUrlEnv);
      }
      else
      {
        identityConn = Configuration.GetConnectionString("IdentityConnection");
      }

      services.AddDbContext<ApplicationDbContext>(options =>
                options.UseMySql(identityConn));

      // Configure Data Protection key persistence. If DATA_PROTECTION_PATH is set to a directory
      // (e.g., "/var/keys"), keys will be persisted there so tokens/cookies remain valid across restarts.
      var dpPath = Environment.GetEnvironmentVariable("DATA_PROTECTION_PATH");
      if (!string.IsNullOrEmpty(dpPath))
      {
        Directory.CreateDirectory(dpPath);
        services.AddDataProtection()
                .PersistKeysToFileSystem(new DirectoryInfo(dpPath))
                .SetApplicationName("LMS");
      }
      else
      {
        services.AddDataProtection().SetApplicationName("LMS");
      }

      services.AddIdentity<ApplicationUser, IdentityRole>(options =>
      {
        // Password settings
        options.Password.RequireDigit = false;
        options.Password.RequiredLength = 1;
        options.Password.RequiredUniqueChars = 1;
        options.Password.RequireLowercase = false;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireUppercase = false;

      })
      .AddEntityFrameworkStores<ApplicationDbContext>()
      .AddDefaultTokenProviders();

      // Add application services.
      services.AddTransient<IEmailSender, EmailSender>();

      services.AddMvc();
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IHostingEnvironment env, IServiceProvider services)
    {
      
      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
        app.UseDatabaseErrorPage();
      }
      else
      {
        app.UseExceptionHandler("/Home/Error");
      }

      // Only enable HTTPS redirection if explicitly enforced. Railway performs TLS termination for the container
      // so redirect middleware may not be applicable in Production unless you set ENFORCE_HTTPS=true.
      var enforceHttps = Environment.GetEnvironmentVariable("ENFORCE_HTTPS");
      if (!string.IsNullOrEmpty(enforceHttps) && enforceHttps.ToLower() == "true")
      {
        app.UseHttpsRedirection();
      }

      app.UseStaticFiles();

      app.UseAuthentication();

      app.UseMvc(routes =>
      {
        routes.MapRoute(
                  name: "default",
                  template: "{controller=Home}/{action=Index}/{id?}");
      });

      CreateUserRoles(services).Wait();
    }

    private async Task CreateUserRoles(IServiceProvider serviceProvider)
    {
      var RoleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
      var UserManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();


      IdentityResult roleResult;
      // Make sure admin, prof, and student roles exist
      // This should only happen the first time the server starts up
      var hasAdmin = await RoleManager.RoleExistsAsync("Administrator");
      if (!hasAdmin)
      {
        roleResult = await RoleManager.CreateAsync(new IdentityRole("Administrator"));
      }

      var hasProfessor = await RoleManager.RoleExistsAsync("Professor");
      if (!hasProfessor)
      {
        roleResult = await RoleManager.CreateAsync(new IdentityRole("Professor"));
      }

      var hasStudent = await RoleManager.RoleExistsAsync("Student");
      if (!hasStudent)
      {
        roleResult = await RoleManager.CreateAsync(new IdentityRole("Student"));
      }
    }

    // Helper to convert MYSQL_URL (mysql://user:pass@host:port/db) into a MySQL connection string
    private string ParseMysqlUrl(string url)
    {
      var uri = new Uri(url);
      var userInfo = uri.UserInfo.Split(':');
      var user = userInfo[0];
      var pass = userInfo.Length > 1 ? userInfo[1] : string.Empty;
      var host = uri.Host;
      var port = uri.Port;
      var db = uri.AbsolutePath.TrimStart('/');
      return $"Server={host};Port={port};User Id={user};Password={pass};Database={db};";
    }

  }
}

