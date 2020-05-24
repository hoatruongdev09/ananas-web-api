using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Ananas.Web.Interfaces {
    public interface IBasicController<T> {
        [HttpPost]
        Task<ActionResult> Create ([FromForm] T model);
        [HttpGet ("{id}")]
        Task<ActionResult> Get (int id);
        [HttpGet]
        Task<ActionResult> GetList ();
        [HttpPost]
        Task<ActionResult> Delete (int id);
        [HttpPost]
        Task<ActionResult> Update ([FromForm] T model);
    }
}