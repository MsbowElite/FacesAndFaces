using EmailService;
using EmailService.Interfaces;
using GreenPipes;
using MassTransit;
using Messaging.InterfacesConstants.Constants;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NotificationService.Consumers;
using System;
using System.IO;
using System.Threading.Tasks;

namespace NotificationService
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            await CreateHostBuilder(args).Build().RunAsync();
        }

        private static IHostBuilder CreateHostBuilder(string[] args)
        {
            var hostBuilder = Host.CreateDefaultBuilder(args)
                .ConfigureHostConfiguration(configHost =>
                {
                    configHost.SetBasePath(Directory.GetCurrentDirectory());
                    configHost.AddJsonFile($"appsettings.json", optional: false);
                    configHost.AddEnvironmentVariables();
                    configHost.AddCommandLine(args);
                })
                .ConfigureAppConfiguration((hostContext, config) =>
                {
                    config.AddJsonFile($"appsettings.{hostContext.HostingEnvironment.EnvironmentName}.json",
                        optional: false);
                })


               .ConfigureServices((hostContext, services) =>
               {
                   var emailConfig = hostContext.Configuration
                   .GetSection("EmailConfiguration")
                   .Get<EmailConfig>();
                   services.AddSingleton(emailConfig);
                   services.AddScoped<IEmailSender, EmailSender>();
                   services.AddMassTransit(x =>
                   {
                       x.AddConsumer<OrderProcessedEventConsumer>();
                       x.AddBus(provider => Bus.Factory.CreateUsingRabbitMq(cfg =>
                       {
                           cfg.UseHealthCheck(provider);
                           cfg.Host(new Uri("rabbitmq://rabbitmq"), h =>
                           {
                               h.Username("guest");
                               h.Password("guest");
                           });
                           cfg.ReceiveEndpoint(RabbitMqMassTransitConstants.NotificationServiceQueue, ep =>
                           {
                               ep.PrefetchCount = 16;
                               ep.UseMessageRetry(r => r.Interval(2, 100));
                               ep.ConfigureConsumer<OrderProcessedEventConsumer>(provider);
                           });
                       }));
                   });
                   services.AddMassTransitHostedService();
               });
            return hostBuilder;

        }
    }
}
