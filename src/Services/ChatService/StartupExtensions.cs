using ChatService.Application.Features.Conversations.Consumers;
using ChatService.Application.Features.Users.Commands.Register;
using ChatService.Application.Jobs;
using ChatService.Domain;
using ChatService.Infrastructure;
using ChatService.Realtime.Services;
using ChatService.Realtime.Services.Interfaces;
using FluentValidation;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Quartz;
using StackExchange.Redis;
using System.Text;

namespace ChatService
{
    public static class StartupExtensions
    {
        public static WebApplicationBuilder AddApplicationServices(this WebApplicationBuilder builder)
        {
            builder.Services.AddScoped<IChatSessionService, ChatSessionService>();
            builder.Services.AddScoped<IChatService, Realtime.Services.ChatService>();

            return builder;
        }

        public static WebApplicationBuilder AddAppAuthorization(this WebApplicationBuilder builder)
        {
            builder.Services.AddAuthorization();
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = false,
                    ValidateIssuerSigningKey = false,
                    ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
                    ValidAudience = builder.Configuration["JwtSettings:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:SecretKey"]))
                };
            });

            return builder;
        }

        public static WebApplicationBuilder AddAppIdentity(this WebApplicationBuilder builder)
        {

            builder.Services.AddIdentity<User, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            builder.Services.Configure<IdentityOptions>(options =>
            {
                options.SignIn.RequireConfirmedAccount = true;
                // options.SignIn.RequireConfirmedEmail = true;
                options.SignIn.RequireConfirmedPhoneNumber = false;
            });

            return builder;
        }

        public static WebApplicationBuilder AddQuartz(this WebApplicationBuilder builder)
        {
            builder.Services.AddQuartz(q =>
            {
                q.UsePersistentStore(pc =>
                {
                    pc.Properties.Add("quartz.jobStore.tablePrefix", "quartz.qrtz_");
                    pc.UsePostgres(builder.Configuration.GetConnectionString("ApplicationDbContext"));
                    pc.UseSystemTextJsonSerializer();
                });

                var cancelExpiredOrdersJobKey = new JobKey("SendUnviewedMessagesToEmailJob");
                q.AddJob<SendUnviewedMessagesToEmailJob>(opts => opts.WithIdentity(cancelExpiredOrdersJobKey));

                q.AddTrigger(opts => opts
                    .ForJob(cancelExpiredOrdersJobKey)
                    .WithIdentity("SendUnviewedMessagesToEmailJob-trigger")
                    .WithCronSchedule("0 * * ? * *")); // Every 10 seconds
            });
            builder.Services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

            return builder;
        }

        public static WebApplicationBuilder AddMediatr(this WebApplicationBuilder builder)
        {
            builder.Services.AddValidatorsFromAssembly(typeof(RegisterCommandHandler).Assembly);
            builder.Services.AddMediatR(config =>
            {
                config.RegisterServicesFromAssemblyContaining<RegisterCommandHandler>();
            });

            return builder;
        }

        public static WebApplicationBuilder AddRedisIntegration(this WebApplicationBuilder builder)
        {
            var redisConnectionString = builder.Configuration.GetValue<string>("REDIS_CONNECTION");
            builder.Services.AddScoped<IDatabase>(cfg =>
            {
                IConnectionMultiplexer multiplexer = ConnectionMultiplexer.Connect(redisConnectionString);
                return multiplexer.GetDatabase();
            });
            builder.Services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = redisConnectionString;
                options.InstanceName = "Chat";
            });

            return builder;
        }

        public static WebApplicationBuilder AddRabbitMqIntegration(this WebApplicationBuilder builder)
        {
            builder.Services.AddMassTransit(x =>
            {
                x.AddEntityFrameworkOutbox<ApplicationDbContext>(o =>
                {
                    o.UsePostgres();

                    // enable the bus outbox
                    o.UseBusOutbox();
                });

                x.AddConsumers(typeof(SendConversationUserMessageConsumer).Assembly);
                x.UsingRabbitMq((context, cfg) =>
                {
                    var configuration = context.GetRequiredService<IConfiguration>();
                    cfg.Host(configuration.GetValue<Uri>("BUS_CONNECTION"));

                    cfg.ConfigureEndpoints(context);
                });
            });

            return builder;
        }

        public static WebApplication MigrateDatabase<TDbContext>(this WebApplication app)
    where TDbContext : DbContext
        {
            using (var scope = app.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<TDbContext>();

                dbContext.Database.Migrate();
            }

            return app;
        }
    }
}
