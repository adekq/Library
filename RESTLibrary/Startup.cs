using JwtSecurity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using NSwag;
using NSwag.Generation.Processors.Security;
using RESTLibrary.Models;
using RESTLibrary.Persisters;
using System;
using System.Text;

namespace RESTLibrary
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
            var jwtTokenConfiguration = Configuration.GetSection("JwtTokenConfiguration").Get<JwtTokenConfiguration>();

            ConfigureSecurity(services, jwtTokenConfiguration);
            ConfigureModels(services);

            services.AddControllers();

            services.AddOpenApiDocument(settings =>
            {
                settings.Title = jwtTokenConfiguration.Claims.Issuer;
                settings.OperationProcessors.Add(new OperationSecurityScopeProcessor("auth"));
                settings.DocumentProcessors.Add(new SecurityDefinitionAppender("auth", new OpenApiSecurityScheme
                {
                    Type = OpenApiSecuritySchemeType.Http,
                    In = OpenApiSecurityApiKeyLocation.Header,
                    Scheme = "bearer",
                    BearerFormat = "jwt"
                }));                
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseOpenApi();
            app.UseSwaggerUi3();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        private void ConfigureSecurity(IServiceCollection services, JwtTokenConfiguration jwtTokenConfiguration)
        {
            services.AddSingleton(jwtTokenConfiguration);
            services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();

            services.AddAuthentication(authenticationOptions =>
            {
                authenticationOptions.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                authenticationOptions.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(jwtBearerOptions =>
            {
                jwtBearerOptions.RequireHttpsMetadata = true;
                jwtBearerOptions.SaveToken = true;
                jwtBearerOptions.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwtTokenConfiguration.Claims.Issuer,

                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtTokenConfiguration.Secret)),

                    ValidAudience = jwtTokenConfiguration.Claims.Audience,
                    ValidateAudience = true,

                    ValidateLifetime = true,

                    ClockSkew = TimeSpan.FromMinutes(1)
                };
            });
        }

        private void ConfigureModels(IServiceCollection services)
        {
            var adminLibrarian = Configuration.GetSection("AdminLibrarian").Get<AdminLibrarian>();
            services.AddSingleton(adminLibrarian);

            var igniteClientConfiguration = Configuration.GetSection("IgniteClientConfiguration").Get<IgniteClientConfiguration>();
            services.AddSingleton(igniteClientConfiguration);
                        
            services.AddSingleton<IgnitePersister>();
            
            services.AddSingleton<IUserServicePersister>(x => x.GetRequiredService<IgnitePersister>());
            services.AddSingleton<IBookServicePersister>(x => x.GetRequiredService<IgnitePersister>());
            services.AddSingleton<IReservationServicePersister>(x => x.GetRequiredService<IgnitePersister>());

            services.AddSingleton<IUserService, UserService>();
            services.AddSingleton<IBookService, BookService>();
            services.AddSingleton<IReservationService, ReservationService>();
        }
    }
}
