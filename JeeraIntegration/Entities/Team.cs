
using System.ComponentModel.DataAnnotations;
namespace JiraIntegration.Entities
{
    public class Team
    {
        [Key]
        public int TeamId { get; set; }

        [Required]
        [Display(Name = "Team Name")]
        [StringLength(50, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 3)]
        public string TeamName { get; set; }

        public bool IsStarTeam { get; set; }
    }
}
