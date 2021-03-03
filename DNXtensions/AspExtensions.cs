#nullable enable
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using static DNXtensions.InterfaceExtensions;

namespace DnxSampleApi
{
    public static class AspExtensions
    {
        static public void ConfigureApi(this IServiceCollection services, OpenApiInfo? apiInfo = null)
        {
            services.AddControllers();
            services.AddMvc().AddJsonOptions(opt =>
            {
                opt.JsonSerializerOptions.Converters.AddRange(DNXtensions.Json.DefaultOptions.Converters);
                opt.JsonSerializerOptions.DefaultIgnoreCondition = DNXtensions.Json.DefaultOptions.DefaultIgnoreCondition;
                opt.JsonSerializerOptions.PropertyNameCaseInsensitive = DNXtensions.Json.DefaultOptions.PropertyNameCaseInsensitive;
            });

            var callerName = Assembly.GetCallingAssembly().GetName().Name;
            services.AddSwaggerGen(c =>
                {
                    c.SwaggerDoc("v1", apiInfo ?? new OpenApiInfo { Title = callerName + " API", Version = "v1" });
                    var xmlFile = $"{callerName}.xml";
                    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                    c.IncludeXmlComments(xmlPath);
                });
        }

        static public void ConfigureApi(this IApplicationBuilder app, IWebHostEnvironment? env)
        {
            if (env?.IsDevelopment() == true)
                app.UseDeveloperExceptionPage();
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
                {
                    endpoints.MapControllers();
                    endpoints.MapSwagger("swagger/{documentName}/swagger.json");
                });
            app.UseSwaggerUI(c => c.RoutePrefix = "");
        }
    }
}
