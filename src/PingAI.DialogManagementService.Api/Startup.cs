using System;
using System.Linq;
using System.Security.Claims;
using FluentValidation.AspNetCore;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using PingAI.DialogManagementService.Api.Authorization.Handlers;
using PingAI.DialogManagementService.Api.Authorization.Services;
using PingAI.DialogManagementService.Api.Behaviours;
using PingAI.DialogManagementService.Api.Filters;
using PingAI.DialogManagementService.Api.Models;
using PingAI.DialogManagementService.Application.Interfaces.Persistence;
using PingAI.DialogManagementService.Application.Interfaces.Services;
using PingAI.DialogManagementService.Application.Interfaces.Services.Nlu;
using PingAI.DialogManagementService.Application.Projects.UpdateProject;
using PingAI.DialogManagementService.Infrastructure.Persistence;
using PingAI.DialogManagementService.Infrastructure.Persistence.Repositories;
using PingAI.DialogManagementService.Infrastructure.Services.Nlu;
using IAuthorizationService = PingAI.DialogManagementService.Application.Interfaces.Services.IAuthorizationService;

namespace PingAI.DialogManagementService.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
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
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                    options.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
                    options.SerializerSettings.DateFormatString = "yyyy-MM-ddTHH:mm:ss.fffzzz";
                    options.SerializerSettings.Converters.Add(new StringEnumConverter());
                });
            
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
                options.Audience = Configuration["OAuth:Audience"];
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = Configuration["Oauth:TokenIssuer"],
                    NameClaimType = ClaimTypes.NameIdentifier,
                    RequireExpirationTime = true
                };
            });
            
            services.AddScoped<IAuthorizationHandler, ProjectAuthorizationHandler>();
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

            services.AddHttpClient<INluService, NluService>(client =>
            {
                client.BaseAddress = new Uri(Configuration["NluApiHost"]);
            });
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
}
