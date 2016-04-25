using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JiraIntegration.Entities
{
    public enum DependencyStatus
    {
        Requested = 1,
        Accepted = 2,
        Rejected = 3,
        Owned = 4
    }
    public enum SoSStatus
    {
        Open =1,
        Closed=2,
        OnTrack=3
    }

    public class StoryDependency
    {
        [Key]
        public int StoryDependencyId { get; set; }
        
        [Required]
        [ForeignKey("FromTeam")]
        [Display(Name = "From Team")]
        public int FromTeamId { get; set; }
        public virtual Team FromTeam { get; set; }
        
        [Required]
        //[ForeignKey("ToTeam")]
        [Display(Name = "To Team")]
        public int ToTeamId { get; set; }
        //public virtual Team ToTeam { get; set; }

        [Required]
        [Display(Name = "Description")]
        [StringLength(1000, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 20)]
        public string Description { get; set; }

        [Required]
        [Display(Name = "Detail")]
        [StringLength(1000, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 20)]
        public string Detail { get; set; }
        
        [Required]
        [ForeignKey("NeedBySprint")]
        [Display(Name = "Need By Sprint")]
        public int NeedBySprintId { get; set; }
        public virtual Sprint NeedBySprint { get; set; }
        
        [Required]
        [Display(Name = "Requested By User")]
        public string RequestedByUserId { get; set; }
        
        [Required]
        [Display(Name = "Blocked Story")]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 3)]
        public string BlockedStory { get; set; }

        [Display(Name = "Dependency Story")]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 3)]
        public string DependencyStory { get; set; }

        [Required]
        [Display(Name = "Status")]
        [EnumDataType(typeof(DependencyStatus))]
        public DependencyStatus Status { get; set; }

        [Display(Name = "RejectReason")]
        public string RejectReason { get; set; }

        [Display(Name = "Owned by")]
        public string OwnedBy { get; set; }

        public DateTime DateCreated { get; set; }
        public bool IsDeleted { get; set; }

        [Required]
        [Display(Name = "SoS Status")]
        [EnumDataType(typeof(SoSStatus))]
        public SoSStatus SoSStatus { get; set; }

        [Display(Name = "SoSComment")]
        public string SoSComment { get; set; }
    }
}
