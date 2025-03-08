namespace DeliveryProductAPI.Server.Dtos
{
    public class UploadImageDto
    {
        public int ChatId { get; set; }
        public IFormFile File { get; set; }
    }
}
