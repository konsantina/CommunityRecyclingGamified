using CommunityRecyclingGamified.Dto;
using CommunityRecyclingGamified.Models;
using CommunityRecyclingGamified.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CommunityRecyclingGamified.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserProfileController : ControllerBase
    {
        private readonly IUserProfileRepository _userProfileRepository;

        public UserProfileController(IUserProfileRepository userProfileRepository)
        {
            _userProfileRepository = userProfileRepository;
        }

        /// <summary>
        /// Get all user profiles (Admin only).
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<UserProfile>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<IEnumerable<UserProfile>>> GetAll()
        {
            var userProfiles = await _userProfileRepository.GetAllAsync();
            return Ok(userProfiles);
        }

        /// <summary>
        /// Get user profile by id (Admin only).
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(UserProfile), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UserProfile>> GetById(int id)
        {
            var userProfile = await _userProfileRepository.GetByIdAsync(id);

            if (userProfile == null)
                return NotFound();

            return Ok(userProfile);
        }

       
        [Authorize(Roles = "Admin")]
        [HttpGet("by-email/{email}")]
        [ProducesResponseType(typeof(UserProfile), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UserProfile>> GetByEmail(string email)
        {
            var userProfile = await _userProfileRepository.GetByEmailAsync(email);

            if (userProfile == null)
                return NotFound();

            return Ok(userProfile);
        }

   
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ProducesResponseType(typeof(UserProfile), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<UserProfile>> AddUserProfile([FromBody] UserProfileDto userProfile)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = new UserProfile
            {
                DisplayName = userProfile.DisplayName,
                Email = userProfile.Email,
                PasswordHash = userProfile.PasswordHash,
                NeighborhoodId = userProfile.NeighborhoodId,
                TotalPoints = userProfile.TotalPoints,
                Level = userProfile.Level,
                CreatedAt = DateTime.UtcNow
            };

            var saved = await _userProfileRepository.AddAsync(user);

            if (!saved)
                return StatusCode(StatusCodes.Status500InternalServerError, "Could not save user");

            return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
        }

      
        [Authorize(Roles = "Admin")]
        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(UserProfile), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UserProfile>> UpdateUserProfile(int id, [FromBody] UserProfileDto userProfile)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existing = await _userProfileRepository.GetByIdAsync(id);
            if (existing == null)
                return NotFound();

            existing.DisplayName = userProfile.DisplayName;
            existing.Email = userProfile.Email;
            existing.PasswordHash = userProfile.PasswordHash;
            existing.NeighborhoodId = userProfile.NeighborhoodId;
            existing.TotalPoints = userProfile.TotalPoints;
            existing.Level = userProfile.Level;

            _userProfileRepository.Update(existing);

            return Ok(existing);
        }

   
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteUserProfile(int id)
        {
            var deleted = await _userProfileRepository.Delete(id);

            if (!deleted)
                return NotFound();

            return NoContent();
        }
    }
}
