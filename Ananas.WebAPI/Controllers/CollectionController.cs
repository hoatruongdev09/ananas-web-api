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
    public class CollectionController : Controller, IBasicController<CollectionModel> {
        private IModifiedLogger logger;
        private IConfiguration configuration;
        private ICollectionService collectionService;

        public CollectionController (ILogger<CollectionController> logger, IConfiguration configuration) {
            this.logger = new ModifiedDebugger (logger);
            this.configuration = configuration;

            collectionService = new CollectionService ();
            collectionService.ConnectionString = this.configuration.GetConnectionString ((collectionService as BaseService).ConnectionName);
        }

        [Route ("~/api/CreateCollection")]
        [HttpPost]
        public async Task<ActionResult> Create ([FromForm] CollectionModel collection) {
            try {
                int id = await collectionService.Add (collection);
                if (id == -1) {
                    return BadRequest ("Collection not created");
                } else {
                    return Ok (id);
                }
            } catch (Exception e) {
                return BadRequest ($"{e.Message}");
            }

        }

        [Route ("~/api/DeleteCollection")]
        [HttpPost]
        public async Task<ActionResult> Delete (int id) {
            try {
                int rowAffect = await collectionService.Delete (id);
                if (id == -1) {
                    return BadRequest ("Collection not created");
                } else {
                    return Ok (rowAffect);
                }
            } catch (Exception e) {
                return BadRequest ($"{e.Message}");
            }
        }

        [Route ("~/api/GetCollection")]
        [HttpGet ("{id}")]
        public async Task<ActionResult> Get (int id) {
            try {
                CollectionModel collection = await collectionService.Get (id);
                if (collection == null) {
                    return NotFound ("Collection not found");
                } else {
                    return Ok (collection);
                }
            } catch (Exception e) {
                return BadRequest ($"{e.Message}");
            }
        }

        [Route ("~/api/GetListCollection")]
        [HttpGet]
        public async Task<ActionResult> GetList () {
            try {
                List<CollectionModel> collectionModels = await collectionService.GetListAll ();
                return Ok (collectionModels);
            } catch (Exception e) {
                return BadRequest ($"{e.Message}");
            }
        }

        [Route ("~/api/UpdateCollection")]
        [HttpPost]
        public async Task<ActionResult> Update ([FromForm] CollectionModel collection) {
            try {
                int rowAffect = await collectionService.Update (collection);
                if (rowAffect == 0) {
                    return NotFound ("Collection not found");
                } else {
                    return Ok (rowAffect);
                }
            } catch (Exception e) {
                return BadRequest ($"{e.Message}");
            }
        }
    }
}