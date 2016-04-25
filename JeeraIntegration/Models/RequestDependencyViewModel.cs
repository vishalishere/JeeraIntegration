using JiraIntegration.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace JiraIntegration.Models
{
    public class RequestDependencyViewModel
    {
        public int StoryDependencyId { get; set; }

        [Required]
        [Display(Name = "From Team")]
        public int FromTeamId { get; set; }
        public List<SelectListItem> FromTeamList { get; set; }

        [Required]
        [Display(Name = "To Team")]
        public int ToTeamId { get; set; }
        public List<SelectListItem> ToTeamList { get; set; }

        [Required]
        [Display(Name = "Summary")]
        [StringLength(1000, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 20)]
        public string Description { get; set; }


        [Required]
        [Display(Name = "Description")]
        [StringLength(1000, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 20)]
        public string Detail { get; set; }
        
        [Required]     
        [Display(Name = "Need By Sprint")]
        public int NeedBySprintId { get; set; }
        public List<SelectListItem> NeedBySprintList { get; set; }

        
        [Display(Name = "Requested By User")]
        public string RequestedByUserId { get; set; }

        [Required]
        [Display(Name = "Blocked Story")]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 3)]
        public string BlockedStory { get; set; }

        [Display(Name = "Dependency Story")]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 3)]
        public string DependencyStory { get; set; }

       
        [Display(Name = "Status")]
        public DependencyStatus Status { get; set; }

        [Display(Name = "RejectReason")]
        public string RejectReason { get; set; }

        [Display(Name = "Owned by")]
        public string OwnedBy { get; set; }

        [Display(Name = "SoS Status")]
        public SoSStatus SoSStatus { get; set; }

        [Display(Name = "SoSComment")]
        public string SoSComment { get; set; }

        public DateTime DateCreated { get; set; }
        public bool IsDeleted { get; set; }

        public List<RequestDependencyListModel> StoryDependencyList { get; set; }

    }

    public class RequestDependencyListModel
    {
        public int StoryDependencyId { get; set; }
        
        public string FromTeam { get; set; }
        public string ToTeam { get; set; }
        public bool IsToTeamStartTeam {get; set;}
        public string Description { get; set; }
        public string Detail { get; set; }
        public string NeedBySprint { get; set; }
        //public List<SelectListItem> NeedBySprintList { get; set; }
        public string RequestedByUser { get; set; }
        public string BlockedStory { get; set; }
        public string DependencyStory { get; set; }
        public DependencyStatus Status { get; set; }
        public string RejectReason { get; set; }
        public string OwnedBy { get; set; }
        public SoSStatus SoSStatus { get; set; }
        public string SoSComment { get; set; }

        public DateTime DateCreated { get; set; }
        public bool IsDeleted { get; set; }

    }
}
