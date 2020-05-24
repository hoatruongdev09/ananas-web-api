using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Ananas.Data.Models;
namespace Ananas.Services.Interfaces {
    public interface IColorService : IBaseService<ColorModel> {
        Task<List<ProductModel>> GetProductByColorID (int idColor);
        Task<List<ColorModel>> GetColorByProductID (int idProduct);
        Task<int> CreateProductColor (ProductColorModel model);
        Task<int> DeleteColorsByProductId (int id);
    }
}