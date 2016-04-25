using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JiraIntegration.Entities
{
    public class BaselineItem
    {
        public string Title { get; set; }
        public string Link { get; set; }
        public string Project { get; set; }
        public string Key { get; set; }
        public string Summary { get; set; }
        public string Description { get; set; }
        public string StatusId { get; set; }
        public string Status { get; set; }
        public string Resolution { get; set; }
        public string Epic { get; set; }
        public string sprint { get; set; }
        public string StoryPoints { get; set; }
        public string AggregateTimeSpent { get; set; }
        public string IssueType { get; set; }
        public long SnapShotId { get; set; }
        [Key]
        public string Guid { get; set; }
    }

    public class Item
    {
        public string Title { get; set; }
        public string Link { get; set; }
        public string Project { get; set; }
        public string Key { get; set; }
        public string Summary { get; set; }
        public string Description { get; set; }
        public string StatusId { get; set; }
        public string Status { get; set; }
        public string Resolution { get; set; }
        public string Epic { get; set; }
        public string sprint { get; set; }
        public string StoryPoints { get; set; }
        public string AggregateTimeSpent { get; set; }
        public string IssueType { get; set; }
        public long SnapShotId { get; set; }
        [Key]
        public string Guid { get; set; }
    }
}