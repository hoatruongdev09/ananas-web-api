using System.Collections.Generic;
using System.Threading.Tasks;
using Ananas.Data.Models;

namespace Ananas.Services.Interfaces {
    public interface IMaterialService : IBaseService<MaterialModel> {
        Task<int> CreateProductMaterial (ProductMaterialModel model);
        Task<List<ProductModel>> GetProductsByMaterialID (int id);
        Task<List<MaterialModel>> GetMaterialsByProductID (int id);
        Task<int> DeleteMaterialsByProductId (int id);
    }
}