using System.Collections.Generic;
using System.Threading.Tasks;
using Ananas.Data.Models;

namespace Ananas.Services.Interfaces {
    public interface IProductImageService {
        Task<int> CreateProductImage (ProductImageModel model);
        Task<List<string>> GetImageByProductId (int id);
        Task<int> RemoveImageByProductId (int id);
    }
}