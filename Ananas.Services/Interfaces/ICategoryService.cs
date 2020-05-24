using System.Collections.Generic;
using System.Threading.Tasks;
using Ananas.Data.Models;

namespace Ananas.Services.Interfaces {
    public interface ICategoryService : IBaseService<CategoryModel> {
        Task<List<CategoryModel>> GetCategoriesByProductID (int productID);
        Task<List<ProductModel>> GetProductByCategoryID (int categoryID);
        Task<int> CreateProductCategory (ProductCategoryModel model);
        Task<int> DeleteCategoriesByProductId (int id);
    }
}