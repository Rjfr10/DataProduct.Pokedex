using DataProduct.Pokedex.Api.Helpers;
using DataProduct.Pokedex.Api.Models.Configuration;
using DataProduct.Pokedex.Api.Services;
using DataProduct.Pokedex.Api.Services.External;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace DataProduct.Pokedex.Api
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

            services.AddSwaggerGen();   //Register the Swagger Generator for creation of swagger documentation.

            services.AddLogging();

            services.Configure<VariableOptions>(Configuration.GetSection(VariableOptions.SectionName)); //Setup configuration for variables using IOptionsMonitor.

            services.AddSingleton<IHttpClientHelper, HttpClientHelper>();   //Add Helper for sending Http requests.
            services.AddSingleton<IPokeApiClient, PokeApiClientAdapter>();  //Add Wrapper "Decorator" for external library missing an implementation. 
            services.AddSingleton<IPokemonService, PokemonService>();   //Register Pokemon Services for business logic.
            services.AddSingleton<ITranslationService, TranslationService>();   //Add Service for handling translation of messages.

            services.AddHttpClient("funtranslations", instance =>   //Register instance of the http client factory.
            {
                instance.BaseAddress = new Uri(Configuration["ExternalSource:FunTranslations"]);
                instance.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, 
                              IWebHostEnvironment env,
                              IConfiguration configuration)
        {
            if (env.IsDevelopment()) 
            {
                app.UseDeveloperExceptionPage();
            }

            // Quick basic implementation to redact some of the information
            // This marked as Tech Debt to be replaced with a Global Exception handler with code classifications and predefined message formats: details, meta data etc.
            app.UseExceptionHandler(exceptionApp =>
            {
                exceptionApp.Run(async context =>
                {
                    context.Response.ContentType = "application/json";
                    var exceptionHandler = context.Features.Get<IExceptionHandlerPathFeature>();
                    await context.Response.WriteAsJsonAsync(new { error = exceptionHandler.Error.Message });
                });
            });

            // Enable Middleware for Swagger generation of document and UI.
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint(configuration["SwaggerUI:Endpoint"], configuration["SwaggerUI:Name"]);
            });

            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

        }
    }
}
