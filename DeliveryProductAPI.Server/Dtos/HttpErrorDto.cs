using System.Text.Json;

namespace DeliveryProductAPI.Server.Dtos
{
    public class HttpErrorDto
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }

        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}
