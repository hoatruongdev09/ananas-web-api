using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Ananas.Data.Models;
using Ananas.Services;
using Ananas.Services.Interfaces;
using Ananas.Services.PostgreServices;
using Ananas.Web.Interfaces;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Ananas.Web.Controllers {
    [Route ("api/[controller]")]
    public class CategoryController : ControllerBase, IBasicController<CategoryModel> {
        private readonly ILogger logger;
        private readonly IConfiguration configuration;
        private ICategoryService categoryService;

        public CategoryController (ILogger<CategoryController> logger, IConfiguration configuration) {
            this.logger = logger;
            this.configuration = configuration;

            categoryService = new CategoryService ();
            categoryService.ConnectionString = this.configuration.GetConnectionString ((categoryService as BaseService).ConnectionName);
        }

        [Route ("~/api/GetListCategory")]
        [HttpGet]
        public async Task<ActionResult> GetList () {
            List<CategoryModel> categories = await categoryService.GetList ();
            return Ok (categories);
        }

        [Route ("~/api/GetCategory")]
        [HttpGet ("{id}")]
        public async Task<ActionResult> Get (int id) {
            CategoryModel category = await categoryService.Get (id);
            if (category != null) {
                return Ok (category);
            } else {
                HttpContext.Response.StatusCode = (int) HttpStatusCode.NotFound;
                return Content ("Category not found");
            }
        }

        [Route ("~/api/CreateCategory")]
        [HttpPost]
        public async Task<ActionResult> Create ([FromForm] CategoryModel model) {
            //logger.Log(LogLevel.Debug, "create category: ", category);
            Debug.WriteLine ($"create category: {model.Name}");
            int id = await categoryService.Add (model);
            if (id != -1) {
                return Ok (id);
            } else {
                HttpContext.Response.StatusCode = (int) HttpStatusCode.BadRequest;
                return Content ("Category not created");
            }
        }

        [Route ("~/api/DeleteCategory")]
        [HttpPost]
        public async Task<ActionResult> Delete (int id) {
            int rowEffect = await categoryService.Delete (id);
            if (rowEffect != -1) {
                return Ok (rowEffect);
            } else {
                HttpContext.Response.StatusCode = (int) HttpStatusCode.BadRequest;
                return Content ("Category not deleted");
            }
        }

        [Route ("~/api/UpdateCategory")]
        [HttpPost]
        public async Task<ActionResult> Update ([FromForm] CategoryModel model) {
            Debug.WriteLine ($"update category {model.ID} {model.Name}");
            int rowEffect = await categoryService.Update (model);
            if (rowEffect != -1) {
                return Ok (rowEffect);
            } else {
                HttpContext.Response.StatusCode = (int) HttpStatusCode.BadRequest;
                return Content ("Category not Updated");
            }
        }
    }
}