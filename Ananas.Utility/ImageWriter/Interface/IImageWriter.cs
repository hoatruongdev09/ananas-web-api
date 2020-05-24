using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
namespace Ananas.Utility.ImageWriter.Interface {
    public interface IImageWriter {
        Task<string> UploadImage (IFormFile file);
        Task<string> UploadImage (IFormFile file, string place);

        Task<int> RemoveImage (string fileName, string place);

    }
}