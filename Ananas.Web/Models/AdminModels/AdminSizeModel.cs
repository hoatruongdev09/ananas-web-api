using System.Collections.Generic;
using Ananas.Data.Models;

namespace Ananas.Web.Models.AdminModels {
    public class AdminSizeModel {
        public List<SizeModel> SizeList { get; set; }
        public List<ShoeSizeModel> ShoeSizeList { get; set; }
    }
}