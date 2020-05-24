using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Ananas.Data.Models;
using Ananas.Services;
using Ananas.Services.Interfaces;
using Ananas.Services.PostgreServices;
using Ananas.Utility.Logger;
using Ananas.Web.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Ananas.Web.Controllers {
    [Route ("api/[controller]")]
    public class ColorController : ControllerBase, IBasicController<ColorModel> {

        private readonly IModifiedLogger debugger;
        private readonly IConfiguration configuration;
        private IColorService colorService;

        public ColorController (ILogger<ColorController> logger, IConfiguration configuration) {
            this.debugger = new ModifiedDebugger (logger);
            this.configuration = configuration;

            colorService = new ColorService ();
            colorService.ConnectionString = this.configuration.GetConnectionString ((colorService as BaseService).ConnectionName);
        }

        [Route ("~/api/GetListColor")]
        [HttpGet]
        public async Task<ActionResult> GetList () {
            List<ColorModel> colorModels = await colorService.GetList ();
            return Ok (colorModels);
        }

        [Route ("~/api/GetColor")]
        [HttpGet ("{id}")]
        public async Task<ActionResult> Get (int id) {
            ColorModel color = await colorService.Get (id);
            if (color == null) {
                return NotFound ("Color not found");
            } else {
                return Ok (color);
            }
        }

        [Route ("~/api/CreateColor")]
        [HttpPost]
        public async Task<ActionResult> Create ([FromForm] ColorModel model) {
            debugger.Log ($"Create color: {model.Name} {model.Code}");
            int id = await colorService.Add (model);
            if (id != -1) {
                return Ok (id);
            } else {
                return BadRequest ("color cannot created");
            }
        }

        [Route ("~/api/DeleteColor")]
        [HttpPost]
        public async Task<ActionResult> Delete (int id) {
            int rowEffect = await colorService.Delete (id);
            if (rowEffect != -1) {
                return Ok (rowEffect);
            } else {
                return BadRequest ("Color not deleted");
            }
        }

        [Route ("~/api/UpdateColor")]
        [HttpPost]
        public async Task<ActionResult> Update ([FromForm] ColorModel color) {
            debugger.Log ($"color: {color.ID} {color.Name} {color.Code}");
            int rowEffect = await colorService.Update (color);
            if (rowEffect != -1) {
                return Ok (rowEffect);
            } else {
                return BadRequest ("Color not updated");
            }

        }
    }
}