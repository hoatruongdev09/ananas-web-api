using System.Collections.Generic;
using System.Threading.Tasks;
using Ananas.Data.Models;

namespace Ananas.Services.Interfaces {
    public interface IShoeSizeService : IBaseService<ShoeSizeModel> {
        Task<int> CreateProductShoeSize (ProductShoeSizeModel model);
        Task<List<ProductModel>> GetProductsByShoeSizeID (int id);
        Task<List<ShoeSizeModel>> GetShoeSizesByProductID (int id);
        Task<int> DeleteProductShoeSizeByProductId (int id);
    }
}