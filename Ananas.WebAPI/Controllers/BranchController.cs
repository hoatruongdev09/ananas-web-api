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

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Ananas.Web.Controllers {
    [Route ("api/[controller]")]
    public class BranchController : Controller, IBasicController<BranchModel> {
        private IModifiedLogger logger;
        private IConfiguration configuration;

        private IBranchService branchService;
        public BranchController (ILogger<BranchController> logger, IConfiguration configuration) {
            this.configuration = configuration;
            this.logger = new ModifiedDebugger (logger);

            branchService = new BranchService ();
            branchService.ConnectionString = this.configuration.GetConnectionString ((branchService as BaseService).ConnectionName);

        }

        [Route ("~/api/CreateBranch")]
        [HttpPost]
        public async Task<ActionResult> Create ([FromForm] BranchModel branch) {
            int id = await branchService.Add (branch);
            if (id != -1) {
                return Ok (id);
            } else {
                return BadRequest ("Branch not created");
            }
        }

        [Route ("~/api/DeleteBranch")]
        [HttpPost]
        public async Task<ActionResult> Delete (int id) {
            int rowAffected = await branchService.Delete (id);
            if (rowAffected == 0) {
                return NotFound ("Branch not found");
            } else if (rowAffected == -1) {
                return BadRequest ("Branch not found");
            } else {
                return Ok (rowAffected);
            }
        }

        [Route ("~/api/GetBranch")]
        [HttpGet ("{id}")]
        public async Task<ActionResult> Get (int id) {
            BranchModel model = await branchService.Get (id);
            if (model == null) {
                return NotFound ("Branch not found");
            } else {
                return Ok (model);
            }
        }

        [Route ("~/api/GetListBranch")]
        [HttpGet]
        public async Task<ActionResult> GetList () {
            List<BranchModel> branchModels = await branchService.GetListAll ();
            return Ok (branchModels);

        }

        [Route ("~/api/UpdateBranch")]
        [HttpPost]
        public async Task<ActionResult> Update ([FromForm] BranchModel branch) {
            int rowAffected = await branchService.Update (branch);
            if (rowAffected == 0) {
                return NotFound ("Branch not found");
            } else if (rowAffected == -1) {
                return BadRequest ("Branch not updated");
            } else {
                return Ok (rowAffected);
            }
        }
    }
}