using CommunityRecyclingGamified.Dto;
using CommunityRecyclingGamified.Models;
using CommunityRecyclingGamified.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
        // GET: api/UserProfile
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserProfile>>> GetAll()
        {
            var userProfiles = await _userProfileRepository.GetAllAsync();
            return Ok(userProfiles);
        }

        [HttpGet("{id:int}")]
        //api/UserProfile/5
        public async Task<ActionResult<UserProfile>> GetById(int id)
        {
            var userProfile = await _userProfileRepository.GetByIdAsync(id);

            if (userProfile == null)
            {
                return NotFound();
            }

            return Ok(userProfile);
        }

        [HttpGet("{email}")]

        public async Task<ActionResult<UserProfile>> GetByEmail(string email)
        {
            var userProfile = await _userProfileRepository.GetByEmailAsync(email);

            if (userProfile == null)
            {
                return NotFound();
            }

            return Ok(userProfile);
        }

        [HttpPost]
        public async Task<ActionResult<UserProfile>> AddUserProfile([FromBody] UserProfileDto userProfile)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = new UserProfile
            {
                DisplayName = userProfile.DisplayName,
                Email = userProfile.Email,
                PasswordHash = userProfile.PasswordHash,
                NeighborhoodId = userProfile.NeighborhoodId,
                TotalPoints = userProfile.TotalPoints,
                Level = userProfile.Level
            };

            var saved = await _userProfileRepository.AddAsync(user);

            if (!saved)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Could not save user");
            }

            // 201 Created + Location header + σώμα
            return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
        }

        [HttpPut("{id:int}")]
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


    }
}
