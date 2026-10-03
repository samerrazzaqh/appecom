using AutoMapper;
using Core.interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AppProject.API.Controllers
{

    public class BugController : BaseController
    {
        public BugController(IUnitOfWork work, IMapper mapper) : base(work, mapper)
        {
        }
        [HttpGet("not-found")]
        public async Task<ActionResult> GetNotFound()
        {
            var category = await work.CategoryRepositry.GetByIdAsync(100);
            if (category == null) return NotFound();
            return Ok(category);
        }

        [HttpGet("server-error")]
        public async Task<ActionResult> GetServerError()
        {
            var category = await work.CategoryRepositry.GetByIdAsync(100);
            category.Name = "";// 
            return Ok(category);
        }

        [HttpGet("bad-request/{Id}")]
        public async Task<ActionResult> GetBadRequest(int id)
        {
            return Ok();
        }

        [HttpGet("bad-request/")]
        public async Task<ActionResult> GetBadRequest()
        {
            return BadRequest();
        }


    }
}
