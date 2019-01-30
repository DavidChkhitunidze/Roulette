using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Serialization;
using NSwag;
using NSwag.AspNetCore;
using NSwag.SwaggerGeneration.Processors.Security;
using Roulette.Domain.Entities;
using Roulette.Persistance;
using Roulette.Services.Bets;
using Roulette.Services.Bets.Abstracts;
using Roulette.Services.GameHistories;
using Roulette.Services.GameHistories.Abstracts;
using Roulette.Services.Jackpots;
using Roulette.Services.Jackpots.Abstracts;
using Roulette.Services.Responses;
using Roulette.Services.Users;
using Roulette.Services.Users.Abstracts;
using Roulette.Services.Users.Models.Write;

namespace Roulette.Api
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
            // Add DbContext using SQL Server Provider and initialize migration assembly
            services.AddDbContext<RouletteDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("RouletteDatabase"),
                ma => ma.MigrationsAssembly("Roulette.Persistance")));

            // Add Identity and Login/Registration settings
            services.AddIdentity<User, UserRole>(settings =>
            {
                settings.Lockout.MaxFailedAccessAttempts = 3;
                settings.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10);
                settings.Password.RequiredLength = 4;
                settings.Password.RequireNonAlphanumeric = false;
                settings.Password.RequireUppercase = false;
                settings.Password.RequireDigit = false;
                settings.Password.RequireLowercase = false;
                settings.User.RequireUniqueEmail = true;
            })
                .AddEntityFrameworkStores<RouletteDbContext>()
                .AddDefaultTokenProviders();

            services.AddCors(o => o.AddPolicy("Roulette", builder =>
            {
                builder.AllowAnyOrigin()
                       .AllowAnyMethod()
                       .AllowAnyHeader();
            }));

            // Add Jwt Authentication and Bearer configurations
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(config =>
                {
                    config.RequireHttpsMetadata = true;
                    config.SaveToken = true;
                    config.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateAudience = true,
                        ValidateIssuer = true,
                        ValidateIssuerSigningKey = true,
                        ValidateLifetime = true,
                        ValidIssuer = Configuration["JwtBearerAuthentication:JwtIssuer"],
                        ValidAudience = Configuration["JwtBearerAuthentication:JwtIssuer"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JwtBearerAuthentication:JwtSecretKey"])),
                        ClockSkew = TimeSpan.Zero
                    };
                    config.Events = new JwtBearerEvents
                    {
                        OnAuthenticationFailed = context =>
                        {
                            if (context.Exception.GetType() == typeof(SecurityTokenException))
                                context.Response.Headers.Add("Token-Expired", "true");

                            return Task.CompletedTask;
                        }
                    };
                });

            // Add MVC and compatibility version
            services.AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
                .AddJsonOptions(options =>
                {
                    options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                })
                .AddFluentValidation();

            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = (context) =>
                {
                    var validationErrors = context.ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToArray();

                    var response = new Response();
                    response.SetStatusCode(System.Net.HttpStatusCode.UnprocessableEntity);
                    response.SetErrorMessages(validationErrors);

                    return new BadRequestObjectResult(response);
                };
            });

            // Add Swagger
            services.AddSwaggerDocument(config =>
            {
                config.OperationProcessors.Add(new OperationSecurityScopeProcessor("JWT Token"));
                config.DocumentProcessors.Add(new SecurityDefinitionAppender("JWT Token", new SwaggerSecurityScheme
                {
                    Type = SwaggerSecuritySchemeType.ApiKey,
                    Name = "Authorization",
                    Description = "Copy 'Bearer ' + valid JWT token into field",
                    In = SwaggerSecurityApiKeyLocation.Header
                }));

                config.PostProcess = document =>
                {
                    document.Info.Version = "v1";
                    document.Info.Title = "Roulette API";
                    document.Info.Description = "Roulette API Example";
                    document.Info.TermsOfService = "None";
                    document.Info.Contact = new SwaggerContact
                    {
                        Name = "David Chkhitunidze",
                        Email = "davidchkhitunidze@gmail.com"
                    };
                };
            });

            // Add Transient dto validations with FluentValidation
            services.AddTransient<IValidator<LoginUserDto>, LoginUserValidator>();
            services.AddTransient<IValidator<RegisterUserDto>, RegisterUserValidator>();

            // Add Scoped services for logics
            services.AddScoped<IGameHisotyService, GameHistoryService>();
            services.AddScoped<IJackpotService, JackpotService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IBetService, BetService>();
        }
        
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseCors("Roulette");

            app.UseHttpsRedirection();
            app.UseAuthentication();

            app.UseSwagger();
            app.UseSwaggerUi3();

            app.UseMvc();
        }
    }
}
