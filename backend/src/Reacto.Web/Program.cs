using System.Net.Sockets;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;
using Reacto.Grains;
using Serilog;

namespace Reacto.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog((context, configuration) => configuration.ReadFrom.Configuration(context.Configuration))
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); })
                .UseOrleans((context, builder) =>
                {
                    builder.Configure<ClusterOptions>(options =>
                    {
                        options.ClusterId = "reacto-cluster";
                        options.ServiceId = "reacto";
                    });

                    if (context.HostingEnvironment.IsDevelopment())
                    {
                        builder.UseLocalhostClustering();
                        builder.AddMemoryGrainStorageAsDefault();
                    }
                    else
                    {
                        builder.ConfigureEndpoints(1111, 30000);
                        builder.UseAzureStorageClustering(options =>
                        {
                            options.ConnectionString = context.Configuration.GetConnectionString("AzureStorage");
                        });
                        builder.AddAzureBlobGrainStorageAsDefault(options =>
                        {
                            options.UseJson = true;
                            options.ConnectionString = context.Configuration.GetConnectionString("AzureStorage");
                        });
                    }

                    builder.ConfigureApplicationParts(parts =>
                        parts.AddApplicationPart(typeof(Stage).Assembly).WithReferences());
                });
    }
}
