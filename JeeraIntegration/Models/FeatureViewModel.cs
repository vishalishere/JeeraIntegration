using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace JiraIntegration.Models
{
    public class FeatureViewModel
    {
        public int FeatureId { get; set; }

        [Required]
        [Display(Name = "Feature Name")]
        [StringLength(50, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 3)]
        public string FeatureName { get; set; }

        [Required]
        [Display(Name = "From Team")]
        public int FromTeamId { get; set; }
        public List<SelectListItem> FromTeamList { get; set; }


        [Required]
        [Display(Name = "Complete By Sprint")]
        public int CompleteBySprintId { get; set; }
        public List<SelectListItem> CompleteBySprintList { get; set; }

        public List<FeatureListModel> FeaturesList { get; set; }
    }

    public class FeatureListModel
    {
        public int FeatureId { get; set; }
        public string FeatureName { get; set; }
        public string FromTeam { get; set; }
        public string CompleteBySprint { get; set; }
    }
}
