using Ananas.Data.Models;
namespace Ananas.Web.Models {
    public class ProductDetail {
        public ProductModel Product { get; set; }
        public ColorModel[] Colors { get; set; }
        public CategoryModel[] Categories { get; set; }
        public CollectionModel[] Collections { get; set; }
        public ProductFormModel[] Forms { get; set; }
        public MaterialModel[] Materials { get; set; }
        public ShoeSizeModel[] ShoeSizes { get; set; }
        public SizeModel[] Sizes { get; set; }
        public string[] DetailImages { get; set; }
    }
}