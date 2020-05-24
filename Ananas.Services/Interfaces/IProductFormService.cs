using System.Collections.Generic;
using System.Threading.Tasks;
using Ananas.Data.Models;

namespace Ananas.Services.Interfaces {
    public interface IProductFormService : IBaseService<ProductFormModel> {
        Task<int> CreateProductForms (ProductFormsModel model);
        Task<List<ProductModel>> GetProductByFormID (int id);
        Task<List<ProductFormModel>> GetFormByProductID (int id);
        Task<int> DeleteFormsByProductId (int id);
    }
}