﻿// <copyright file="OAuthTestStartup.cs" company="Allan Hardy">
// Copyright (c) Allan Hardy. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace App.Metrics.AspNetCore.Integration.Facts.Startup
{
    public class OAuthTestStartup : TestStartup
    {
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            SetFakeClaimsPrincipal(app);

            app.UseMetricsAllMiddleware();

            SetupAppBuilder(app, env, loggerFactory);
        }

        public void ConfigureServices(IServiceCollection services)
        {
            var appMetricsOptions = new MetricsOptions
                                    {
                                        MetricsEnabled = true
                                    };

            var appMetricsMiddlewareOptions = new MetricsAspNetCoreOptions
                                              {
                                                  MetricsTextEndpointEnabled = true,
                                                  MetricsEndpointEnabled = true,
                                                  PingEndpointEnabled = true,
                                                  OAuth2TrackingEnabled = true
                                              };

            SetupServices(services, appMetricsOptions, appMetricsMiddlewareOptions);
        }

        private static void SetFakeClaimsPrincipal(IApplicationBuilder app)
        {
            app.Use(
                (context, func) =>
                {
                    var clientId = string.Empty;

                    if (context.Request.Path.Value.Contains("oauth"))
                    {
                        clientId = context.Request.Path.Value.Split('/').Last();
                    }

                    if (!string.IsNullOrWhiteSpace(clientId))
                    {
                        context.User =
                            new ClaimsPrincipal(
                                new List<ClaimsIdentity>
                                {
                                    new ClaimsIdentity(
                                        new[]
                                        {
                                            new Claim("client_id", clientId)
                                        })
                                });
                    }

                    return func();
                });
        }
    }
}