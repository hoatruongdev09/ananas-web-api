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
using Ananas.Web.Models;
using Ananas.Web.Models.AdminModels;
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
        }

        public ActionResult Index () {
            return View ();
        }

        public async Task<ActionResult> ProductList (int pageIndex = 1, int pageSize = 20) {
            List<ProductModel> listProduct = await productService.GetList (pageIndex, pageSize);
            return View (listProduct);
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
    }
}