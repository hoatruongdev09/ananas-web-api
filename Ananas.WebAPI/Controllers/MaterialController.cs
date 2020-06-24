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
    public class MaterialController : Controller, IBasicController<MaterialModel> {
        private IModifiedLogger logger;
        private IConfiguration configuration;
        private IMaterialService materialService;

        public MaterialController (ILogger<MaterialController> logger, IConfiguration configuration) {
            this.logger = new ModifiedDebugger (logger);
            this.configuration = configuration;

            materialService = new MaterialService ();
            materialService.ConnectionString = this.configuration.GetConnectionString ((materialService as BaseService).ConnectionName);
        }

        [Route ("~/api/CreateMaterial")]
        [HttpPost]
        public async Task<ActionResult> Create ([FromForm] MaterialModel model) {
            try {
                int id = await materialService.Add (model);
                return Ok (id);
            } catch (Exception e) {
                return BadRequest (e);
            }
        }

        [Route ("~/api/DeleteMaterial")]
        [HttpPost]
        public async Task<ActionResult> Delete (int id) {
            try {
                int rowAffect = await materialService.Delete (id);
                if (rowAffect == 0) {
                    return NotFound ("Material not found");
                } else {
                    return Ok (rowAffect);
                }
            } catch (Exception e) {
                return BadRequest (e);
            }
        }

        [Route ("~/api/GetMaterial")]
        [HttpPost ("{id}")]
        public async Task<ActionResult> Get (int id) {
            try {
                MaterialModel material = await materialService.Get (id);
                if (material == null) {
                    return NotFound ("Material not found");
                } else {
                    return Ok (material);
                }
            } catch (Exception e) {
                return BadRequest (e);
            }
        }

        [Route ("~/api/GetListMaterial")]
        [HttpGet]
        public async Task<ActionResult> GetList () {
            try {
                List<MaterialModel> materialModels = await materialService.GetListAll ();
                return Ok (materialModels);
            } catch (Exception e) {
                return BadRequest (e);
            }
        }

        [Route ("~/api/UpdateMaterial")]
        [HttpPost]
        public async Task<ActionResult> Update ([FromForm] MaterialModel model) {
            try {
                int rowAffect = await materialService.Update (model);
                if (rowAffect == 0) {
                    return NotFound ("Material not found");
                } else {
                    return Ok (rowAffect);
                }
            } catch (Exception e) {
                return BadRequest (e);
            }
        }
    }

}