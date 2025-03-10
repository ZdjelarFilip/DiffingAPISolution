using DiffingAPI.Models;
using DiffingAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace DiffingAPI.Controllers
{
    [ApiController]
    [Route("v1/diff/{id}")]
    public class DiffController : ControllerBase
    {
        private readonly IDiffService _diffService;

        public DiffController(IDiffService diffService)
        {
            _diffService = diffService;
        }

        [HttpPut("left")]
        public IActionResult UploadLeft(string id, [FromBody] DiffRequest request)
        {
            if (request?.Data == null) return BadRequest();
            _diffService.StoreLeft(id, request.Data);
            return Created($"v1/diff/{id}/left", null);
        }

        [HttpPut("right")]
        public IActionResult UploadRight(string id, [FromBody] DiffRequest request)
        {
            if (request?.Data == null) return BadRequest();
            _diffService.StoreRight(id, request.Data);
            return Created($"v1/diff/{id}/right", null);
        }

        [HttpGet]
        public IActionResult GetDiff(string id)
        {
            var result = _diffService.GetDiff(id);
            return result != null ? Ok(result) : NotFound();
        }
    }
}