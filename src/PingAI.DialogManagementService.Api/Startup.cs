using System;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Text.Json.Serialization;
using FluentValidation.AspNetCore;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Npgsql.Logging;
using PingAI.DialogManagementService.Api.Authorization;
using PingAI.DialogManagementService.Api.Authorization.Handlers;
using PingAI.DialogManagementService.Api.Authorization.Requirements;
using PingAI.DialogManagementService.Api.Authorization.Services;
using PingAI.DialogManagementService.Api.Behaviours;
using PingAI.DialogManagementService.Api.Filters;
using PingAI.DialogManagementService.Api.Models;
using PingAI.DialogManagementService.Application.Interfaces.Persistence;
using PingAI.DialogManagementService.Application.Interfaces.Services;
using PingAI.DialogManagementService.Application.Interfaces.Services.Nlu;
using PingAI.DialogManagementService.Application.Interfaces.Services.Slack;
using PingAI.DialogManagementService.Application.Projects.UpdateProject;
using PingAI.DialogManagementService.Infrastructure.Configuration;
using PingAI.DialogManagementService.Infrastructure.Persistence;
using PingAI.DialogManagementService.Infrastructure.Persistence.Repositories;
using PingAI.DialogManagementService.Infrastructure.Services.Nlu;
using PingAI.DialogManagementService.Infrastructure.Services.Slack;
using Swashbuckle.AspNetCore.SwaggerGen;
using IAuthorizationService = PingAI.DialogManagementService.Application.Interfaces.Services.IAuthorizationService;

namespace PingAI.DialogManagementService.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            
            // Uncomment this for SQL debugging
            // NpgsqlLogManager.Provider = new ConsoleLoggingProvider(NpgsqlLogLevel.Debug,
            //     true);
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers(options => options.Filters.Add<DomainExceptionFilter>())
                .ConfigureApiBehaviorOptions(options =>
                {
                    options.InvalidModelStateResponseFactory = c =>
                    {
                        var errors =
                            c.ModelState.Values.Where(v => v.Errors.Count > 0)
                                .SelectMany(v => v.Errors)
                                .Select(v => v.ErrorMessage);
                        return new BadRequestObjectResult(new ErrorsDto(errors));
                    };
                })
                .AddFluentValidation(fv =>
                {
                    fv.RunDefaultMvcValidationAfterFluentValidationExecutes = false;
                    fv.RegisterValidatorsFromAssemblyContaining<Startup>();
                })
                .AddJsonOptions(options => {
                    options.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
                    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                    options.JsonSerializerOptions.IgnoreReadOnlyProperties = false;
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                    options.JsonSerializerOptions.Converters.Add(new DateTimeConverter());
                });
                // .AddNewtonsoftJson(options =>
                // {
                //     options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                //     options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                //     options.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
                //     options.SerializerSettings.DateFormatString = "yyyy-MM-ddTHH:mm:ss.fffzzz";
                //     options.SerializerSettings.Converters.Add(new StringEnumConverter());
                // });
            
            services.AddHealthChecks();
            services.AddApiVersioning();
            
            // Authentication setup
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.Authority = Configuration["OAuth:Authority"];
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = Configuration["Oauth:TokenIssuer"],
                    NameClaimType = ClaimTypes.NameIdentifier,
                    RequireExpirationTime = true,
                    ValidAudiences = Configuration["OAuth:Audience"]
                        .Split(",", StringSplitOptions.RemoveEmptyEntries)
                };
            });
            services.AddAuthorization(options =>
            {
                options.AddPolicy(Policies.AdminOnly, policy =>
                    policy.Requirements.Add(new AdministratorOnlyRequirement()));
            });
            
            services.AddScoped<IAuthorizationHandler, ProjectAuthorizationHandler>();
            services.AddScoped<IAuthorizationHandler, AdministratorOnlyAuthorizationHandler>();
            services.AddHttpContextAccessor();
            services.AddTransient<IAuthorizationService, AuthorizationService>();
            services.AddTransient<IRequestContext, HttpRequestContext>();

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "Dialog Management API v1"
                });
                
                options.SwaggerDoc("v1.1", new OpenApiInfo
                {
                    Version = "v1.1",
                    Title = "Dialog Management API v1.1"
                });
                
                options.OperationFilter<RemoveVersionFromParameter>();
                options.DocumentFilter<ReplaceVersionWithExactValueInPath>();
                
                options.DocInclusionPredicate((version, desc) =>
                {
                    var controllerActionDescriptor = desc.ActionDescriptor as ControllerActionDescriptor;
                    
                    var versions = 
                        controllerActionDescriptor?.ControllerTypeInfo.GetCustomAttributes(true)
                        .OfType<ApiVersionAttribute>()
                        .SelectMany(attr => attr.Versions) ?? new ApiVersion[0];

                    var mappedVersions = new ApiVersion[0];
                    if (desc.TryGetMethodInfo(out var mi))
                    {
                        mappedVersions = mi.GetCustomAttributes(true).OfType<MapToApiVersionAttribute>()
                            .SelectMany(attr => attr.Versions)
                            .ToArray();
                    }

                    return versions.Any(v => $"v{v.ToString()}" == version)
                           && (!mappedVersions.Any() || mappedVersions.Any(v => $"v{v.ToString()}" == version));;
                });
                
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please enter JWT with Bearer into field",
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey
                });
                
                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] { }
                    }
                });
                
                options.DescribeAllParametersInCamelCase();
            });
            
            services.AddMediatR(typeof(UpdateProjectCommand));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));

            services.AddDbContext<DialogManagementContext>(options =>
                options.UseNpgsql(Configuration.GetConnectionString("DialogManagement")));
            
            // TODO: DI these in a smarter way (Autofac for example)
            services.AddTransient<IUnitOfWork, UnitOfWork>();
            services.AddTransient<IProjectRepository, ProjectRepository>();
            services.AddTransient<IOrganisationRepository, OrganisationRepository>();
            services.AddTransient<IUserRepository, UserRepository>();
            services.AddTransient<IIntentRepository, IntentRepository>();
            services.AddTransient<IEntityTypeRepository, EntityTypeRepository>();
            services.AddTransient<IEntityNameRepository, EntityNameRepository>();
            services.AddTransient<IResponseRepository, ResponseRepository>();
            services.AddTransient<IQueryRepository, QueryRepository>();
            services.AddTransient<IProjectVersionRepository, ProjectVersionRepository>();
            services.AddTransient<ISlackWorkspaceRepository, SlackWorkspaceRepository>();

            services.AddHttpClient<INluService, NluService>(client =>
            {
                client.BaseAddress = new Uri(Configuration["NluApiHost"]);
            });
            services.AddHttpClient<ISlackService, SlackService>(client =>
            {
                client.BaseAddress = new Uri(Configuration["Slack:ApiHost"]);
            });

            services.AddSingleton<IConfigurationManager, ConfigurationManager>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (!env.IsProduction())
            {
                app.UseDeveloperExceptionPage();
                
                app
                    .UseSwagger(opts =>
                    {
                        opts.RouteTemplate = $"{Configuration["RoutePrefix"]}/swagger/{{documentName}}/swagger.json";
                    })
                    .UseSwaggerUI(x =>
                    {
                        x.SwaggerEndpoint($"/{Configuration["RoutePrefix"]}/swagger/v1/swagger.json",
                            $"Dialog Management API v1");
                        x.SwaggerEndpoint($"/{Configuration["RoutePrefix"]}/swagger/v1.1/swagger.json",
                            $"Dialog Management API v1.1");                        
                        x.RoutePrefix = $"{Configuration["RoutePrefix"]}/swagger";
                    });
            }

            // app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();
            
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHealthChecks($"/{Configuration["RoutePrefix"]}/health");
                endpoints.MapControllers();
            });
        }
    }

    internal class ConfigurationManager : IConfigurationManager
    {
        private readonly IConfiguration _configuration;

        public ConfigurationManager(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string SlackClientId => _configuration["Slack:ClientId"];
        public string SlackClientSecret => _configuration["Slack:ClientSecret"];
    }
    
    public class DateTimeConverter : JsonConverter<DateTime>
    {
        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            Debug.Assert(typeToConvert == typeof(DateTime));
            return DateTime.Parse(reader.GetString());
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffzzz"));
        }
    }
    
    internal class RemoveVersionFromParameter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var versionParameter = operation.Parameters.Single(p => p.Name == "version");
            operation.Parameters.Remove(versionParameter);

        }
    }
    
    internal class ReplaceVersionWithExactValueInPath : IDocumentFilter
    {
        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            var oldPaths = swaggerDoc.Paths;
            swaggerDoc.Paths = new OpenApiPaths();
            foreach (var (key, value) in oldPaths)
            {
                swaggerDoc.Paths[key.Replace("v{version}", swaggerDoc.Info.Version)] = value;
            }
        }
    }
}
