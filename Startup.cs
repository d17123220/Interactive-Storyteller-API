using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;

using Interactive_Storyteller_API.Services;

namespace Interactive_Storyteller_API
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

            services.AddControllers();

            services.AddSingleton<ICosmosDBService>(InitializeCosmosClientInstanceAsync(Configuration).GetAwaiter().GetResult());

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Interactive_Storyteller_API", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Interactive_Storyteller_API v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        private static async Task<CosmosDBService> InitializeCosmosClientInstanceAsync(IConfiguration Configuration)
        {
            // CosmosDB connection information
            string account = Configuration["CosmosDB:Account"];
            string key = Configuration["CosmosDB:Key"];
            
            // CosmosDB database
            string databaseName = Configuration["CosmosDB:DatabaseName"];

            // CosmosDB containers
            var containerNames = Configuration
                                    .AsEnumerable()
                                    .Where(o => o.Key.Contains("CosmosDB:ContainerName"))
                                    .Select(o => o.Value)
                                    .ToList();   

            // initialize CosmosDB client
            Microsoft.Azure.Cosmos.CosmosClient client = new Microsoft.Azure.Cosmos.CosmosClient(account, key);
            
            // initialize service
            CosmosDBService cosmosDbService = new CosmosDBService(client);

            // for each container in list:   
            containerNames.ForEach(async containerName => 
            {
                // create database if missing
                Microsoft.Azure.Cosmos.DatabaseResponse database = await client.CreateDatabaseIfNotExistsAsync(databaseName);
                // create container inside database, if missing
                await database.Database.CreateContainerIfNotExistsAsync(containerName, "/id");
                // add container to the list inside service
                await cosmosDbService.AddContainerDefinition(databaseName, containerName);
            });

            // Do nothing. Satisfy debugger's requirement to have "await" in it
            await Task.Run(() => {});

            return cosmosDbService;
        }
    }
}
