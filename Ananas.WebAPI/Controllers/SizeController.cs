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
    public class SizeController : Controller, IBasicController<SizeModel> {
        private IModifiedLogger logger;
        private IConfiguration configuration;
        private ISizeService sizeService;

        public SizeController (ILogger<SizeController> logger, IConfiguration configuration) {
            this.logger = new ModifiedDebugger (logger);
            this.configuration = configuration;
            sizeService = new SizeService ();
            sizeService.ConnectionString = this.configuration.GetConnectionString ((sizeService as BaseService).ConnectionName);
        }

        [Route ("~/api/CreateSize")]
        [HttpPost]
        public async Task<ActionResult> Create ([FromForm] SizeModel model) {
            try {
                int id = await sizeService.Add (model);
                return Ok (id);
            } catch (Exception e) {
                return BadRequest (e);
            }
        }

        [Route ("~/api/DeleteSize")]
        [HttpPost]
        public async Task<ActionResult> Delete (int id) {
            try {
                int rowAffect = await sizeService.Delete (id);
                if (rowAffect == 0) {
                    return NotFound ("Size not found");
                } else {
                    return Ok (rowAffect);
                }
            } catch (Exception e) {
                return BadRequest (e);
            }
        }

        [Route ("~/api/GetSize")]
        [HttpGet ("{id}")]
        public async Task<ActionResult> Get (int id) {
            try {
                SizeModel size = await sizeService.Get (id);
                if (size == null) {
                    return NotFound ("Size not found");
                } else {
                    return Ok (size);
                }
            } catch (Exception e) {
                return BadRequest (e);
            }
        }

        [Route ("~/api/GetListSize")]
        [HttpGet]
        public async Task<ActionResult> GetList () {
            try {
                List<SizeModel> sizeModels = await sizeService.GetListAll ();
                return Ok (sizeModels);
            } catch (Exception e) {
                return BadRequest (e);
            }
        }

        [Route ("~/api/UpdateSize")]
        public async Task<ActionResult> Update ([FromForm] SizeModel model) {
            try {
                int rowAffect = await sizeService.Update (model);
                if (rowAffect == 0) {
                    return NotFound ("Size not found");
                } else {
                    return Ok (rowAffect);
                }
            } catch (Exception e) {
                return BadRequest (e);
            }
        }
    }
}