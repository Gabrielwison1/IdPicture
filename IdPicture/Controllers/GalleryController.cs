using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyId.Application.DTOs;
using MyId.Application.Interface;
using System.Security.Claims;

namespace IdPicture.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GalleryController : ControllerBase
    {
        private readonly IGalleryService _service;

        public GalleryController(IGalleryService service) => _service = service;

        private string CurrentUserId => User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;

        // GET: api/Gallery (Public Access - Anyone can view)
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetAll()
            => Ok(await _service.GetAllAsync(CurrentUserId, isAdmin: true));

        // GET: api/Gallery/5 (Public Access - Anyone can view)
        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _service.GetByIdAsync(id, CurrentUserId, isAdmin: true);
            return result == null ? NotFound("Record not found.") : Ok(result);
        }

        // POST: api/Gallery (Admin Only)
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Create([FromForm] CreateGalleryItemDto dto)
        {
            var result = await _service.CreateAsync(dto, CurrentUserId);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        // PUT: api/Gallery/5 (Admin Only)
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Update(int id, [FromForm] CreateGalleryItemDto dto)
            => await _service.UpdateAsync(id, dto, CurrentUserId, isAdmin: true)
                ? NoContent()
                : NotFound("Record not found.");

        // DELETE: api/Gallery/5 (Admin Only)
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
            => await _service.DeleteAsync(id, CurrentUserId, isAdmin: true)
                ? NoContent()
                : NotFound("Record not found.");
    }
}