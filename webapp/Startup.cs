using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using cosmoschat.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace cosmoschat
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
            services.AddMvc();
            CosmosDBOptions options = new CosmosDBOptions(Configuration.GetSection("CosmosDB"));
            DocumentClient documentClient = new DocumentClient(options.Endpoint, options.Key);
            InitializeDocumentCollections(documentClient).Wait();
            services.AddSingleton<IDocumentClient>(documentClient);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }

        private static async Task InitializeDocumentCollections(DocumentClient documentClient)
        {
            Database database = new Database();
            database.Id = "social";
            Uri databaseUri = UriFactory.CreateDatabaseUri(database.Id);
            await documentClient.CreateDatabaseIfNotExistsAsync(database);
            DocumentCollection postsCollection = new DocumentCollection();
            postsCollection.Id = "posts";
            await documentClient.CreateDocumentCollectionIfNotExistsAsync(databaseUri, postsCollection, new RequestOptions() { OfferThroughput = 400 });
            DocumentCollection resultsCollection = new DocumentCollection();
            resultsCollection.Id = "results";
            await documentClient.CreateDocumentCollectionIfNotExistsAsync(databaseUri, resultsCollection, new RequestOptions() { OfferThroughput = 400 });
        }
    }
}
