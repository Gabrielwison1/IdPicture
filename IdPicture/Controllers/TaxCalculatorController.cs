using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyId.Application.DTOs;
using MyId.Application.Interface;
using System.Security.Claims;

namespace IdPicture.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class TaxCalculatorController : ControllerBase
    {
        private readonly ITaxCalculatorService _service;

        public TaxCalculatorController(ITaxCalculatorService service) => _service = service;

        private string CurrentUserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        private bool IsAdmin => User.IsInRole("Admin");

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _service.GetAllAsync(CurrentUserId, IsAdmin));

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _service.GetByIdAsync(id, CurrentUserId, IsAdmin);
            return result == null ? NotFound("Record not found or access denied.") : Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateTaxCalculatorDto dto)
        {
            var result = await _service.CreateAsync(dto, CurrentUserId);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] CreateTaxCalculatorDto dto) =>
            await _service.UpdateAsync(id, dto, CurrentUserId, IsAdmin) ? NoContent() : NotFound("Record not found or access denied.");

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id) =>
            await _service.DeleteAsync(id, CurrentUserId, IsAdmin) ? NoContent() : NotFound("Record not found or access denied.");
    }
}