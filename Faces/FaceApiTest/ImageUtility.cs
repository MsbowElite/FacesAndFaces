using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace FaceApiTest
{
    public class ImageUtility
    {
        public byte[] ConvertToBytes(string imagePath)
        {
            MemoryStream memoryStream = new();
            using FileStream fileStream = new(imagePath, FileMode.Open);

            fileStream.CopyTo(memoryStream);

            var bytes = memoryStream.ToArray();
            return bytes;
        }

        public void FromBytesToImage(byte[] imageBytes, string fileName)
        {
            using MemoryStream memoryStram = new(imageBytes);

            Image img = Image.FromStream(memoryStram);
            img.Save(fileName + ".jpg", ImageFormat.Jpeg);
        }
    }
}
