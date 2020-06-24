using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Ananas.Data;
using Ananas.Data.Models;
using Ananas.Services;
using Ananas.Services.Interfaces;
using Ananas.Services.PostgreServices;
using Ananas.Utility;
using Ananas.Utility.ImageWriter.Classes;
using Ananas.Utility.ImageWriter.Interface;
using Ananas.Web.Models;
using Ananas.Web.Models.AdminModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Ananas.Web.Controllers {
    public class AdminController : Controller {

        private IProductService productService;
        private ICategoryService categoryService;
        private ICollectionService collectionService;
        private IColorService colorService;
        private IProductFormService formService;
        private IMaterialService materialService;
        private ISizeService sizeService;
        private IShoeSizeService shoeSizeService;
        private IProductImageService productImageService;
        private IBranchService branchService;
        private IGenderService genderService;
        private IStatusService statusService;

        public AdminController (IConfiguration configuration) {
            string connectionString = configuration.GetConnectionString ("PostgreSQL");

            productService = new ProductService (connectionString);
            categoryService = new CategoryService (connectionString);
            collectionService = new CollectionService (connectionString);
            colorService = new ColorService (connectionString);
            formService = new ProductFormService (connectionString);
            materialService = new MaterialService (connectionString);
            sizeService = new SizeService (connectionString);
            shoeSizeService = new ShoeSizeService (connectionString);
            productImageService = new ProductImageService (connectionString);
            branchService = new BranchService (connectionString);
            genderService = new GenderService (connectionString);
            statusService = new StatusService (connectionString);
        }

        public ActionResult Index () {
            return View ();
        }

        public async Task<ActionResult> ProductList (int pageIndex = 1, int pageSize = 20) {
            List<ProductModel> listProduct = await productService.GetList (pageIndex, pageSize);
            return View (listProduct);
        }

        [HttpGet]
        public async Task<ActionResult> CreateProduct (AdminCreateProductErrorModel createModel = null, AdminCreateProductErrorModel model = null) {
            List<CategoryModel> categories = await categoryService.GetListAll ();
            List<CollectionModel> collections = await collectionService.GetListAll ();
            List<GenderModel> genders = await genderService.GetListAll ();
            List<BranchModel> branches = await branchService.GetListAll ();
            List<ColorModel> colors = await colorService.GetListAll ();
            List<MaterialModel> materials = await materialService.GetListAll ();
            List<ProductFormModel> forms = await formService.GetListAll ();
            List<SizeModel> sizes = await sizeService.GetListAll ();
            List<ShoeSizeModel> shoeSizes = await shoeSizeService.GetListAll ();
            List<StatusModel> status = await statusService.GetListAll ();
            ViewBag.categories = categories;
            ViewBag.collections = collections;
            ViewBag.genders = genders;
            ViewBag.branches = branches;
            ViewBag.colors = colors;
            ViewBag.materials = materials;
            ViewBag.forms = forms;
            ViewBag.sizes = sizes;
            ViewBag.shoeSizes = shoeSizes;
            ViewBag.status = status;

            return View (model);
        }

        [HttpPost]
        public async Task<ActionResult> CreateProduct (AdminCreateProductModel model) {
            if (!ModelState.IsValid) {
                return await CreateProduct (model);
            }
            ProductModel product = new ProductModel () {
                ID = -1,
                Name = model.Name,
                Branch = model.Branch,
                Code = model.Code,
                Description = model.Description == null? "": model.Description,
                Infomation = model.Detail == null? "": model.Detail,
                Price = model.Price,
                Status = model.Status,
                Gender = model.Gender,
                Category = model.Category
            };
            AdminCreateProductErrorModel errors = new AdminCreateProductErrorModel ();
            IImageWriter imageWriter = new ImageWriter ();
            int idProduct = -1;
            try {
                product.Image = imageWriter.GenerateFileID (model.Image);
            } catch (Exception e) {
                errors.ImageError = true;
            }
            try {
                idProduct = await productService.Add (product);
            } catch (Exception e) {
                errors.NameError = true;
                errors.BranchError = true;
                errors.CodeError = true;
                errors.DescriptionError = true;
                errors.DetailError = true;
                errors.PriceError = true;
                errors.StatusError = true;
                errors.GenderError = true;
                errors.CategoryError = true;
            }
            try {
                await imageWriter.UploadImage (model.Image, product.Image, "Uploads");
            } catch (Exception e) {

            }
            try {
                if (model.Collection != null && model.Collection.Length != 0) {
                    await collectionService.CreateProductCollection (new ProductCollectionModel () { IDProduct = idProduct, IDCollections = model.Collection });
                }
            } catch (Exception e) { errors.CodeError = true; }
            try {
                if (model.Color != null && model.Color.Length != 0) {
                    await colorService.CreateProductColor (new ProductColorModel () { IDProduct = idProduct, IDColor = model.Color });
                }
            } catch (Exception e) { errors.CodeError = true; }
            try {
                if (model.Form != null && model.Form.Length != 0) {
                    await formService.CreateProductForms (new ProductFormsModel () { IDProduct = idProduct, IDForms = model.Form });
                }
            } catch (Exception e) { errors.FormError = true; }
            try {
                if (model.Material != null && model.Material.Length != 0) {
                    await materialService.CreateProductMaterial (new ProductMaterialModel () { IDProduct = idProduct, IDMaterials = model.Material });
                }
            } catch (Exception e) { errors.MaterialError = true; }
            try {
                if (model.Size != null && model.Size.Length != 0) {
                    await sizeService.CreateProductSize (new ProductSizeModel () { IDProduct = idProduct, IDSizes = model.Size });
                }
            } catch (Exception e) { errors.SizeError = true; }
            try {
                if (model.ShoeSize != null && model.ShoeSize.Length != 0) {
                    await shoeSizeService.CreateProductShoeSize (new ProductShoeSizeModel () { IDProduct = idProduct, IDShoeSizes = model.ShoeSize });
                }
            } catch (Exception e) { errors.ShoeSizeError = true; }
            try {
                if (model.DetailImage != null && model.Detail.Length != 0) {
                    List<string> images = await UploadManyImage (model.DetailImage);
                    await productImageService.CreateProductImage (new ProductImageModel () { IDProduct = idProduct, Image = images.ToArray () });
                }
            } catch (Exception e) { errors.DetailImageError = true; }
            return await CreateProduct (errors);
        }

        public async Task<ActionResult> CategoryList (int pageIndex = 1, int pageSize = 20) {
            List<CategoryModel> listCategory = await categoryService.GetList (pageIndex, pageSize);
            return View (listCategory);
        }
        public async Task<ActionResult> BranchList (int pageIndex = 1, int pageSize = 20) {
            List<BranchModel> listBranch = await branchService.GetList (pageIndex, pageSize);
            return View (listBranch);
        }
        public async Task<ActionResult> SizeList () {
            List<SizeModel> sizeList = await sizeService.GetListAll ();
            List<ShoeSizeModel> shoeSizeList = await shoeSizeService.GetListAll ();
            AdminSizeModel model = new AdminSizeModel () { SizeList = sizeList, ShoeSizeList = shoeSizeList };
            return View (model);
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
            foreach (IFormFile image in images) {
                try {
                    listImages.Add (await UploadImage (image));
                } catch (Exception e) {

                }
            }
            return listImages;
        }
        private async Task RemoveManyImage (List<string> images) {
            IImageWriter imageWriter = new ImageWriter ();
            foreach (var image in images) {
                await imageWriter.RemoveImage (image, "Uploads");
            }
        }

        #region AJAX CALL
        [HttpGet]
        public async Task<ActionResult<List<ColorModel>>> GetListColor () {
            return await colorService.GetListAll ();
        }

        [HttpGet]
        public async Task<ActionResult<List<CategoryModel>>> GetListCategory () {
            return await categoryService.GetListAll ();
        }

        [HttpGet]
        public async Task<ActionResult<List<BranchModel>>> GetListBranch () {
            return await branchService.GetListAll ();
        }

        [HttpGet]
        public async Task<ActionResult<List<GenderModel>>> GetListGender () {
            return await genderService.GetListAll ();
        }

        [HttpGet]
        public async Task<ActionResult<List<SizeModel>>> GetListSize () {
            return await sizeService.GetListAll ();
        }

        [HttpGet]
        public async Task<ActionResult<List<ShoeSizeModel>>> GetListShoeSize () {
            return await shoeSizeService.GetListAll ();
        }

        [HttpGet]
        public async Task<ActionResult<List<CollectionModel>>> GetListCollection () {
            return await collectionService.GetListAll ();
        }

        [HttpGet]
        public async Task<ActionResult<List<ProductFormModel>>> GetListForm () {
            return await formService.GetListAll ();
        }

        [HttpGet]
        public async Task<ActionResult<List<StatusModel>>> GetListStatus () {
            return await statusService.GetListAll ();
        }

        [HttpGet]
        public async Task<ActionResult<List<MaterialModel>>> GetListMaterial () {
            return await materialService.GetListAll ();
        }
        #endregion
    }
}