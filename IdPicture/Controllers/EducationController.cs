using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyId.Application.DTOs;
using MyId.Application.Interface;
using System.Security.Claims;

namespace IdPicture.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EducationController : ControllerBase
    {
        private readonly IEducationService _service;

        public EducationController(IEducationService service) => _service = service;

        private string CurrentUserId => User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;

        // GET: api/Education (Public Access - Anyone can view)
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetAll()
            => Ok(await _service.GetAllAsync(CurrentUserId, isAdmin: true));

        // GET: api/Education/5 (Public Access - Anyone can view)
        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _service.GetByIdAsync(id, CurrentUserId, isAdmin: true);
            return result == null ? NotFound("Record not found.") : Ok(result);
        }

        // POST: api/Education (Admin Only)
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateEducationDto dto)
        {
            var result = await _service.CreateAsync(dto, CurrentUserId);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        // PUT: api/Education/5 (Admin Only)
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] CreateEducationDto dto)
            => await _service.UpdateAsync(id, dto, CurrentUserId, isAdmin: true)
                ? NoContent()
                : NotFound("Record not found.");

        // DELETE: api/Education/5 (Admin Only)
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
            => await _service.DeleteAsync(id, CurrentUserId, isAdmin: true)
                ? NoContent()
                : NotFound("Record not found.");
    }
}