﻿using System.Collections.Generic;
using flooding_service.Helpers;
using flooding_service.Models;
using flooding_service.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using StockportGovUK.NetStandard.Gateways.Extensions;
using StockportGovUK.NetStandard.Gateways.MailingService;
using StockportGovUK.NetStandard.Gateways.VerintService;

namespace flooding_service.Utils.ServiceCollectionExtensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddGateways(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHttpClient<IVerintServiceGateway, VerintServiceGateway>(configuration);
            services.AddHttpClient<IMailingServiceGateway, MailingServiceGateway>(configuration);

            return services;
        }

        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddSingleton<IFloodingService, FloodingService>();

            return services;
        }

        public static IServiceCollection AddHelpers(this IServiceCollection services)
        {
            services.AddSingleton<IStreetHelper, StreetHelper>();

            return services;
        }

        public static IServiceCollection AddIOptions(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<PavementVerintOptions>(configuration.GetSection(PavementVerintOptions.ConfigValue));
            services.Configure<ConfirmAttributeFormOptions>(configuration.GetSection(ConfirmAttributeFormOptions.ConfigValue));

            return services;
        }

        public static void AddSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Flooding Service API", Version = "v1" });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    In = ParameterLocation.Header,
                    Description = "Authorization using the Bearer scheme. Example: \"Authorization: Bearer {token}\""
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
                        },
                        new List<string>()
                    }
                });
            });
        }
    }
}
