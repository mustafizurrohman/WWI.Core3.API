﻿using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using System;
using System.IO;

namespace WWI.Core3.Core.ExtensionMethods
{
    /// <summary>
    /// Extensions for Swagger
    /// </summary>
    public static class SwaggerExtensions
    {

        /// <summary>
        /// Extension method to configure swagger and add documentation
        /// </summary>
        /// <param name="services"></param>
        /// <param name="info"></param>
        /// <param name="apiKeyScheme"></param>
        /// <returns></returns>
        public static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services, OpenApiInfo info, OpenApiSecurityScheme apiKeyScheme)
        {
            info ??= default;
            apiKeyScheme ??= default;

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc(info?.Version, new OpenApiInfo
                {
                    Version = info?.Version,
                    Title = info?.Title,
                    Description = info?.Description,
                    TermsOfService = info?.TermsOfService,
                    Contact = new OpenApiContact()
                    {
                        Name = info?.Contact.Name,
                        Email = info?.Contact.Email,
                        Url = info?.Contact.Url
                    }
                });

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = apiKeyScheme?.Description,
                    Name = apiKeyScheme?.Name,
                    Type = SecuritySchemeType.Http
                });

                var xmlFile = $"WWI.Core3.API.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);


            });

            return services;
        }

        /// <summary>
        /// Extension method to use swagger documentation
        /// </summary>
        /// <param name="app"></param>
        /// <param name="info"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseSwaggerDocumentation(this IApplicationBuilder app, OpenApiInfo info)
        {
            info ??= default;

            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/" + info?.Version + "/swagger.json", info?.Title + " v" + info?.Version);
            });

            return app;
        }
    }
}