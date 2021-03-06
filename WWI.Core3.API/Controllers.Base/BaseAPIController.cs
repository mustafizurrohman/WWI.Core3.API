﻿// ***********************************************************************
// Assembly         : WWI.Core3.API
// Author           : Mustafizur Rohman
// Created          : 05-01-2020
//
// Last Modified By : Mustafizur Rohman
// Last Modified On : 05-19-2020
// ***********************************************************************
// <copyright file="BaseAPIController.cs" company="WWI.Core3.API">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using WWI.Core3.API.ActionFilters;
using WWI.Core3.Models.DbContext;
using WWI.Core3.Services.ServiceCollection;

namespace WWI.Core3.API.Controllers.Base
{

    /// <summary>
    /// Base API Controller
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.ControllerBase" />
    [ApiController]
    [Benchmark]
    [Route("[controller]")]
    [Produces("application/json")]
    public abstract class BaseAPIController : ControllerBase
    {

        #region  -- Attributes -- 

        /// <summary>
        /// The database context
        /// </summary>
        /// <value>The database context.</value>
        protected DocAppointmentContext DbContext { get; }

        /// <summary>
        /// AutoMapper
        /// </summary>
        /// <value>The automatic mapper.</value>
        protected IMapper AutoMapper { get; }

        #endregion

        #region -- Constructor -- 

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseAPIController" /> class.
        /// </summary>
        /// <param name="applicationServices">The database context.</param>
        protected BaseAPIController(ApplicationServices applicationServices)
        {
            DbContext = applicationServices.DbContext;
            AutoMapper = applicationServices.AutoMapper;
        }

        #endregion

    }

}
