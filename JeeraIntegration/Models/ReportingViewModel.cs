using JiraIntegration.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using DotNet.Highcharts;

namespace JiraIntegration.Models
{
    public class ReportingViewModel
    {
        public List<ReportingItemListModel> SnapshotList { get; set; }
        public string BaselinePlannedPoints { get; set; }
        public DotNet.Highcharts.Highcharts chart { get; set; }

    }

    public class ReportingItemListModel
    {
        public string SnapShotNumber { get; set; }
        public string SnapshotDate { get; set; }
        public string TotalCurrentPoints { get; set; }
        public string TotalClosedPoints { get; set; }
        //public string TotalStroiesWithoutEstimates { get; set; }
    }
}
