using System.Collections.Generic;
using System.Threading.Tasks;
using Ananas.Data.Models;

namespace Ananas.Services.Interfaces {
    public interface ISizeService : IBaseService<SizeModel> {
        Task<int> CreateProductSize (ProductSizeModel model);
        Task<List<ProductModel>> GetProductsBySizeID (int id);
        Task<List<SizeModel>> GetSizesByProductID (int id);
        Task<int> DeleteSizesByProductId (int id);
    }
}