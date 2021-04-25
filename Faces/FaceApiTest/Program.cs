using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;

namespace FaceApiTest
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var imagePath = @"photo.jpg";
            var urlAddress = "http://localhost:6000/api/faces";

            ImageUtility imageUtility = new();
            var bytes = imageUtility.ConvertToBytes(imagePath);
            List<byte[]> faceList = null;

            var byteContent = new ByteArrayContent(bytes);
            byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            using HttpClient httpClient = new();
            using var response = await httpClient.PostAsync(urlAddress, byteContent);

            var apiResponse = await response.Content.ReadAsStringAsync();
            faceList = JsonSerializer.Deserialize<List<byte[]>>(apiResponse);

            if (faceList is not null)
            {
                for (int i = 0; i < faceList.Count; i++)
                {
                    imageUtility.FromBytesToImage(faceList[i], "face" + i);
                }
            }
        }
    }
}
