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
    public class GenderController : Controller, IBasicController<GenderModel> {
        private IModifiedLogger logger;
        private IConfiguration configuration;
        private IGenderService genderService;

        public GenderController (ILogger<GenderController> logger, IConfiguration configuration) {
            this.logger = new ModifiedDebugger (logger);
            genderService = new GenderService ();
            this.configuration = configuration;
            genderService.ConnectionString = this.configuration.GetConnectionString ((genderService as BaseService).ConnectionName);
        }

        [Route ("~/api/CreateGender")]
        [HttpPost]
        public async Task<ActionResult> Create ([FromForm] GenderModel gender) {
            try {
                int id = await genderService.Add (gender);
                return Ok (id);
            } catch (Exception e) {
                return BadRequest ($"{e.Message} {e.StackTrace}");
            }
        }

        [Route ("~/api/DeleteGender")]
        [HttpPost]
        public async Task<ActionResult> Delete (int id) {
            try {
                int rowAffect = await genderService.Delete (id);
                if (rowAffect == 0) {
                    return NotFound ("Gender not found");
                } else {
                    return Ok (rowAffect);
                }
            } catch (Exception e) {
                return BadRequest (e.Message);
            }
        }

        [Route ("~/api/GetGender")]
        [HttpGet ("{id}")]
        public async Task<ActionResult> Get (int id) {
            try {
                GenderModel gender = await genderService.Get (id);
                if (gender == null) {
                    return NotFound ("Gender not found");
                } else {
                    return Ok (gender);
                }
            } catch (Exception e) {
                return BadRequest (e.Message);
            }
        }

        [Route ("~/api/GetListGender")]
        [HttpGet]
        public async Task<ActionResult> GetList () {
            try {
                List<GenderModel> genderModels = await genderService.GetListAll ();
                return Ok (genderModels);
            } catch (Exception e) {
                return BadRequest (e.Message);
            }
        }

        [Route ("~/api/UpdateGender")]
        [HttpPost]
        public async Task<ActionResult> Update ([FromForm] GenderModel gender) {
            try {
                int rowAffect = await genderService.Update (gender);
                if (rowAffect == 0) {
                    return NotFound ("Gender not found");
                } else {
                    return Ok (rowAffect);
                }
            } catch (Exception e) {
                return BadRequest (e.Message);
            }
        }
    }
}