
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;

namespace DeliveryProductAPI.Server
{
    public class Helpers
    {
        public static async Task<byte[]> ResizeImageAsync(IFormFile file, int maxWidth = 800, int maxHeight = 600, int quality = 85)
        {
            using var image = await Image.LoadAsync(file.OpenReadStream());

            // Поддержание пропорций
            image.Mutate(x => x.Resize(new ResizeOptions
            {
                Mode = ResizeMode.Max, // Сохранение пропорций
                Size = new Size(maxWidth, maxHeight)
            }));

            // Сжатие
            var encoder = new JpegEncoder { Quality = quality };

            using var ms = new MemoryStream();
            await image.SaveAsync(ms, encoder);
            return ms.ToArray();
        }

        public static byte[] ConvertToJpg(byte[] imageBytes, int quality = 100)
        {
            using (MemoryStream inputStream = new MemoryStream(imageBytes))
            using (MemoryStream outputStream = new MemoryStream())
            {
                using (Image image = Image.Load(inputStream))
                {
                    var encoder = new JpegEncoder { Quality = quality }; // Настройка качества JPG
                    image.Save(outputStream, encoder);
                }
                return outputStream.ToArray();
            }
        }
    }
}
