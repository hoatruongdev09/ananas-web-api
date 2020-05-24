using System.Collections.Generic;
using System.Threading.Tasks;
using Ananas.Data.Models;
namespace Ananas.Services.Interfaces {
    public interface ICollectionService : IBaseService<CollectionModel> {
        Task<List<ProductModel>> GetProductsByCollectionID (int collectionID);
        Task<List<CollectionModel>> GetCollectionsByProductID (int productID);
        Task<int> CreateProductCollection (ProductCollectionModel model);
        Task<int> DeleteCollectionsByProductId (int id);
    }
}