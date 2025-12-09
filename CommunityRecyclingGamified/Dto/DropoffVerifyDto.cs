
using System.ComponentModel.DataAnnotations;

namespace CommunityRecyclingGamified.Dto
{
    public class DropoffVerifyDto
    {
        [Required]
        public int VerifierUserId { get; set; }
    }
}
