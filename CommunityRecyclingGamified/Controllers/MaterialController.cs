using CommunityRecyclingGamified.Dto;
using CommunityRecyclingGamified.Models;
using CommunityRecyclingGamified.Repositories;
using CommunityRecyclingGamified.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CommunityRecyclingGamified.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MaterialController : ControllerBase
    {
        private readonly IMaterialRepository _materialRepository;

        public MaterialController(IMaterialRepository materialRepository)
        {
            _materialRepository = materialRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Material>>> GetAllMaterials()
        {
            var material = await _materialRepository.GetAllAsync();
            return Ok(material);
        }

        [HttpGet("{id:int}")]
        //api/Material/{id}
        public async Task<ActionResult<Material>> GetMaterialById(int id)
        {
            var material = await _materialRepository.GetByIdAsync(id);


            if (material == null)
            {
                return NotFound();
            }

            return Ok(material);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<Material>> AddMaterial([FromBody] MaterialDto material)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var convertMaterialDtoToMaterial = new Material()
            {
                Name = material.Name,
                CreatedAt = DateTime.UtcNow,
                Description = material.Description,
                IsActive = material.IsActive,
                PointFactor = material.PointFactor,
                Unit = material.Unit
            };

            var isAdded = _materialRepository.AddMaterial(convertMaterialDtoToMaterial);

            if (!isAdded)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Could not save user");
            }

            return CreatedAtAction(nameof(GetMaterialById), new { id = convertMaterialDtoToMaterial.Id }, convertMaterialDtoToMaterial);
        }
        [Authorize(Roles = "Admin")]
        [HttpPut("{id:int}")]
        public async Task<ActionResult<Material>> UpdateMaterial([FromBody] MaterialDto material, int id)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var convertMaterialDtoToMaterial = new Material()
            {
                Name = material.Name,
                CreatedAt = DateTime.UtcNow,
                Description = material.Description,
                IsActive = material.IsActive,
                PointFactor = material.PointFactor,
                Unit = material.Unit
            };

            var isAdded = _materialRepository.UpdateAsync(convertMaterialDtoToMaterial, id);

            if (!isAdded)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Could not save user");
            }

            return CreatedAtAction(nameof(GetMaterialById), new { id = convertMaterialDtoToMaterial.Id }, convertMaterialDtoToMaterial);
        }
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:int}")]

        public async Task<IActionResult> DeleteMaterial(int id)
        {
            var deleted = await _materialRepository.Delete(id);

            if (!deleted)
                return NotFound();

            return NoContent();
        }

    }
}
