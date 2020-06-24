using System.Collections.Generic;
using System.Threading.Tasks;
using Ananas.Data.Models;
using Ananas.Services.Interfaces;
using Ananas.Services.PostgreServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Ananas.Web.Controllers {
    public class ColorController : Controller {
        private IColorService colorService;

        public ColorController (IConfiguration configuration) {
            colorService = new ColorService (configuration.GetConnectionString ("PostgreSQL"));

        }

        [HttpGet]
        public async Task<ActionResult<List<ColorModel>>> GetListColor () {
            return await colorService.GetListAll ();
        }
    }
}