using System.Collections.Generic;
using System.Threading.Tasks;
using Ananas.Data.Models;
using Ananas.Services.Interfaces;
using Ananas.Services.PostgreServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Ananas.Web.Controllers {
    public class BranchController : Controller {

        private IBranchService branchService;
        public BranchController (IConfiguration configuration) {
            branchService = new BranchService (configuration.GetConnectionString ("PostgreSQL"));
        }

        [HttpGet]
        public async Task<List<BranchModel>> GetListBranch () {
            List<BranchModel> branches = new List<BranchModel> ();
            branches = await branchService.GetListAll ();
            return branches;
        }

    }
}