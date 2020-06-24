using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Ananas.Web.Models.AdminModels {
    public class AdminCreateProductModel {
        [Required]
        [StringLength (50)]
        public string Name { get; set; }

        [Required]
        [StringLength (10)]
        public string Code { get; set; }

        [Required]
        public int Price { get; set; }

        [Required]
        public int Gender { get; set; }

        [Required]
        public int Branch { get; set; }

        [Required]
        public int Category { get; set; }

        [Required]
        public int Status { get; set; }
        public int[] Color { get; set; }
        public int[] Size { get; set; }
        public int[] ShoeSize { get; set; }
        public int[] Collection { get; set; }
        public int[] Form { get; set; }
        public int[] Material { get; set; }
        public string Description { get; set; }
        public string Detail { get; set; }

        [Required]
        public IFormFile Image { get; set; }
        public List<IFormFile> DetailImage { get; set; }
    }
}