using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyId.Application.DTOs;
using MyId.Application.Interface;
using System.Security.Claims;

namespace IdPicture.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExperienceController : ControllerBase
    {
        private readonly IExperienceService _service;

        public ExperienceController(IExperienceService service) => _service = service;

        private string CurrentUserId => User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;

        // GET: api/Experience (Public Access - Anyone can view)
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetAll()
            => Ok(await _service.GetAllAsync(CurrentUserId, isAdmin: true));

        // GET: api/Experience/5 (Public Access - Anyone can view)
        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _service.GetByIdAsync(id, CurrentUserId, isAdmin: true);
            return result == null ? NotFound("Record not found.") : Ok(result);
        }

        // POST: api/Experience (Admin Only)
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateExperienceDto dto)
        {
            var result = await _service.CreateAsync(dto, CurrentUserId);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        // PUT: api/Experience/5 (Admin Only)
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] CreateExperienceDto dto)
            => await _service.UpdateAsync(id, dto, CurrentUserId, isAdmin: true)
                ? NoContent()
                : NotFound("Record not found.");

        // DELETE: api/Experience/5 (Admin Only)
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
            => await _service.DeleteAsync(id, CurrentUserId, isAdmin: true)
                ? NoContent()
                : NotFound("Record not found.");
    }
}