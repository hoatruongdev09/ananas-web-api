using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace Ananas.Web.Models {
    public class ProductFigure {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public int Price { get; set; }
        public string Infomation { get; set; }
        public int Status { get; set; }
        public int Branch { get; set; }
        public int Gender { get; set; }
        public int[] Color { get; set; }
        public int[] Category { get; set; }
        public int[] Collection { get; set; }
        public int[] Form { get; set; }
        public int[] Material { get; set; }
        public int[] ShoeSize { get; set; }
        public int[] Size { get; set; }
        public IFormFile ImageProduct { get; set; }
        public List<IFormFile> DetailImages { get; set; }
    }
}