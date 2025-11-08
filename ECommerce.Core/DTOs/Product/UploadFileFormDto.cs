
using Microsoft.AspNetCore.Http;

namespace ECommerce.Core.DTOs.Product
{
    public class UploadFileFormDto
    {
        public List<IFormFile> Files { get; set; }
        public int ProductId { get; set; }
    }
}
