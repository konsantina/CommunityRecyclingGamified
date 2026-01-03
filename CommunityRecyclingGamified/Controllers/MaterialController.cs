using CommunityRecyclingGamified.Dto;
using CommunityRecyclingGamified.Models;
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

        /// <summary>
        /// Get all materials (public).
        /// </summary>
        [AllowAnonymous]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Material>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<Material>>> GetAllMaterials()
        {
            var materials = await _materialRepository.GetAllAsync();
            return Ok(materials);
        }

        /// <summary>
        /// Get material by id (public).
        /// </summary>
        [AllowAnonymous]
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(Material), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Material>> GetMaterialById(int id)
        {
            var material = await _materialRepository.GetByIdAsync(id);

            if (material == null)
                return NotFound();

            return Ok(material);
        }

        /// <summary>
        /// Add new material (Admin only).
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ProducesResponseType(typeof(Material), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Material>> AddMaterial([FromBody] MaterialDto material)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var entity = new Material
            {
                Name = material.Name,
                Description = material.Description,
                Unit = material.Unit,
                PointFactor = material.PointFactor,
                IsActive = material.IsActive,
                CreatedAt = DateTime.UtcNow
            };

            // Σημείωση: κρατάω τη δική σου υπογραφή (bool return).
            // Αν η AddMaterial είναι sync, άστο έτσι. Αν είναι async, κάν' το await.
            var isAdded = _materialRepository.AddMaterial(entity);

            if (!isAdded)
                return StatusCode(StatusCodes.Status500InternalServerError, "Could not save material");

            return CreatedAtAction(nameof(GetMaterialById), new { id = entity.Id }, entity);
        }

        /// <summary>
        /// Update material (Admin only).
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(Material), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Material>> UpdateMaterial([FromBody] MaterialDto material, int id)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var entity = new Material
            {
                Name = material.Name,
                Description = material.Description,
                Unit = material.Unit,
                PointFactor = material.PointFactor,
                IsActive = material.IsActive,
                CreatedAt = DateTime.UtcNow
            };

            // Σημείωση: η δική σου UpdateAsync επιστρέφει κάτι που το λες isAdded.
            // Το κρατάω, αλλά ιδανικά θα ήταν await Task<bool>.
            var updated = _materialRepository.UpdateAsync(entity, id);

            if (!updated)
                return NotFound(); // ή 500 αν εσύ το θεωρείς failure, αλλά NotFound είναι πιο σωστό.

            return Ok(entity);
        }

        /// <summary>
        /// Delete material (Admin only).
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteMaterial(int id)
        {
            var deleted = await _materialRepository.Delete(id);

            if (!deleted)
                return NotFound();

            return NoContent();
        }
    }
}
