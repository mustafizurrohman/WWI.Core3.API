﻿using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using WWI.Core3.Middleware.ExceptionHandler;
using WWI.Core3.Models.DatabaseContext;

namespace WWI.Core3.Core.ExtensionMethods
{
    /// <summary>
    /// Extension methods for Application Builder
    /// </summary>
    public static class ApplicationBuilderExtensions
    {
        /// <summary>
        /// Extension Method to use custom exception handler
        /// </summary>
        /// <param name="applicationBuilder">Application Builder</param>
        /// <returns></returns>
        public static IApplicationBuilder UseCustomExceptionHandler(this IApplicationBuilder applicationBuilder)
        {
            return applicationBuilder.UseMiddleware<ExceptionHandler>();
        }

        /// <summary>
        /// Migrates the database.
        /// </summary>
        /// <param name="applicationBuilder">The application builder.</param>
        /// <returns></returns>
        public static void MigrateDatabase(this IApplicationBuilder applicationBuilder)
        {
            using IServiceScope serviceScope = applicationBuilder.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope();
            {
                using var context = serviceScope.ServiceProvider.GetService<DocAppointmentContext>();
                context.Database.Migrate();
            }
        }

    }

}
