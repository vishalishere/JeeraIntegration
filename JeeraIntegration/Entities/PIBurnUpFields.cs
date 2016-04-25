using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraIntegration.Entities
{
    class PIBurnUpFields
    {
        public DateTime date { get; set; }
        public string TotalPoints { get; set; }
        public string TotalClosedPoints { get; set; }
        public string TotalStories { get; set; }
        public string TotalDefects { get; set; }
        public string TotalStoriesWithoutEstimates { get; set; }
    }
}
