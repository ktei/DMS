using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.AspNetCore;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PingAI.DialogManagementService.Api.Behaviours;
using PingAI.DialogManagementService.Api.Filters;
using PingAI.DialogManagementService.Api.Models;
using PingAI.DialogManagementService.Application.Interfaces.Persistence;
using PingAI.DialogManagementService.Application.Projects.UpdateProject;
using PingAI.DialogManagementService.Domain.ErrorHandling;
using PingAI.DialogManagementService.Infrastructure.Persistence;
using PingAI.DialogManagementService.Infrastructure.Persistence.Repositories;

namespace PingAI.DialogManagementService.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
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
                });
            services.AddMediatR(typeof(UpdateProjectCommand));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));

            services.AddTransient<IProjectRepository, ProjectRepository>();
            
            services.AddDbContext<DialogManagementContext>(options =>
                options.UseNpgsql(Configuration.GetConnectionString("DialogManagement")));
            services.AddTransient<IUnitOfWork, UnitOfWork>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // app.UseHttpsRedirection();

            app.UseRouting();

            // app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
