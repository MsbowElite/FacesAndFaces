﻿using Faces.WebMvc.Models;
using Faces.WebMvc.ViewModels;
using MassTransit;
using Messaging.InterfacesConstants.Commands;
using Messaging.InterfacesConstants.Constants;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Faces.WebMvc.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IBusControl _busControl;

        public HomeController(ILogger<HomeController> logger, IBusControl busControl)
        {
            _logger = logger;
            _busControl = busControl;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult RegisterOrder()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RegisterOrder(OrderViewModel viewModel)
        {
            MemoryStream memoryStream = new();

            using var uploadedFile = viewModel.File.OpenReadStream();
            await uploadedFile.CopyToAsync(memoryStream);

            viewModel.ImageData = memoryStream.ToArray();
            viewModel.ImageUrl = viewModel.File.FileName;
            viewModel.OrderId = Guid.NewGuid();

            var sendToUri = new Uri($"{RabbitMqMassTransitConstants.RabbitMquri}" +
                $"{RabbitMqMassTransitConstants.RegisterOrderCommandQueue}");

            var endPoint = await _busControl.GetSendEndpoint(sendToUri);
            await endPoint.Send<IRegisterOrderCommand>(
                new
                {
                    viewModel.OrderId,
                    viewModel.UserEmail,
                    viewModel.ImageData,
                    viewModel.ImageUrl
                });

            ViewData["OrderId"] = viewModel.OrderId;
            return View("RegisterOrderResult");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
