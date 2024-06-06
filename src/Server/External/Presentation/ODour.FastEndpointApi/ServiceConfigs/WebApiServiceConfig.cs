using System;
using System.Security.Cryptography;
using System.Text;
using FastEndpoints.Security;
using FastEndpoints.Swagger;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using NSwag;
using ODour.Configuration.Presentation.WebApi.Authentication;
using ODour.Configuration.Presentation.WebApi.Swagger;

namespace ODour.FastEndpointApi.ServiceConfigs;

internal static class WebApiServiceConfig
{
    internal static IServiceCollection Config(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        // ====
        var authOption = configuration
            .GetRequiredSection(key: "Authentication")
            .Get<JwtAuthenticationOption>();

        services.AddAuthenticationJwtBearer(
            signingOptions: jwtSigningOption =>
            {
                jwtSigningOption.SigningKey = authOption.Jwt.IssuerSigningKey;
            },
            bearerOptions: jwtBearerOption =>
            {
                jwtBearerOption.TokenValidationParameters = new()
                {
                    ValidateIssuer = authOption.Jwt.ValidateIssuer,
                    ValidateAudience = authOption.Jwt.ValidateAudience,
                    ValidateLifetime = authOption.Jwt.ValidateLifetime,
                    ValidateIssuerSigningKey = authOption.Jwt.ValidateIssuerSigningKey,
                    RequireExpirationTime = authOption.Jwt.RequireExpirationTime,
                    ValidTypes = authOption.Jwt.ValidTypes,
                    ValidIssuer = authOption.Jwt.ValidIssuer,
                    ValidAudience = authOption.Jwt.ValidAudience,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        key: new HMACSHA256(
                            key: Encoding.UTF8.GetBytes(s: authOption.Jwt.IssuerSigningKey)
                        ).Key
                    )
                };

                jwtBearerOption.Validate();
            }
        );

        // ====
        services.AddAuthorization();

        // ====
        services.AddCors(setupAction: config =>
        {
            config.AddDefaultPolicy(configurePolicy: policy =>
            {
                policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
            });
        });

        // ====
        services.AddDataProtection();

        // ====
        services.AddLogging(configure: config =>
        {
            config.ClearProviders();
            config.AddConsole();
        });

        // ====
        services.AddResponseCaching();

        // ====
        var swaggerOption = configuration
            .GetRequiredSection(key: "Swagger")
            .GetRequiredSection(key: "NSwag")
            .Get<NSwagOption>();

        services.SwaggerDocument(documentOption =>
        {
            documentOption.DocumentSettings = setting =>
            {
                setting.PostProcess = document =>
                {
                    document.Info = new()
                    {
                        Version = swaggerOption.Doc.PostProcess.Info.Version,
                        Title = swaggerOption.Doc.PostProcess.Info.Title,
                        Description = swaggerOption.Doc.PostProcess.Info.Description,
                        Contact = new()
                        {
                            Name = swaggerOption.Doc.PostProcess.Info.Contact.Name,
                            Email = swaggerOption.Doc.PostProcess.Info.Contact.Email
                        },
                        License = new()
                        {
                            Name = swaggerOption.Doc.PostProcess.Info.License.Name,
                            Url = new(value: swaggerOption.Doc.PostProcess.Info.License.Url)
                        }
                    };
                };

                setting.AddAuth(
                    schemeName: JwtBearerDefaults.AuthenticationScheme,
                    securityScheme: new()
                    {
                        Type = (OpenApiSecuritySchemeType)
                            Enum.ToObject(
                                enumType: typeof(OpenApiSecuritySchemeType),
                                value: swaggerOption.Doc.Auth.Bearer.Type
                            ),
                        In = (OpenApiSecurityApiKeyLocation)
                            Enum.ToObject(
                                enumType: typeof(OpenApiSecurityApiKeyLocation),
                                value: swaggerOption.Doc.Auth.Bearer.In
                            ),
                        Scheme = swaggerOption.Doc.Auth.Bearer.Scheme,
                        BearerFormat = swaggerOption.Doc.Auth.Bearer.BearerFormat,
                        Description = swaggerOption.Doc.Auth.Bearer.Description,
                    }
                );
            };

            documentOption.EnableJWTBearerAuth = swaggerOption.EnableJWTBearerAuth;
        });

        return services;
    }
}
