using EmailService;
using EmailService.Interfaces;
using MassTransit;
using Messaging.InterfacesConstants.Events;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;

namespace NotificationService.Consumers
{
    public class OrderProcessedEventConsumer : IConsumer<IOrderProcessedEvent>
    {
        private static readonly string rootFolder = AppContext.BaseDirectory.Substring(0, AppContext.BaseDirectory.IndexOf("bin"));
        private readonly IEmailSender _emailSender;

        public OrderProcessedEventConsumer(IEmailSender emailSender)
        {
            _emailSender = emailSender;
        }

        public async Task Consume(ConsumeContext<IOrderProcessedEvent> context)
        {
            var result = context.Message;
            var facesData = result.Faces;

            if (facesData.Count < 1)
            {
                await Console.Out.WriteLineAsync("No faces Detected");
            }

            string[] mailAddress = { result.UserEmail };

            await _emailSender.SendEmailAsync(new Message(mailAddress, "your order" +
                result.Id, "From FacesAndFaces", facesData));

            await context.Publish<IOrderDispatchedEvent>(new
            {
                context.Message.Id,
                DispatchDateTime = DateTime.UtcNow
            });
        }
    }
}
