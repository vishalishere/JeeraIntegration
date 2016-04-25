using JiraIntegration.Entities;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace JiraIntegration.Models
{
    public class TeamViewModel
    {
        public int TeamId { get; set; }

        [Required]
        [Display(Name = "Team Name")]
        [StringLength(50, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 3)]
        public string TeamName { get; set; }

        [Display(Name = "Is Star Team?")]
        public bool IsStarTeam { get; set; }

        public List<Team> Teames { get; set; }
    }
}
