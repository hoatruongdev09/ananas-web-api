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
    public class StatusController : Controller, IBasicController<StatusModel> {
        private IModifiedLogger logger;
        private IConfiguration configuration;
        private IStatusService statusService;
        public StatusController (ILogger<StatusController> logger, IConfiguration configuration) {
            this.logger = new ModifiedDebugger (logger);
            this.configuration = configuration;

            statusService = new StatusService ();
            statusService.ConnectionString = this.configuration.GetConnectionString ((statusService as BaseService).ConnectionName);
        }

        [Route ("~/api/CreateStatus")]
        [HttpPost]
        public async Task<ActionResult> Create ([FromForm] StatusModel model) {
            try {
                int id = await statusService.Add (model);
                return Ok (id);
            } catch (Exception e) {
                return BadRequest (e);
            }
        }

        [Route ("~/api/DeleteStatus")]
        [HttpPost]
        public async Task<ActionResult> Delete (int id) {
            try {
                int rowAffect = await statusService.Delete (id);
                if (rowAffect == 0) {
                    return NotFound ("Status not found");
                } else {
                    return Ok (rowAffect);
                }
            } catch (Exception e) {
                return BadRequest (e);
            }
        }

        [Route ("~/api/GetStatus")]
        [HttpGet ("{id}")]
        public async Task<ActionResult> Get (int id) {
            try {
                var model = await statusService.Get (id);
                if (model == null) {
                    return NotFound ("Status not found");
                } else {
                    return Ok (model);
                }
            } catch (Exception e) {
                return BadRequest (e);
            }
        }

        [Route ("~/api/GetListStatus")]
        [HttpGet]
        public async Task<ActionResult> GetList () {
            try {
                var models = await statusService.GetListAll ();
                return Ok (models);
            } catch (Exception e) {
                return BadRequest (e);
            }
        }

        [Route ("~/api/UpdateStatus")]
        [HttpPost]
        public async Task<ActionResult> Update ([FromForm] StatusModel model) {
            try {
                int rowAffect = await statusService.Update (model);
                if (rowAffect == 0) {
                    return NotFound ("Status not found");
                } else {
                    return Ok (rowAffect);
                }
            } catch (Exception e) {
                return BadRequest (e);
            }
        }
    }
}