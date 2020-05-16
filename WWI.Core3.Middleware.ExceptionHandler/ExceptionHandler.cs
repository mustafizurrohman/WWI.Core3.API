﻿// ***********************************************************************
// Assembly         : WWI.Core3.Middleware.ExceptionHandler
// Author           : Mustafizur Rohman
// Created          : 05-02-2020
//
// Last Modified By : Mustafizur Rohman
// Last Modified On : 05-02-2020
// ***********************************************************************
// <copyright file="ExceptionHandler.cs" company="WWI.Core3.Middleware.ExceptionHandler">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Serilog;
using System;
using System.Net;
using System.Threading.Tasks;
using WWI.Core3.Middleware.Base;

namespace WWI.Core3.Middleware.ExceptionHandler
{
    /// <summary>
    /// Exception Handler Middleware
    /// </summary>
    public class ExceptionHandler : BaseMiddleware
    {

        /// <summary>
        /// Current hosting environment
        /// </summary>
        private readonly IHostingEnvironment _hostingEnvironment;


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="requestDelegate">Request Delegate</param>
        /// <param name="hostingEnvironment">Hosting Environment</param>
        /// <param name="serviceProvider">ServiceProvider required Dependency Injection (DI)</param>
        public ExceptionHandler(RequestDelegate requestDelegate, IHostingEnvironment hostingEnvironment, IServiceProvider serviceProvider)
            : base(requestDelegate, serviceProvider)
        {
            _hostingEnvironment = hostingEnvironment;
        }

        /// <summary>
        /// Invoke as an asynchronous operation.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>Task.</returns>
        public override async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await Next.Invoke(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }

        }

        /// <summary>
        /// Handles the exception asynchronously
        /// </summary>
        /// <param name="httpContext">The HTTP context.</param>
        /// <param name="ex">The ex.</param>
        /// <returns>Task.</returns>
        private Task HandleExceptionAsync(HttpContext httpContext, Exception ex)
        {
            // TODO: Log the exception here!
            Log.Error(ex.ToString());

            httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            return httpContext.Response.WriteAsync(ex.Message);
        }


    }
}
