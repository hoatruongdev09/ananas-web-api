using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ananas.Data.Models;
using Ananas.Services;
using Ananas.Services.Interfaces;
using Ananas.Services.PostgreServices;
using Ananas.Utility.Logger;
using Ananas.Web.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Ananas.Web.Controllers {
    [Route ("/api/[controller]")]
    public class ProductFormController : Controller, IBasicController<ProductFormModel> {
        private ModifiedDebugger logger;
        private IConfiguration configuration;

        private IProductFormService formService;

        public ProductFormController (ILogger<ProductFormController> logger, IConfiguration configuration) {
            this.logger = new ModifiedDebugger (logger);
            this.configuration = configuration;

            formService = new ProductFormService ();
            formService.ConnectionString = this.configuration.GetConnectionString ((formService as BaseService).ConnectionName);
        }

        [Route ("~/api/CreateForm")]
        [HttpPost]
        public async Task<ActionResult> Create ([FromForm] ProductFormModel model) {
            try {
                int id = await formService.Add (model);
                return Ok (id);
            } catch (Exception e) {
                return BadRequest (e.Message);
            }
        }

        [Route ("~/api/DeleteForm")]
        [HttpPost]
        public async Task<ActionResult> Delete (int id) {
            try {
                int rowAffect = await formService.Delete (id);
                if (rowAffect == 0) {
                    return NotFound ("Form not found");
                } else {
                    return Ok (rowAffect);
                }
            } catch (Exception e) {
                return BadRequest (e.Message);
            }
        }

        [Route ("~/api/GetForm")]
        [HttpGet ("{id}")]
        public async Task<ActionResult> Get (int id) {
            try {
                ProductFormModel form = await formService.Get (id);
                if (form == null) {
                    return NotFound ("Form not found");
                } else {
                    return Ok (form);
                }
            } catch (Exception e) {
                return BadRequest (e.Message);
            }
        }

        [Route ("~/api/GetListForm")]
        [HttpGet]
        public async Task<ActionResult> GetList () {
            try {
                List<ProductFormModel> listForm = await formService.GetListAll ();
                return Ok (listForm);
            } catch (Exception e) {
                return BadRequest (e.Message);
            }
        }

        [Route ("~/api/UpdateForm")]
        [HttpPost]
        public async Task<ActionResult> Update ([FromForm] ProductFormModel model) {
            try {
                int rowAffect = await formService.Update (model);
                if (rowAffect == 0) {
                    return NotFound ("Form not found");
                } else {
                    return Ok (rowAffect);
                }
            } catch (Exception e) {
                return BadRequest ($"{e.Message}");
            }
        }
    }
}