using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ananas.Data.Models;
using Ananas.Services;
using Ananas.Services.Interfaces;
using Ananas.Services.PostgreServices;
using Ananas.Utility.ImageWriter.Classes;
using Ananas.Utility.ImageWriter.Helper;
using Ananas.Utility.ImageWriter.Interface;
using Ananas.Utility.Logger;
using Ananas.Web.Interfaces;
using Ananas.Web.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
namespace Ananas.Web.Controllers {
    [Route ("/api/[controller]")]
    public class ProductController : Controller, IBasicController<ProductModel> {
        private IModifiedLogger logger;
        private IConfiguration configuration;

        private IProductService productService;
        private ICategoryService categoryService;
        private ICollectionService collectionService;
        private IColorService colorService;
        private IProductFormService formService;
        private IMaterialService materialService;
        private ISizeService sizeService;
        private IShoeSizeService shoeSizeService;
        private IProductImageService productImageService;

        public ProductController (ILogger<ProductController> logger, IConfiguration configuration) {
            this.logger = new ModifiedDebugger (logger);
            this.configuration = configuration;
            InitializeServices ();
        }
        private void InitializeServices () {

            productService = new ProductService ();
            string connectionString = this.configuration.GetConnectionString ((productService as BaseService).ConnectionName);
            productService.ConnectionString = connectionString;
            categoryService = new CategoryService (connectionString);
            collectionService = new CollectionService (connectionString);
            colorService = new ColorService (connectionString);
            formService = new ProductFormService (connectionString);
            materialService = new MaterialService (connectionString);
            sizeService = new SizeService (connectionString);
            shoeSizeService = new ShoeSizeService (connectionString);
            productImageService = new ProductImageService (connectionString);
        }

        [Route ("~/api/CreateProduct")]
        [HttpPost]
        public async Task<ActionResult> Create ([FromForm] ProductFigure model) {
            ProductModel product = new ProductModel () {
                ID = -1,
                Name = model.Name,
                Branch = model.Branch,
                Code = model.Code,
                Description = model.Description,
                Infomation = model.Infomation,
                Price = model.Price,
                Status = model.Status,
                Gender = model.Gender
            };
            try {
                IImageWriter imageWriter = new ImageWriter ();
                product.Image = await imageWriter.UploadImage (model.ImageProduct, "Uploads");
                int id = await productService.Add (product);
                await categoryService.CreateProductCategory (new ProductCategoryModel () { IDProduct = id, IDCategories = model.Category });
                await collectionService.CreateProductCollection (new ProductCollectionModel () { IDProduct = id, IDCollections = model.Collection });
                await colorService.CreateProductColor (new ProductColorModel () { IDProduct = id, IDColor = model.Color });
                await formService.CreateProductForms (new ProductFormsModel () { IDProduct = id, IDForms = model.Form });
                await materialService.CreateProductMaterial (new ProductMaterialModel () { IDProduct = id, IDMaterials = model.Material });
                await sizeService.CreateProductSize (new ProductSizeModel () { IDProduct = id, IDSizes = model.Size });
                await shoeSizeService.CreateProductShoeSize (new ProductShoeSizeModel () { IDProduct = id, IDShoeSizes = model.ShoeSize });
                List<string> images = await UploadManyImage (model.DetailImages);
                await productImageService.CreateProductImage (new ProductImageModel () { IDProduct = id, Image = images.ToArray () });
                return Ok (id);
            } catch (Exception e) {
                return BadRequest (e);
            }
        }

        public Task<ActionResult> Create ([FromForm] ProductModel model) {
            throw new NotImplementedException ();
        }

        [Route ("~/api/DeleteProduct")]
        [HttpPost]
        public async Task<ActionResult> Delete (int id) {
            try {
                ProductModel product = await productService.Get (id);
                if (product == null) {
                    return NotFound ("Product not found");
                }

                int rowAffect = await productService.Delete (id);
                await categoryService.DeleteCategoriesByProductId (id);
                await collectionService.DeleteCollectionsByProductId (id);
                await colorService.DeleteColorsByProductId (id);
                await formService.DeleteFormsByProductId (id);
                await materialService.DeleteMaterialsByProductId (id);
                await sizeService.DeleteSizesByProductId (id);
                await shoeSizeService.DeleteProductShoeSizeByProductId (id);

                IImageWriter imageWriter = new ImageWriter ();
                await imageWriter.RemoveImage (product.Image, "Uploads");
                List<string> productImages = await productImageService.GetImageByProductId (id);
                await RemoveManyImage (productImages);
                await productImageService.RemoveImageByProductId (id);

                return Ok (rowAffect);
            } catch (Exception e) {
                return BadRequest (e);
            }
        }

        [Route ("~/api/GetProduct")]
        [HttpGet ("{id}")]
        public async Task<ActionResult> Get (int id) {
            try {
                var model = await productService.Get (id);
                if (model == null) {
                    return NotFound ("Product not found");
                } else {
                    return Ok (model);
                }
            } catch (Exception e) {
                return BadRequest (e);
            }
        }

        [Route ("~/api/GetDetailProduct")]
        [HttpGet ("{id}")]
        public async Task<ActionResult> GetDetailProduct (int id) {
            try {
                var model = await productService.Get (id);
                if (model == null) {
                    return NotFound ("Product not found");
                }
                ProductDetail detail = new ProductDetail ();
                detail.Product = model;
                detail.Categories = (await categoryService.GetCategoriesByProductID (id)).ToArray ();
                detail.Collections = (await collectionService.GetCollectionsByProductID (id)).ToArray ();
                detail.Colors = (await colorService.GetColorByProductID (id)).ToArray ();
                detail.Forms = (await formService.GetFormByProductID (id)).ToArray ();
                detail.Materials = (await materialService.GetMaterialsByProductID (id)).ToArray ();
                detail.ShoeSizes = (await shoeSizeService.GetShoeSizesByProductID (id)).ToArray ();
                detail.Sizes = (await sizeService.GetSizesByProductID (id)).ToArray ();
                detail.DetailImages = (await productImageService.GetImageByProductId (id)).ToArray ();

                return Ok (detail);
            } catch (Exception e) {
                return BadRequest (e);
            }
        }

        [Route ("~/api/GetListProduct")]
        [HttpGet]
        public async Task<ActionResult> GetList () {
            try {
                var listModel = await productService.GetList ();
                return Ok (listModel);
            } catch (Exception e) {
                return BadRequest (e);
            }
        }

        [Route ("~/api/UpdateProduct")]
        [HttpPost]
        public async Task<ActionResult> Update ([FromForm] ProductFigure model) {
            try {
                var product = await productService.Get (model.ID);
                if (product == null) {
                    return NotFound ("Product not found");
                }
                product.Name = model.Name;
                product.Branch = model.Branch;
                product.Code = model.Code;
                product.Description = model.Description;
                product.Infomation = model.Infomation;
                product.Price = model.Price;
                product.Status = model.Status;
                product.Gender = model.Gender;

                if (model.Category.Length != 0) {
                    await categoryService.DeleteCategoriesByProductId (model.ID);
                    await categoryService.CreateProductCategory (new ProductCategoryModel () { IDProduct = model.ID, IDCategories = model.Category });
                }
                if (model.Collection.Length != 0) {
                    await collectionService.DeleteCollectionsByProductId (model.ID);
                    await collectionService.CreateProductCollection (new ProductCollectionModel () { IDProduct = model.ID, IDCollections = model.Collection });
                }
                if (model.Color.Length != 0) {
                    await colorService.DeleteColorsByProductId (model.ID);
                    await colorService.CreateProductColor (new ProductColorModel () { IDProduct = model.ID, IDColor = model.Color });
                }
                if (model.Form.Length != 0) {
                    await formService.DeleteFormsByProductId (model.ID);
                    await formService.CreateProductForms (new ProductFormsModel () { IDProduct = model.ID, IDForms = model.Form });
                }
                if (model.Material.Length != 0) {
                    await materialService.DeleteMaterialsByProductId (model.ID);
                    await materialService.CreateProductMaterial (new ProductMaterialModel () { IDProduct = model.ID, IDMaterials = model.Material });
                }
                if (model.Size.Length != 0) {
                    await sizeService.DeleteSizesByProductId (model.ID);
                    await sizeService.CreateProductSize (new ProductSizeModel () { IDProduct = model.ID, IDSizes = model.Size });
                }
                if (model.ShoeSize.Length != 0) {
                    await shoeSizeService.DeleteProductShoeSizeByProductId (model.ID);
                    await shoeSizeService.CreateProductShoeSize (new ProductShoeSizeModel () { IDProduct = model.ID, IDShoeSizes = model.ShoeSize });
                }
                if (model.DetailImages.Count != 0) {
                    List<string> images = await productImageService.GetImageByProductId (model.ID);
                    await RemoveManyImage (images);
                    await productImageService.RemoveImageByProductId (model.ID);
                    await productImageService.CreateProductImage (new ProductImageModel () { IDProduct = model.ID, Image = (await UploadManyImage (model.DetailImages)).ToArray () });
                }
                if (model.ImageProduct != null) {
                    IImageWriter imageWriter = new ImageWriter ();
                    await imageWriter.RemoveImage (product.Image, "Uploads");
                    product.Image = await imageWriter.UploadImage (model.ImageProduct, "Uploads");
                }
                int result = await productService.Update (product);
                return Ok (result);
            } catch (Exception e) {
                return BadRequest (e);
            }
        }

        public Task<ActionResult> Update ([FromForm] ProductModel model) {
            throw new NotImplementedException ();
        }

        public async Task<string> UploadImage (IFormFile image) {
            IImageWriter imageWriter = new ImageWriter ();
            try {
                string imagePath = await imageWriter.UploadImage (image, "Uploads");
                return imagePath;
            } catch (Exception e) {
                throw e;
            }
        }

        private async Task<List<string>> UploadManyImage (List<IFormFile> images) {
            List<string> listImages = new List<string> ();
            IImageWriter imageWriter = new ImageWriter ();
            try {
                foreach (IFormFile image in images) {
                    listImages.Add (await UploadImage (image));
                }
            } catch (Exception e) {
                logger.Log ($"{e.Message}");
            }
            return listImages;
        }
        private async Task RemoveManyImage (List<string> images) {
            IImageWriter imageWriter = new ImageWriter ();
            foreach (var image in images) {
                await imageWriter.RemoveImage (image, "Uploads");
            }
        }
    }
}