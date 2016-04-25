using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JiraIntegration.Entities
{
    public class Features
    {
        [Key]
        public int FeatureId { get; set; }

        [Required]
        [Display(Name = "Team Name")]
        [StringLength(50, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 3)]
        public string FeatureName { get; set; }

        [Required]
        [ForeignKey("CompleteBySprint")]
        [Display(Name = "Complete By Sprint")]
        public int CompleteBySprintId { get; set; }
        public virtual Sprint CompleteBySprint { get; set; }

        [Required]
        [ForeignKey("FromTeam")]
        [Display(Name = "From Team")]
        public int FromTeamId { get; set; }
        public virtual Team FromTeam { get; set; }
    }
}
