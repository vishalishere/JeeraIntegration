
using System.ComponentModel.DataAnnotations;
namespace JiraIntegration.Entities
{
    public class Sprint
    {
        [Key]
        public int SprintId { get; set; }
        
        [Required]
        [Display(Name = "Sprint Name")]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        public string SprintName { get; set; }
    }
}
