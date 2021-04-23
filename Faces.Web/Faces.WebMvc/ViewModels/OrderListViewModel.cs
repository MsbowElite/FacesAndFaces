using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;

namespace Faces.WebMvc.ViewModels
{
    public class OrderListViewModel
    {
        [Display(Name = "Order Id")]
        public Guid Id { get; set; }

        [Display(Name = "Email")]
        public string UserEmail { get; set; }

        [Display(Name = "Image File")]
        public IFormFile File { get; set; }

        [Display(Name = "ImageUrl")]
        public string ImageUrl { get; set; }

        [Display(Name = "Order Status")]
        public string StatusString { get; set; }

        public byte[] ImageData { get; set; }
        public byte[] ImageString { get; set; }
    }
}
