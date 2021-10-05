using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Reacto.Web.Hubs;
using Serilog;

namespace Reacto.Web
{
    public class Startup
    {
        private readonly IWebHostEnvironment env;

        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            this.env = env;
        }

        /// <summary>
        /// Contains the configuration of the application.
        /// </summary>
        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors();

            if (env.IsDevelopment())
            {
                services.AddSignalR();
            }
            else
            {
                services.AddSignalR().AddAzureSignalR();
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSerilogRequestLogging();

            app.UseCors(builder => builder.WithOrigins(Configuration.GetValue<string>("AllowedOrigin"))
                .WithMethods("GET", "POST")
                .AllowAnyHeader()
                .AllowCredentials());

            app.UseRouting();

            app.UseCors();

            app.UseEndpoints(endpoints => { endpoints.MapHub<StageHub>("/stage"); });
        }
    }
}
