using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JiraIntegration.Entities
{
    public class SnapShot
    {
        public long SnapShotID { get; set; }
        public DateTime SnapshotDate { get; set; }

        [Key]
        public string Guid { get; set; }
    }
}