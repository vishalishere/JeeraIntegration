using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace JiraIntegration.Entities
{
    public class MessageLog
    {
        [Key]
        public int MessageLogId { get; set; }

        [Required]
        [ForeignKey("StoryDependency")]
        public int StoryDependencyId { get; set; }
        public virtual StoryDependency StoryDependency { get; set; }

        public string Request { get; set; }

        public DateTime RequestTime { get; set; }

        public string Response { get; set; }

        public DateTime ResponseTime { get; set; }
    }
}