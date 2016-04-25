using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JiraIntegration.Entities
{
    public class PlannedPointsForPI
    {
        [Key]
        public string Guid { get; set; }
        public string BaselinePlannedPointsForPI { get; set; }
        public string TotalStoriesWithoutEstimates { get; set; }
        public string TotalStories { get; set; }
        public string TotalDefects { get; set; }
        public string CurrentTotalPoints { get; set; }
        public string TotalClosedPoints { get; set; }
        public string SnapshotDate { get; set; }
        public string SnapshotNumber { get; set; }
    }
}
