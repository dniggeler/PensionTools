using System;
using System.IO;
using System.Reflection;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using Tax.Data;
using Tax.Tools.Comparison;


namespace TaxCalculator.WebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            this.Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHealthChecks()
                .AddDbContextCheck<FederalTaxTariffDbContext>()
                .AddDbContextCheck<TaxTariffDbContext>()
                .AddDbContextCheck<TaxRateDbContext>()
                .AddDbContextCheck<MunicipalityDbContext>();

            services.AddControllersWithViews()
                .AddJsonOptions(options =>
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
            services.AddTaxData(this.Configuration);
            services.AddTaxCalculators();
            services.AddTaxComparers();
            services.AddSwaggerExamplesFromAssemblyOf<Examples.CapitalBenefitTaxRequestExample>();
            services.AddSwaggerGen(opt =>
            {
                opt.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Tax Calculators",
                    Contact = new OpenApiContact
                    {
                        Name = "Dieter Niggeler",
                        Email = "dnig69+2@gmail.com",
                    },
                    Version = "v1",
                });
                opt.ExampleFilters();

                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                opt.IncludeXmlComments(xmlPath);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
            app.UseAuthorization();
            app.UseHealthChecks("/");
            app.UseSwagger();
            app.UseSwaggerUI(opt =>
            {
                opt.SwaggerEndpoint("/swagger/v1/swagger.json", "Tax Calculators Api V1");
                opt.RoutePrefix = "swagger";
            });
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
