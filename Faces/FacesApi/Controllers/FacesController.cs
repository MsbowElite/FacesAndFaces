using FacesApi.Configurations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.CognitiveServices.Vision.Face;
using Microsoft.Azure.CognitiveServices.Vision.Face.Models;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace FacesApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FacesController : ControllerBase
    {
        private readonly AzureFaceConfiguration _azureFaceConfiguration;

        public FacesController(AzureFaceConfiguration azureFaceConfiguration)
        {
            _azureFaceConfiguration = azureFaceConfiguration;
        }

        [HttpPost]
        public async Task<Tuple<List<byte[]>, Guid>> ReadFaces(Guid orderId)
        {
            using var ms = new MemoryStream(2048);

            await Request.Body.CopyToAsync(ms);
            var imageBytes = ms.ToArray();
            Image image = Image.Load(imageBytes);
            image.Save("dummy.jpg");

            var facesCropped = await UploadAndDetectFaces(image, new MemoryStream(imageBytes));
            return new Tuple<List<byte[]>, Guid>(facesCropped, orderId);
        }

        public static IFaceClient Authenticate(string endpoint, string key)
        {
            return new FaceClient(new ApiKeyServiceClientCredentials(key)) { Endpoint = endpoint };
        }

        private async Task<List<byte[]>> UploadAndDetectFaces(Image image, MemoryStream imageStream)
        {
            string subKey = _azureFaceConfiguration.SubscriptionKey;
            string endPoint = _azureFaceConfiguration.Endpoint;

            var client = Authenticate(endPoint, subKey);
            var faceList = new List<byte[]>();
            IList<DetectedFace> faces = null;
            try
            {
                faces = await client.Face.DetectWithStreamAsync(imageStream, true, false, null);
                
                int j = 0;
                foreach (var face in faces)
                {
                    var s = new MemoryStream();
                    var zoom = 1.0;
                    int h = (int)(face.FaceRectangle.Height / zoom);
                    int w = (int)(face.FaceRectangle.Width / zoom);
                    int x = face.FaceRectangle.Left;
                    int y = face.FaceRectangle.Top;

                    image.Clone(ctx => ctx.Crop(new Rectangle(x, y, w, h))).Save("face" + j + ".jpg");
                    image.Clone(ctx => ctx.Crop(new Rectangle(x, y, w, h))).SaveAsJpeg(s);
                    faceList.Add(s.ToArray());

                    j++;
                }
            }
            catch (Exception ex)
            {
                Console.Out.WriteLine(ex.Message);
            }
            return faceList;

        }
    }
}
