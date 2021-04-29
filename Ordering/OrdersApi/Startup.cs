using GreenPipes;
using MassTransit;
using Messaging.InterfacesConstants.Constants;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OrdersApi.AutoMappers;
using OrdersApi.Configurations;
using OrdersApi.Hubs;
using OrdersApi.Messages.Consumers;
using OrdersApi.Persistence;
using OrdersApi.Persistence.Repositories;
using OrdersApi.Persistence.Repositories.Interfaces;
using OrdersApi.Services;
using System;

namespace OrdersApi
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
            services.Configure<OrderSettings>(Configuration);

            services.AddDbContext<OrdersDbContext>(optionsAction => optionsAction.UseSqlServer
            (
                Configuration["OrdersContextConnection"]
            ));

            services.AddSignalR()
                .AddJsonProtocol(options =>
                {
                    options.PayloadSerializerOptions.PropertyNamingPolicy = null;
                });

            services.AddHttpClient();

            services.AddTransient<IOrderRepository, OrderRepository>();

            services.AddMassTransit(x =>
            {
                x.AddConsumer<RegisterOrderCommandConsumer>();
                x.AddConsumer<OrderDispatchedEventConsumer>();
                x.AddBus(provider => Bus.Factory.CreateUsingRabbitMq(cfg =>
                {
                    cfg.UseHealthCheck(provider);
                    cfg.Host(new Uri("rabbitmq://rabbitmq"), h =>
                    {
                        h.Username("guest");
                        h.Password("guest");
                    });
                    cfg.ReceiveEndpoint(RabbitMqMassTransitConstants.RegisterOrderCommandQueue, ep =>
                    {
                        ep.PrefetchCount = 16;
                        ep.UseMessageRetry(r => r.Interval(2, 100));
                        ep.ConfigureConsumer<RegisterOrderCommandConsumer>(provider);

                    });
                    cfg.ReceiveEndpoint(RabbitMqMassTransitConstants.OrderDispatchedServiceQueue, ep =>
                    {
                        ep.PrefetchCount = 16;
                        ep.UseMessageRetry(r => r.Interval(2, 100));
                        ep.ConfigureConsumer<OrderDispatchedEventConsumer>(provider);

                    });

                }));
            });
            services.AddMassTransitHostedService();

            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .SetIsOriginAllowed((host) => true)
                        .AllowCredentials());
            });

            services.AddAutoMapperConfiguration();
            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors("CorsPolicy");

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<OrderHub>("/orderhub");
            });
        }
    }
}
