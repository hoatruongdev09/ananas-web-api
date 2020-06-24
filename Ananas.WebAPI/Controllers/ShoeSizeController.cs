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
    public class ShoeSizeController : Controller, IBasicController<ShoeSizeModel> {
        private IModifiedLogger logger;
        private IConfiguration configuration;
        private IShoeSizeService shoeSizeService;

        public ShoeSizeController (ILogger<ShoeSizeController> logger, IConfiguration configuration) {
            this.logger = new ModifiedDebugger (logger);
            this.configuration = configuration;

            shoeSizeService = new ShoeSizeService ();
            shoeSizeService.ConnectionString = this.configuration.GetConnectionString ((shoeSizeService as BaseService).ConnectionName);
        }

        [Route ("~/api/CreateShoeSize")]
        [HttpPost]
        public async Task<ActionResult> Create ([FromForm] ShoeSizeModel model) {
            try {
                int id = await shoeSizeService.Add (model);
                return Ok (id);
            } catch (Exception e) {
                return BadRequest (e);
            }
        }

        [Route ("~/api/DeleteShoeSize")]
        [HttpPost]
        public async Task<ActionResult> Delete (int id) {
            try {
                int rowAffect = await shoeSizeService.Delete (id);
                if (rowAffect == 0) {
                    return NotFound ("Shoe size not found");
                } else {
                    return Ok (rowAffect);
                }
            } catch (Exception e) {
                return BadRequest (e);
            }
        }

        [Route ("~/api/GetShoeSize")]
        [HttpGet ("{id}")]
        public async Task<ActionResult> Get (int id) {
            try {
                var model = await shoeSizeService.Get (id);
                if (model == null) {
                    return NotFound ("Shoe size not found");
                } else {
                    return Ok (model);
                }
            } catch (Exception e) {
                return BadRequest (e);
            }
        }

        [Route ("~/api/GetListShoeSize")]
        [HttpGet ()]
        public async Task<ActionResult> GetList () {
            try {
                var models = await shoeSizeService.GetListAll ();
                return Ok (models);
            } catch (Exception e) {
                return BadRequest (e);
            }
        }

        [Route ("~/api/UpdateShoeSize")]
        [HttpPost]
        public async Task<ActionResult> Update ([FromForm] ShoeSizeModel model) {
            try {
                int rowAffect = await shoeSizeService.Update (model);
                if (rowAffect == 0) {
                    return NotFound ("Shoe size not found");
                } else {
                    return Ok (rowAffect);
                }
            } catch (Exception e) {
                return BadRequest (e);
            }
        }
    }
}