using MassTransit;
using Messaging.InterfacesConstants.Constants;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OrdersApi.Messages.Consumers;
using OrdersApi.Persistence;
using OrdersApi.Persistence.Repositories;
using OrdersApi.Persistence.Repositories.Interfaces;
using OrdersApi.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

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
            services.AddDbContext<OrdersDbContext>(optionsAction => optionsAction.UseSqlServer
            (
                Configuration.GetConnectionString("OrdersContextConnection")   
            ));

            services.AddHttpClient();

            services.AddTransient<IOrderRepository, OrderRepository>();

            services.AddMassTransit(
                c =>
                {
                    c.AddConsumer<RegisterOrderCommandConsumer>();
                });

            services.AddSingleton(provider => Bus.Factory.CreateUsingRabbitMq(
                config =>
                {
                    config.Host("localhost", "/", h =>
                    {
                        h.Username("guest");
                        h.Password("guest");
                    });

                    config.ReceiveEndpoint(RabbitMqMassTransitConstants.RegisterOrderCommandQueue, e =>
                    {
                        e.PrefetchCount = 16;
                        e.Consumer<RegisterOrderCommandConsumer>(provider);
                    });
                }));
            services.AddSingleton<IHostedService, BusService>();

            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
