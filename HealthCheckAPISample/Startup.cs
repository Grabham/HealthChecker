using HealthCheckAPISample.Extensions;
using HealthChecks.System;
using HealthChecks.UI.Client;
using HealthChecks.UI.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace HealthCheckAPISample
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
            var HC = services;
            HC.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            HC.AddHealthChecks()
                .AddTypeActivatedCheck<CustomHealthChecksWithArgs>("public/v2/users",
                    failureStatus: HealthStatus.Degraded,
                    tags: new[] { "e-layer" },
                    args: new object[] { Configuration["DEVConnectionStrings:URI"], "public/v2/users" })
                .AddTypeActivatedCheck<CustomHealthChecksWithArgs>("public/v2/posts", 
                    failureStatus: HealthStatus.Degraded,
                    tags: new[] { "p-layer" },
                    args: new object[] { Configuration["DEVConnectionStrings:URI"], "public/v2/posts" })
                .AddTypeActivatedCheck<CustomHealthChecksWithArgs>("public/v2/comments",
                    failureStatus: HealthStatus.Degraded,
                    tags: new[] { "s-layer" },
                    args: new object[] { Configuration["DEVConnectionStrings:URI"], "public/v2/comments" })
                .AddTypeActivatedCheck<CustomHealthChecksWithArgs>("public/v2/comments2",
                    failureStatus: HealthStatus.Degraded,
                    tags: new[] { "s-layer" },
                    args: new object[] { Configuration["DEVConnectionStrings:URI"], "public/v2/comments2" });
            //.AddCheck("My Database", new SqlConnectionHealthCheck(Configuration["ConnectionStrings:DefaultConnection"])); 
            HC.AddHealthChecksUI();
        }

        

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            // HealthCheck middleware
            app.UseHealthChecks("/e-layer", new HealthCheckOptions()
            {
                Predicate = (check) => check.Tags.Contains("e-layer"),
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });
            app.UseHealthChecks("/p-layer", new HealthCheckOptions()
            {
                Predicate = (check) => check.Tags.Contains("p-layer"),
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });
            app.UseHealthChecks("/s-layer", new HealthCheckOptions()
            {
                Predicate = (check) => check.Tags.Contains("s-layer"),
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });
            app.UseHealthChecksUI(delegate (Options options)
            {
                options.UIPath = "/hc-ui";
                options.AddCustomStylesheet("./Customization/custom.css");
            });
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
