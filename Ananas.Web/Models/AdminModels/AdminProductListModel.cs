using System.Collections.Generic;
using Ananas.Data.Models;

namespace Ananas.Web.Models {
    public class AdminProductListModel {
        public List<ProductModel> ListProduct { get; set; }
        public List<CategoryModel> ListCategory { get; set; }
        public List<BranchModel> ListBranch { get; set; }
        public List<ColorModel> ListColor { get; set; }
        public List<GenderModel> ListGender { get; set; }
        public List<SizeModel> ListSize { get; set; }
        public List<ShoeSizeModel> ListShoeSize { get; set; }
        public List<CollectionModel> ListCollection { get; set; }
        public List<ProductFormModel> ListForm { get; set; }
        public List<StatusModel> ListStatus { get; set; }
        public List<MaterialModel> ListMaterial { get; set; }
    }
}