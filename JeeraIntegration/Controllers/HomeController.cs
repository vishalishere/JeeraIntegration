//extern alias Newtonsoft;
using JiraIntegration.Entities;
using JiraIntegration.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Xml.Linq;
using System.Globalization;
using System.Drawing;

using System.Net;
using System.Net.Http;
using Atlassian;
using Atlassian.Jira;
using Atlassian.Jira.Remote;
using Atlassian.Jira.Linq;
using System.Configuration;
using System.Collections.Specialized;

using RestSharp;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Utilities;
using System.Threading.Tasks;
using RestSharp.Authenticators;
using DotNet.Highcharts;
using DotNet.Highcharts.Options;
using DotNet.Highcharts.Helpers;
using DotNet.Highcharts.Enums;
using System.Reflection;

namespace JiraIntegration.Controllers
{
    public class HomeController : Controller
    {
        //private const string jiraRestApiUrl = "https://jira.ges.symantec.com/rest/api/2/";
        private const string jiraRestApiUrl = "https://jira-dev.ges.symantec.com/rest/api/2/";
        private const string jiraUsername = "Star_JiraAPI";
        private const string jiraPassword = "Symantec@1234";
        private const string JiraCreateIssueResource = "issue";
        private const string JiraLinkIssueResource = "issueLink";
        private const string JiraGetIssueDetail = "issue";
        private const string JiraFilterUrlEndPointPart1 = "/sr/jira.issueviews:searchrequest-xml/";
        private const string JiraFilterUrlEndPointPart2 = "?tempMax=1000&";

        private JiraDbContext context = new JiraDbContext();
        #region Basic Pages
        public ActionResult Index()
        {
            var s = context.Sprint.FirstOrDefault();
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "STAR Program Management Team.";

            return View();
        }
        #endregion

        #region Jira

        #region Dependency
        public ActionResult DependencyList()
        {
            List<Team> teamList = new List<Team>();
            teamList = context.Team.OrderBy(t => t.TeamId).ToList();
            List<SelectListItem> teames = teamList
                    .Select(s => new SelectListItem
                    {
                        Value = s.TeamId.ToString(),
                        Text = s.TeamName,
                        Selected = false
                    }).ToList();

            List<Sprint> sprintList = new List<Sprint>();
            sprintList = context.Sprint.OrderBy(t => t.SprintId).ToList();
            List<SelectListItem> sprints = sprintList
                    .Select(s => new SelectListItem
                    {
                        Value = s.SprintId.ToString(),
                        Text = s.SprintName,
                        Selected = false
                    }).ToList();

            RequestDependencyViewModel model = new RequestDependencyViewModel();
            model.FromTeamList = teames;
            model.ToTeamList = teames;
            model.NeedBySprintList = sprints;
            model.StoryDependencyList = GetStoryDepdencyList();
            return View(model);
        }

        public ActionResult CheckStoryKeyValidity(string storyKey)
        {
            bool bSuccess = false;
            try
            {
                var restClient = new RestClient(jiraRestApiUrl)
                {
                    Authenticator = new HttpBasicAuthenticator(jiraUsername, jiraPassword)
                };

                var checkStory = new RestRequest(JiraCreateIssueResource + "/" + storyKey, Method.GET);
                checkStory.Method = Method.GET;
                checkStory.AddHeader("Accept", "application/json");
                checkStory.Parameters.Clear();
                IRestResponse linkResponse = restClient.Execute(checkStory);
                //TODO: Fix the code to check on response code / status code and not rely on the JSON being returned, 
                //use the status code as documented in JIRA API https://docs.atlassian.com/jira/REST/latest/#d2e1037
                if (linkResponse != null)
                {
                    HttpStatusCode code = linkResponse.StatusCode;
                    if (linkResponse.StatusDescription.ToUpper() == "OK")
                        bSuccess = true;
                }

                if (bSuccess)
                    return Content(Boolean.TrueString);
                else
                    return Content(Boolean.FalseString);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                bSuccess = false;
                return Content(Boolean.FalseString);
            }
        }

        public ActionResult RequestDependency()
        {
            List<Team> teamList = new List<Team>();
            teamList = context.Team.OrderBy(t => t.TeamId).ToList();
            List<SelectListItem> teames = teamList
                    .Select(s => new SelectListItem
                    {
                        Value = s.TeamId.ToString(),
                        Text = s.TeamName,
                        Selected = false
                    }).ToList();

            List<Sprint> sprintList = new List<Sprint>();
            sprintList = context.Sprint.OrderBy(t => t.SprintId).ToList();
            List<SelectListItem> sprints = sprintList
                    .Select(s => new SelectListItem
                    {
                        Value = s.SprintId.ToString(),
                        Text = s.SprintName,
                        Selected = false
                    }).ToList();

            RequestDependencyViewModel model = new RequestDependencyViewModel();
            model.FromTeamList = teames;
            model.ToTeamList = teames;
            model.NeedBySprintList = sprints;
            model.StoryDependencyList = GetStoryDepdencyList();
            return View(model);
        }

        [HttpPost]
        public ActionResult RequestDependency(RequestDependencyViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.StoryDependencyId > 0) //Update
                {
                    UpdateDependency(model);
                }
                else
                {
                    StoryDependency storyDepdency = new StoryDependency();
                    storyDepdency.FromTeamId = model.FromTeamId;
                    storyDepdency.ToTeamId = model.ToTeamId;
                    storyDepdency.NeedBySprintId = model.NeedBySprintId;
                    storyDepdency.IsDeleted = false;
                    storyDepdency.RequestedByUserId = "1";
                    storyDepdency.DateCreated = DateTime.Now;
                    storyDepdency.Description = model.Description;
                    storyDepdency.Detail = model.Detail;
                    storyDepdency.BlockedStory = model.BlockedStory;
                    storyDepdency.DependencyStory = model.DependencyStory;
                    storyDepdency.Status = DependencyStatus.Requested;
                    storyDepdency.SoSStatus = SoSStatus.Open;
                    storyDepdency.SoSComment = "";
                    context.StoryDependency.Add(storyDepdency);
                    bool result = Convert.ToBoolean(context.SaveChanges());
                }

            }
            List<Team> teamList = new List<Team>();
            teamList = context.Team.OrderBy(t => t.TeamId).ToList();
            List<SelectListItem> teames = teamList
                    .Select(s => new SelectListItem
                    {
                        Value = s.TeamId.ToString(),
                        Text = s.TeamName,
                        Selected = false
                    }).ToList();

            List<Sprint> sprintList = new List<Sprint>();
            sprintList = context.Sprint.OrderBy(t => t.SprintId).ToList();
            List<SelectListItem> sprints = sprintList
                    .Select(s => new SelectListItem
                    {
                        Value = s.SprintId.ToString(),
                        Text = s.SprintName,
                        Selected = false
                    }).ToList();
            model.FromTeamList = teames;
            model.ToTeamList = teames;
            model.NeedBySprintList = sprints;
            model.StoryDependencyList = GetStoryDepdencyList();
            return View(model);
        }

        private List<SelectListItem> GetNeedBySprintList()
        {
            List<Sprint> sprintList = new List<Sprint>();
            sprintList = context.Sprint.OrderBy(t => t.SprintId).ToList();
            List<SelectListItem> sprints = sprintList
                    .Select(s => new SelectListItem
                    {
                        Value = s.SprintId.ToString(),
                        Text = s.SprintName,
                        Selected = false
                    }).ToList();
            return sprints;
        }

        [HttpPost]
        public ActionResult EditDependency(int storyDependencyId)
        {
            if (storyDependencyId > 0)
            {
                StoryDependency stDepdency = context.StoryDependency
                    .Where(s => s.StoryDependencyId == storyDependencyId
                        && s.IsDeleted == false).FirstOrDefault();
                if (stDepdency != null)
                {
                    return new JsonResult
                    {
                        Data = new
                        {
                            Success = true,
                            StoryDependencyId = storyDependencyId,
                            FromTeamId = stDepdency.FromTeamId,
                            ToTeamId = stDepdency.ToTeamId,
                            NeedBySprintId = stDepdency.NeedBySprintId,
                            Description = stDepdency.Description,
                            Detail = stDepdency.Detail,
                            BlockedStory = stDepdency.BlockedStory,
                            DependencyStory = stDepdency.DependencyStory,
                            RejectReason = stDepdency.RejectReason
                        }
                    };
                }
            }
            //code reaches here only if story dependency not found
            return Content(Boolean.FalseString);
        }

        [HttpPost]
        public ActionResult RejectDependency(int storyDependencyId, string Reason)
        {
            StoryDependency stDepdency = context.Set<StoryDependency>().Find(storyDependencyId);
            stDepdency.Status = DependencyStatus.Rejected;
            stDepdency.RejectReason = Reason;
            context.Set<StoryDependency>().Attach(stDepdency);
            context.Entry<StoryDependency>(stDepdency).Property(p => p.Status).IsModified = true;
            context.Entry<StoryDependency>(stDepdency).Property(p => p.RejectReason).IsModified = true;
            var result = context.SaveChanges();
            if (result == 1)
            {
                return Content(Boolean.TrueString);
            }
            else
            {
                return Content(Boolean.FalseString);
            }
        }

        [HttpPost]
        public ActionResult AcceptDependency(int storyDependencyId)
        {
            try
            {
                StoryDependency stDepdency = context.Set<StoryDependency>().Find(storyDependencyId);
                stDepdency.Status = DependencyStatus.Accepted;
                context.Set<StoryDependency>().Attach(stDepdency);
                context.Entry<StoryDependency>(stDepdency).Property(p => p.Status).IsModified = true;
                var result = context.SaveChanges();
                if (result == 1)
                {
                    string OutwardDependencyStoryKey = string.Empty;
                    string status = string.Empty;
                    bool bStoryLinked = CreateAndLinkStory(stDepdency.BlockedStory, stDepdency.Description, stDepdency.ToTeamId, stDepdency.Detail, stDepdency.NeedBySprintId, out OutwardDependencyStoryKey, out status);

                    if (bStoryLinked)
                    {
                        stDepdency.DependencyStory = OutwardDependencyStoryKey;
                        var SaveDepdency = context.SaveChanges();
                        //if (SaveDepdency==1)
                        //{

                        //}
                    }
                    return Content(Boolean.TrueString);
                }
                else
                {
                    return Content(Boolean.FalseString);
                }
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException e)
            {
                foreach (var validationErrors in e.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        Console.WriteLine("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
                    }
                }
                return Content(Boolean.FalseString);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return Content(Boolean.FalseString);
            }
        }

        [HttpPost]
        public ActionResult AcceptExistingDependency(int storyDependencyId, string dependencyStoryNumber)
        {
            bool bSuccess = false;
            StoryDependency stDepdency = context.Set<StoryDependency>().Find(storyDependencyId);
            stDepdency.Status = DependencyStatus.Accepted;
            stDepdency.DependencyStory = dependencyStoryNumber;
            context.Set<StoryDependency>().Attach(stDepdency);
            context.Entry<StoryDependency>(stDepdency).Property(p => p.Status).IsModified = true;
            context.Entry<StoryDependency>(stDepdency).Property(p => p.DependencyStory).IsModified = true;
            var result = context.SaveChanges();
            if (result == 1)
            {

                try
                {
                    var restClient = new RestClient(jiraRestApiUrl)
                    {
                        Authenticator = new HttpBasicAuthenticator(jiraUsername, jiraPassword)
                    };
                    bSuccess = LinkStories(stDepdency.BlockedStory, dependencyStoryNumber, bSuccess, restClient);
                    if (bSuccess)
                        return Content(Boolean.TrueString);
                    else
                        return Content(Boolean.FalseString);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    bSuccess = false;
                    return Content(Boolean.FalseString);
                }
            }
            else
            {
                return Content(Boolean.FalseString);
            }
        }

        public async Task<ActionResult> GetExtendedFields(string storyDependencyId, string BlockedStoryId)
        {
            bool bSuccess = false;
            string BlockedStorySprint = string.Empty;
            string BlockedStoryStatus = string.Empty;
            string DependencyStoryStatus = string.Empty;
            string DependencyStorySprint = string.Empty;

            var restClient = new RestClient(jiraRestApiUrl)
            {
                Authenticator = new HttpBasicAuthenticator(jiraUsername, jiraPassword)
            };

            if (!string.IsNullOrEmpty(BlockedStoryId))
            {
                var pathBlockedStory = String.Format(JiraGetIssueDetail + "/{0}", BlockedStoryId);
                IRestRequest restRequestBlockedStory = new RestRequest(pathBlockedStory, Method.GET);
                IRestResponse RestResponseBlockedStory = await restClient.ExecuteGetTaskAsync(restRequestBlockedStory);
                JObject objBlockedStory = RestResponseBlockedStory.ContentType.Contains("application/json") ? JObject.Parse(RestResponseBlockedStory.Content) : null;
                string BlockedStorySprintBigString = objBlockedStory["fields"]["customfield_10000"].ToString();
                int first = BlockedStorySprintBigString.IndexOf("15.");
                int last = BlockedStorySprintBigString.IndexOf(",startDate");
                BlockedStorySprint = BlockedStorySprintBigString.Substring(first, last - first);
                BlockedStoryStatus = objBlockedStory["fields"]["status"]["name"].ToString();
                bSuccess = true;
            }

            if (!string.IsNullOrEmpty(storyDependencyId))
            {
                var pathDependencyStory = String.Format(JiraGetIssueDetail + "/{0}", storyDependencyId);
                IRestRequest restRequestDependencyStory = new RestRequest(pathDependencyStory, Method.GET);
                IRestResponse RestResponseDependencyStory = await restClient.ExecuteGetTaskAsync(restRequestDependencyStory);
                JObject objDependencyStory = RestResponseDependencyStory.ContentType.Contains("application/json") ? JObject.Parse(RestResponseDependencyStory.Content) : null;
                string DependencyStorySprintBigString = (string)objDependencyStory["fields"]["customfield_10000"].ToString();
                int first = DependencyStorySprintBigString.IndexOf("15.");
                int last = DependencyStorySprintBigString.IndexOf(",startDate");
                DependencyStorySprint = DependencyStorySprintBigString.Substring(first, last - first);
                DependencyStoryStatus = (string)objDependencyStory["fields"]["status"]["name"].ToString();
                bSuccess = true;
            }

            if (bSuccess)
            {
                return new JsonResult
                {
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                    Data = new
                    {
                        Success = true,
                        StoryDependencyId = storyDependencyId,
                        DependencyStorySprint = DependencyStorySprint,
                        DependencyStoryStatus = DependencyStoryStatus,
                        BlockedStory = BlockedStoryId,
                        BlockedStorySprint = BlockedStorySprint,
                        BlockedStoryStatus = BlockedStoryStatus
                    }
                };
            }

            //code reaches here only if story dependency not found
            return Content(Boolean.FalseString);
        }

        private static T ParseEnum<T>(string value)
        {
            return (T)Enum.Parse(typeof(T), value, true);
        }

        [HttpPost]
        public ActionResult UpdateSoS(int storyDependencyId, string SoSstatus, string SoScomment)
        {
            StoryDependency stDepdency = context.Set<StoryDependency>().Find(storyDependencyId);
            if (!string.IsNullOrEmpty(SoSstatus))
            {
                stDepdency.SoSStatus = ParseEnum<SoSStatus>(SoSstatus);
                context.Entry<StoryDependency>(stDepdency).Property(p => p.SoSStatus).IsModified = true;
            }
            if (!string.IsNullOrEmpty(SoScomment))
            {
                stDepdency.SoSComment = SoScomment;
                context.Entry<StoryDependency>(stDepdency).Property(p => p.SoSComment).IsModified = true;
            }
            var result = context.SaveChanges();
            if (result == 1)
            {
                return Content(Boolean.TrueString);
            }
            else
            {
                return Content(Boolean.FalseString);
            }
        }

        [HttpPost]
        public ActionResult UpdateNeedBySprint(int storyDependencyId, int Sprint)
        {
            StoryDependency stDepdency = context.Set<StoryDependency>().Find(storyDependencyId);
            if (stDepdency != null && Sprint != 0)
            {
                stDepdency.NeedBySprintId = Sprint;
                context.Entry<StoryDependency>(stDepdency).Property(p => p.NeedBySprintId).IsModified = true;
            }
            var result = context.SaveChanges();
            if (result == 1)
            {
                return Content(Boolean.TrueString);
            }
            else
            {
                return Content(Boolean.FalseString);
            }
        }

        [HttpPost]
        public ActionResult OwnExistingDependency(int storyDependencyId, string OwnerName)
        {
            StoryDependency stDepdency = context.Set<StoryDependency>().Find(storyDependencyId);
            stDepdency.Status = DependencyStatus.Owned;
            stDepdency.OwnedBy = OwnerName;
            //stDepdency.DependencyStory = dependencyStoryNumber;
            context.Set<StoryDependency>().Attach(stDepdency);
            context.Entry<StoryDependency>(stDepdency).Property(p => p.Status).IsModified = true;
            context.Entry<StoryDependency>(stDepdency).Property(p => p.OwnedBy).IsModified = true;
            var result = context.SaveChanges();
            if (result == 1)
            {
                return Content(Boolean.TrueString);
            }
            else
            {
                return Content(Boolean.FalseString);
            }
        }

        [HttpPost]
        public ActionResult FilterDependency(int? fromTeamId, int? toTeamId, int? status, int? needBySprintId, int? SosStatus)
        {
            var stDepdency = GetStoryDepdencyList(fromTeamId, toTeamId, status, needBySprintId, SosStatus);
            if (Request.IsAjaxRequest())
            {
                if (stDepdency == null)
                {
                    return Content(Boolean.FalseString);
                }
                else
                {
                    return PartialView("_DependencyList", stDepdency);
                }
            }
            if (stDepdency == null)
            {
                return Content(Boolean.FalseString);
            }
            else
            {
                //json grid view
                return Content(Boolean.TrueString);
            }
        }

        #endregion

        #region MasterData

        //[Route("Manage/Sprint", Name = "AddSprint")]
        public ActionResult AddSprint()
        {
            return View();
        }


        //[Route("Manage/Team", Name = "AddTeam")]
        public ActionResult AddTeam()
        {
            TeamViewModel teams = new TeamViewModel();
            teams.Teames = new List<Team>();
            teams.Teames = context.Team.OrderBy(t => t.TeamId).ToList();
            return View(teams);

        }

        [HttpPost]
        public ActionResult AddTeam(TeamViewModel model)
        {
            if (ModelState.IsValid)
            {
                Team team = new Team();
                team.TeamName = model.TeamName;
                team.IsStarTeam = model.IsStarTeam;
                context.Team.Add(team);
                bool result = Convert.ToBoolean(context.SaveChanges());

            }
            model.Teames = context.Team.OrderBy(t => t.TeamId).ToList();
            return View("AddTeam", model);
        }

        [Route("Team/{teamId:int:min(1)}/Delete", Name = "DeleteTeam")]
        [HttpPost]
        public ActionResult DeleteTeam(int teamId)
        {
            try
            {
                Team mrec = context.Set<Team>().Find(teamId);
                context.Set<Team>().Remove(mrec);
                var result = context.SaveChanges();
                if (Convert.ToBoolean(result))
                {
                    return Content(Boolean.TrueString);
                }
                else
                {
                    return Content(Boolean.FalseString);
                }
            }
            catch (Exception ex)
            {
                //logger.Log(LogEventType.Error, "DeletedTeam", ex);
                return Content(Boolean.FalseString);
            }
        }

        #endregion

        #region Features
        public ActionResult AddFeature()
        {
            List<Team> teamList = new List<Team>();
            teamList = context.Team.OrderBy(t => t.TeamId).ToList();
            List<SelectListItem> teames = teamList
                    .Select(s => new SelectListItem
                    {
                        Value = s.TeamId.ToString(),
                        Text = s.TeamName,
                        Selected = false
                    }).ToList();

            List<Sprint> sprintList = new List<Sprint>();
            sprintList = context.Sprint.OrderBy(t => t.SprintId).ToList();
            List<SelectListItem> sprints = sprintList
                    .Select(s => new SelectListItem
                    {
                        Value = s.SprintId.ToString(),
                        Text = s.SprintName,
                        Selected = false
                    }).ToList();

            FeatureViewModel model = new FeatureViewModel();
            model.FromTeamList = teames;
            model.CompleteBySprintList = sprints;
            model.FeaturesList = GetFeatureList();
            return View("Feature", model);
        }

        [HttpPost]
        public ActionResult AddFeature(FeatureViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.FeatureId > 0) //Update
                {
                    UpdateFeature(model);
                }
                else
                {
                    Features feature = new Features();
                    feature.FromTeamId = model.FromTeamId;
                    feature.CompleteBySprintId = model.CompleteBySprintId;
                    feature.FeatureName = model.FeatureName;
                    context.Feature.Add(feature);
                    bool result = Convert.ToBoolean(context.SaveChanges());
                }

            }
            List<Team> teamList = new List<Team>();
            teamList = context.Team.OrderBy(t => t.TeamId).ToList();
            List<SelectListItem> teames = teamList
                    .Select(s => new SelectListItem
                    {
                        Value = s.TeamId.ToString(),
                        Text = s.TeamName,
                        Selected = false
                    }).ToList();

            List<Sprint> sprintList = new List<Sprint>();
            sprintList = context.Sprint.OrderBy(t => t.SprintId).ToList();
            List<SelectListItem> sprints = sprintList
                    .Select(s => new SelectListItem
                    {
                        Value = s.SprintId.ToString(),
                        Text = s.SprintName,
                        Selected = false
                    }).ToList();
            model.FromTeamList = teames;
            model.FromTeamList = teames;
            model.CompleteBySprintList = sprints;
            model.FeaturesList = GetFeatureList();
            return View("Feature", model);
        }

        [HttpPost]
        public ActionResult EditFeature(int featureId)
        {
            if (featureId > 0)
            {
                Features feature = context.Feature
                    .Where(s => s.FeatureId == featureId).FirstOrDefault();
                if (feature != null)
                {
                    return new JsonResult
                    {
                        Data = new
                        {
                            Success = true,
                            FeatureId = feature.FeatureId,
                            FromTeamId = feature.FromTeamId,
                            CompleteBySprintId = feature.CompleteBySprintId,
                            FeatureName = feature.FeatureName
                        }
                    };
                }
            }
            //code reaches here only if story dependency not found
            return Content(Boolean.FalseString);
        }

        [HttpPost]
        public ActionResult DeleteFeature(int featureId)
        {
            Features feature = context.Set<Features>().Find(featureId);
            context.Set<Features>().Remove(feature);
            var result = context.SaveChanges();
            if (result == 1)
            {
                return Content(Boolean.TrueString);
            }
            else
            {
                return Content(Boolean.FalseString);
            }
        }

        #endregion

        #region Reporting

        private Highcharts GetReleaseBurnUpChart(ReportingViewModel model)
        {
            TimeSpan interval = GetEndDateOfPI() - GetStartDateOfPI();
            string[] categories = GetXAxisCategories(model);
            object[] PlannedPointsForPI = GetBaseLine(model, categories.Length);// new[] { model.BaselinePlannedPoints }.Select(double.Parse).ToList().ToArray().Cast<object>().ToArray();
            object[] IdealBurn = IdealBurnLine(model, categories.Length);

            object[] totalCurrentPoints = GetTotalCurrentPointsLine(model, categories);
            //object[] totalCurrentPoints = model.SnapshotList.Select(x => x.TotalCurrentPoints).ToArray().Select(double.Parse).ToList().ToArray().Cast<object>().ToArray();
            object[] totalClosedPoints = GetTotalClosedPointsLine(model, categories);
            //object[] totalClosedPoints = model.SnapshotList.Select(x => x.TotalClosedPoints).ToArray().Select(double.Parse).ToList().ToArray().Cast<object>().ToArray();


            Highcharts chart = new Highcharts("chart")
            .InitChart(new Chart { ZoomType = ZoomTypes.Xy, DefaultSeriesType = ChartTypes.Line })
            .SetTitle(new Title { Text = "PI Burnup Chart" })
            .SetXAxis(new XAxis
            {
                Type = AxisTypes.Category,
                Categories = categories,
                Labels = new XAxisLabels
                {
                    Rotation = 0.5
                }
            })
            .SetYAxis(new YAxis
            {
                Title = new YAxisTitle { Text = "Story Points" },
                Min = 0,
            })
            .SetTooltip(new Tooltip { Enabled = true, Formatter = @"function() { return '<b>'+ this.series.name +'</b><br/>'+ this.x +': '+ this.y +' points'; }" })
            .SetPlotOptions(new PlotOptions
            {
                Line = new PlotOptionsLine
                {
                    DataLabels = new PlotOptionsLineDataLabels
                    {
                        Enabled = true
                    },
                    EnableMouseTracking = true
                }
            })
            .SetSeries(new[]
                       {
                           new Series {Name= "TotalCurrentPoints", Data = new Data(totalCurrentPoints), Type= ChartTypes.Line, Color=ColorTranslator.FromHtml("#1565C0") },
                           new Series {Name= "TotalClosedPoints", Data = new Data(totalClosedPoints), Type= ChartTypes.Line, Color=ColorTranslator.FromHtml("#00C853") },
                           new Series {Name="Baseline", Data = new Data(PlannedPointsForPI), PlotOptionsLine = new PlotOptionsLine { } },
                           new Series {Name="IdealBurn", Data = new Data (IdealBurn), PlotOptionsLine = new PlotOptionsLine {DataLabels = new PlotOptionsLineDataLabels {Enabled=true }, EnableMouseTracking=false, DashStyle= DashStyles.ShortDashDotDot,Color= Color.DimGray } }
                       });

            return chart;
        }


        [HttpGet]
        public ActionResult Reporting()
        {
            ReportingViewModel model = new ReportingViewModel();
            PlannedPointsForPI baseline = context.PlannedPointsForPI.FirstOrDefault();
            if (baseline != null)
            {
                model.BaselinePlannedPoints = baseline.BaselinePlannedPointsForPI;
                model.SnapshotList = GetSnapShotList();
                model.chart = GetReleaseBurnUpChart(model);
            }
            return View(model);
        }

        private object[] GetTotalClosedPointsLine(ReportingViewModel model, string[] XAxisDates)
        {
            string[] categories = model.SnapshotList.Select(x => x.SnapshotDate).ToList().OrderBy(x => DateTime.Parse(x)).Distinct().ToList().ToArray();
            CultureInfo culture = CultureInfo.CreateSpecificCulture("en-US");
            DateTimeFormatInfo dtfi = culture.DateTimeFormat;
            dtfi.DateSeparator = "-";
            string LastSnapShotDate = categories.Last();
            int intervals = GetSprintsInPI();
            int TotalClosedPointLineLength = Array.IndexOf(XAxisDates, LastSnapShotDate);
            int?[] TotalClosedPointsLine = new int?[TotalClosedPointLineLength + 1]; //this is because the index is 0 based

            foreach (ReportingItemListModel snapshot in model.SnapshotList)
            {
                int pos = Array.IndexOf(XAxisDates, snapshot.SnapshotDate);
                TotalClosedPointsLine[pos] = Convert.ToInt32(double.Parse(snapshot.TotalClosedPoints));
            }
            return TotalClosedPointsLine.ToList().ToArray().Cast<object>().ToArray();
        }
        private object[] GetTotalCurrentPointsLine(ReportingViewModel model, string[] XAxisDates)
        {
            string[] categories = model.SnapshotList.Select(x => x.SnapshotDate).ToList().OrderBy(x => DateTime.Parse(x)).Distinct().ToList().ToArray();
            CultureInfo culture = CultureInfo.CreateSpecificCulture("en-US");
            DateTimeFormatInfo dtfi = culture.DateTimeFormat;
            dtfi.DateSeparator = "-";
            string LastSnapShotDate = categories.Last();
            int intervals = GetSprintsInPI();
            int TotalCurrentPointLineLength = Array.IndexOf(XAxisDates, LastSnapShotDate);
            int?[] TotalCurrentPointsLine = new int?[TotalCurrentPointLineLength + 1]; //this is because the index is 0 based

            foreach (ReportingItemListModel snapshot in model.SnapshotList)
            {
                int pos = Array.IndexOf(XAxisDates, snapshot.SnapshotDate);
                TotalCurrentPointsLine[pos] = Convert.ToInt32(double.Parse(snapshot.TotalCurrentPoints));
            }
            return TotalCurrentPointsLine.ToList().ToArray().Cast<object>().ToArray();
        }
        private string[] GetXAxisCategories(ReportingViewModel model)
        {
            string[] categories = model.SnapshotList.Select(x => x.SnapshotDate).ToArray();
            CultureInfo culture = CultureInfo.CreateSpecificCulture("en-US");
            DateTimeFormatInfo dtfi = culture.DateTimeFormat;
            dtfi.DateSeparator = "-";
            int intervals = GetSprintsInPI();
            DateTime StartOfPI = GetStartDateOfPI();
            //DateTime EndOfPI = GetEndDateOfPI();
            //List<DateTime> datesInPI = GetDatesBetween(StartOfPI, EndOfPI);
            DateTime[] sprint_EndDates = new DateTime[(intervals * 2) + 1];
            //DateTime[] sprint_EndDates = new DateTime[datesInPI.Count];
            sprint_EndDates[0] = StartOfPI;
            //for (int i = 0; i < datesInPI.Count; i++)
            //{
            //    sprint_EndDates[i] = datesInPI[i];
            //}
            for (int i = 1; i <= intervals * 2; i++)
            {
                sprint_EndDates[i] = sprint_EndDates[i - 1].AddDays(7).Date;
            }
            string[] sprintEndDates = sprint_EndDates.Select(x => x.ToString("d", dtfi)).ToArray().Union(categories).ToList().OrderBy(x => DateTime.Parse(x)).ToList().ToArray();
            return sprintEndDates;
        }
        private object[] GetBaseLine(ReportingViewModel model, int NumberOfDataPoints)
        {
            PlannedPointsForPI baseline = context.PlannedPointsForPI.FirstOrDefault();
            string baselinePoints = baseline.BaselinePlannedPointsForPI;
            int BaselinePlannedPoints = Convert.ToInt32(double.Parse(baselinePoints));
            int intervals = GetSprintsInPI();
            int[] BaseLine = new int[NumberOfDataPoints];
            for (int i = 0; i < NumberOfDataPoints; i++)
            {
                BaseLine[i] = BaselinePlannedPoints;
            }
            return BaseLine.Cast<object>().ToArray();
        }
        private object[] IdealBurnLine(ReportingViewModel model, int NumberOfDataPoints)
        {

            PlannedPointsForPI baseline = context.PlannedPointsForPI.FirstOrDefault();
            string baselinePoints = baseline.BaselinePlannedPointsForPI;
            int BaselinePlannedPoints = Convert.ToInt32(double.Parse(baselinePoints));
            int intervals = GetSprintsInPI();

            //int idealBurnPerInterval = (BaselinePlannedPoints / (intervals * 2));
            //decimal RealidealBurnPerInterval = ((decimal)BaselinePlannedPoints / (decimal)NumberOfDataPoints);
            decimal RealidealBurnPerInterval = ((decimal)BaselinePlannedPoints / (intervals *2));
            int idealBurnPerInterval = (int)Math.Round(RealidealBurnPerInterval, 0, MidpointRounding.AwayFromZero);
            int[] IdealBurnLine = new int[NumberOfDataPoints];
            IdealBurnLine[0] = 0;
            IdealBurnLine[1] = idealBurnPerInterval;

            for (int i = 2; i < NumberOfDataPoints; i++)
            {
                if (i == NumberOfDataPoints - 1)
                {
                    IdealBurnLine[i] = IdealBurnLine[i - 1] + (BaselinePlannedPoints - IdealBurnLine[i - 1]);
                }
                else
                {
                    IdealBurnLine[i] = IdealBurnLine[i - 1] + idealBurnPerInterval;
                }
            }
            return IdealBurnLine.Cast<object>().ToArray();
        }

        private List<ReportingItemListModel> GetSnapShotList()
        {
            List<ReportingItemListModel> snapshotList = new List<ReportingItemListModel>();
            ReportingItemListModel model;
            var plannedpoints = context.PlannedPointsForPI.Join(
                context.Snapshot.AsEnumerable(),
                plannedpointsforPI => plannedpointsforPI.SnapshotNumber, Snapshot => Snapshot.SnapShotID.ToString(),
                (snapshot, plannedpointsforPI) => new
                {
                    CurrentTotalPoints = snapshot.CurrentTotalPoints,
                    SnapshotNumber = snapshot.SnapshotNumber,
                    TotalClosedPoints = snapshot.TotalClosedPoints,
                    SnapshotDate = snapshot.SnapshotDate.Replace("/", "-")
                }).ToList();
            //foreach(PlannedPointsForPI p in plannedpoints)
            //changed to var p because the entity was created as annoymous in the join query above
            foreach (var p in plannedpoints)
            {
                model = new ReportingItemListModel();
                model.SnapShotNumber = p.SnapshotNumber;
                model.TotalCurrentPoints = p.CurrentTotalPoints;
                model.TotalClosedPoints = p.TotalClosedPoints;
                model.SnapshotDate = p.SnapshotDate;
                snapshotList.Add(model);
            }
            return snapshotList.OrderBy(s => s.SnapshotDate).ToList();

            //////List<SnapShot> SnapShot = new List<SnapShot>();
            //////SnapShot = context.Snapshot.OrderBy(t => t.SnapShotID).ToList();

            //////var snapshot = from i in context.Item group i by i.SnapShotId into g
            //////               let closedstrories = g.h
            //////               select new
            //////               {
            //////                   snapshotnumber= g.Key,
            //////                   totalcurrentpoints = g.Sum(o => string.IsNullOrEmpty(o.StoryPoints) ? 0 : decimal.Parse(o.StoryPoints.ToString())),
            //////                   totalclosedpoints = g.Where(closedStatus => new[] { "closed", "Closed", "done", "Done" }.Contains(g))) AsQueryable().Sum(o => string.IsNullOrEmpty(o.StoryPoints) ? 0 : decimal.Parse(o.StoryPoints.ToString()))
            //////               }
            ////////var list = from l in context.Snapshot select;
            //////var closedStories = context.Item.Where(closedStatus => new[] { "closed", "Closed", "done", "Done" }.Contains(closedStatus.Status)).ToList();
            //////model.TotalClosedPoints = closedStories.AsEnumerable().Select(o => string.IsNullOrEmpty(o.StoryPoints) ? 0 : decimal.Parse(o.StoryPoints.ToString())).Sum().ToString();
            //////model.TotalStroiesWithoutEstimates = context.Item.AsEnumerable().Select(o => string.IsNullOrEmpty(o.StoryPoints)).Count().ToString();
            //////model.SnapshotDate = DateTime.Now.Date.ToShortDateString();
            ////////throw new NotImplementedException();
        }

        [HttpPost]
        public ActionResult TakeJiraSnapShot()
        {
            try
            {
                long sequence = context.Database.SqlQuery<long>("SELECT NEXT VALUE FOR Sequence_Snapshot").FirstOrDefault();
                XDocument xdoc = null;
                int StartPos = 0;
                if (Request.IsAjaxRequest())
                {
                    string JiraFilter = JiraFilterUrlBuilder();
                    try
                    {
                        xdoc = XDocument.Load(JiraFilter);
                    }
                    catch (System.Net.WebException e)
                    {
                        //todo: check for this error extract the code to separate method that takes the xmldoc and return dataofinterest type 
                        //that needs to be added to dataofinterest till you reach end of items
                        //"The remote server returned an error: (403) Forbidden."
                    }
                    //NameValueCollection dbfields = GetFieldsOfInterest();
                    var dataofInterest = (from item in xdoc.Elements("rss").Elements("channel").Elements("item")
                                          select new Item
                                          {
                                              SnapShotId = sequence,
                                              Guid = Guid.NewGuid().ToString(),
                                              Description = item.Element("description").Value ?? "",
                                              Title = item.Element("title").Value ?? "",
                                              Link = item.Element("link").Value,
                                              Project = item.Element("project").Value,
                                              Key = item.Element("key").Value,
                                              Summary = item.Element("summary").Value ?? "",
                                              IssueType = item.Element("type").Value ?? "",
                                              Status = item.Element("status").Value,
                                              Resolution = item.Element("resolution").Value,
                                              StatusId = item.Element("status").Attribute("id").Value,
                                              AggregateTimeSpent = item.Element("aggregatetimespent") != null ? item.Element("aggregatetimespent").Value ?? "" : string.Empty,
                                              //AggregateTimeSpent = item.Element("aggregatetimespent").Value ?? "",
                                              sprint = (from customField in item.Elements("customfields").Elements("customfield").Where(p => (string)p.Attribute("id") == "customfield_10000")
                                                            //where ((string)customField.Attribute("id") == "customfield_10000")
                                                        select customField.Element("customfieldvalues").Element("customfieldvalue").Value).FirstOrDefault() ?? "",

                                              StoryPoints = (from customField in item.Elements("customfields").Elements("customfield").Where(p => (string)p.Attribute("id") == "customfield_10008")
                                                                 //where ((string)customField.Attribute("id") == "customfield_10008")
                                                             select customField.Element("customfieldvalues").Element("customfieldvalue").Value).FirstOrDefault() ?? "",

                                              Epic = (from customField in item.Elements("customfields").Elements("customfield").Where(p => (string)p.Attribute("id") == "customfield_10001")
                                                          //where ((string)customField.Attribute("id") == "customfield_10001")
                                                      select customField.Element("customfieldvalues").Element("customfieldvalue").Value).FirstOrDefault() ?? ""
                                          }
                     ).ToList();
                    var currentTotalPoints = (from b in dataofInterest select string.IsNullOrEmpty(b.StoryPoints) ? 0 : decimal.Parse(b.StoryPoints.ToString())).Sum();
                    var totalStories = (from b in dataofInterest where (new string[] { "Story", "story", "Request", "request", "Incident", "incident" }).Contains(b.IssueType) select b).Count().ToString();
                    var totalDefects = (from b in dataofInterest where (new string[] { "Defect", "defect", "Story Defect", "story defect" }).Contains(b.IssueType) select b).Count().ToString();
                    var totalStoriesWithoutEstimates = (from b in dataofInterest select string.IsNullOrEmpty(b.StoryPoints)).Count();
                    var closedStories = from stories in dataofInterest where (new string[] { "closed", "Closed", "done", "Done" }).Contains(stories.Status) select stories;
                    var TotalClosedPoints = closedStories.AsEnumerable().Select(o => string.IsNullOrEmpty(o.StoryPoints) ? 0 : decimal.Parse(o.StoryPoints.ToString())).Sum().ToString();
                    PlannedPointsForPI BaselinePlannedPoints = new PlannedPointsForPI()
                    {
                        BaselinePlannedPointsForPI = context.PlannedPointsForPI.FirstOrDefault().BaselinePlannedPointsForPI.ToString(),
                        TotalStoriesWithoutEstimates = totalStoriesWithoutEstimates.ToString(),
                        CurrentTotalPoints = currentTotalPoints.ToString(),
                        TotalClosedPoints = TotalClosedPoints,
                        SnapshotDate = DateTime.Now.Date.ToShortDateString(),
                        SnapshotNumber = sequence.ToString(),
                        TotalStories = totalStories,
                        TotalDefects = totalDefects,
                        Guid = Guid.NewGuid().ToString()
                    };
                    SnapShot snap = new SnapShot()
                    {
                        Guid = Guid.NewGuid().ToString(),
                        SnapshotDate = DateTime.Now.Date,
                        SnapShotID = sequence
                    };
                    context.Snapshot.Add(snap);
                    context.PlannedPointsForPI.Add(BaselinePlannedPoints);

                    context.Item.AddRange(dataofInterest);

                    context.SaveChanges();

                    //Task changesSaved = context.SaveChangesAsync();
                    //changesSaved.RunSynchronously();
                    return Content(Boolean.TrueString);
                }
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException dbEx)
            {
                Exception raise = dbEx;
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        string message = string.Format("{0}:{1}",
                            validationErrors.Entry.Entity.ToString(),
                            validationError.ErrorMessage);
                        // raise a new exception nesting
                        // the current instance as InnerException
                        raise = new InvalidOperationException(message, raise);
                    }
                }
                throw raise;
            }
            return Content(Boolean.FalseString);
        }
        [HttpPost]
        public ActionResult CreateBaseLine()
        {
            if (Request.IsAjaxRequest())
            {
                //check if already baselined, if baselined simply return
                if (context.BaselineItem.FirstOrDefault() != null && (!string.IsNullOrEmpty(context.BaselineItem.FirstOrDefault().Guid.ToString())))
                {
                    return Content(bool.FalseString);
                }
                string JiraFilter = JiraFilterUrlBuilder();
                XDocument xdoc = XDocument.Load(JiraFilter);
                long sequence = context.Database.SqlQuery<long>("SELECT NEXT VALUE FOR Sequence_Snapshot").FirstOrDefault();
                //NameValueCollection dbfields = GetFieldsOfInterest();
                var dataofInterest = (from item in xdoc.Elements("rss").Elements("channel").Elements("item")
                                      select new BaselineItem
                                      {
                                          SnapShotId = sequence,
                                          Guid = Guid.NewGuid().ToString(),
                                          Description = item.Element("description").Value ?? "",
                                          Title = item.Element("title").Value ?? "",
                                          Link = item.Element("link").Value,
                                          Project = item.Element("project").Value,
                                          Key = item.Element("key").Value,
                                          Summary = item.Element("summary").Value ?? "",
                                          IssueType = item.Element("type").Value ?? "",
                                          Status = item.Element("status").Value,
                                          Resolution = item.Element("resolution").Value,
                                          StatusId = item.Element("status").Attribute("id").Value,
                                          AggregateTimeSpent = item.Element("aggregatetimespent") != null ? item.Element("aggregatetimespent").Value ?? "" : string.Empty,
                                          //AggregateTimeSpent = item.Element("aggregatetimespent").Value ?? "",
                                          sprint = (from customField in item.Elements("customfields").Elements("customfield").Where(p => (string)p.Attribute("id") == "customfield_10000")
                                                        //where ((string)customField.Attribute("id") == "customfield_10000")
                                                    select customField.Element("customfieldvalues").Element("customfieldvalue").Value).FirstOrDefault() ?? "",

                                          StoryPoints = (from customField in item.Elements("customfields").Elements("customfield").Where(p => (string)p.Attribute("id") == "customfield_10008")
                                                             //where ((string)customField.Attribute("id") == "customfield_10008")
                                                         select customField.Element("customfieldvalues").Element("customfieldvalue").Value).FirstOrDefault() ?? "",

                                          Epic = (from customField in item.Elements("customfields").Elements("customfield").Where(p => (string)p.Attribute("id") == "customfield_10001")
                                                      //where ((string)customField.Attribute("id") == "customfield_10001")
                                                  select customField.Element("customfieldvalues").Element("customfieldvalue").Value).FirstOrDefault() ?? ""
                                      }
                 ).ToList();
                var currentTotalPoints = (from b in dataofInterest select string.IsNullOrEmpty(b.StoryPoints) ? 0 : decimal.Parse(b.StoryPoints.ToString())).Sum();
                var totalStories = (from b in dataofInterest where (new string[] { "Story", "story", "Request", "request", "Incident", "incident" }).Contains(b.IssueType) select b).Count().ToString();
                var totalDefects = (from b in dataofInterest where (new string[] { "Defect", "defect", "Story Defect", "story defect" }).Contains(b.IssueType) select b).Count().ToString();
                var totalStoriesWithoutEstimates = (from b in dataofInterest select string.IsNullOrEmpty(b.StoryPoints)).Count();
                var closedStories = from stories in dataofInterest where (new string[] { "closed", "Closed", "done", "Done" }).Contains(stories.Status) select stories;
                var TotalClosedPoints = closedStories.AsEnumerable().Select(o => string.IsNullOrEmpty(o.StoryPoints) ? 0 : decimal.Parse(o.StoryPoints.ToString())).Sum().ToString();
                //var firstSnapshot = dataofInterest.Cast<Item>().ToList();
                PlannedPointsForPI BaselinePlannedPoints = new PlannedPointsForPI()
                {
                    BaselinePlannedPointsForPI = currentTotalPoints.ToString(),
                    TotalStoriesWithoutEstimates = totalStoriesWithoutEstimates.ToString(),
                    CurrentTotalPoints = currentTotalPoints.ToString(),
                    TotalClosedPoints = TotalClosedPoints,
                    SnapshotDate = DateTime.Now.Date.ToShortDateString(),
                    SnapshotNumber = sequence.ToString(),
                    TotalStories = totalStories,
                    TotalDefects = totalDefects,
                    Guid = Guid.NewGuid().ToString()
                };
                SnapShot snap = new SnapShot()
                {
                    Guid = Guid.NewGuid().ToString(),
                    SnapshotDate = DateTime.Now.Date,
                    SnapShotID = sequence
                };
                context.Snapshot.Add(snap);
                context.PlannedPointsForPI.Add(BaselinePlannedPoints);
                context.BaselineItem.AddRange(dataofInterest);
                //context.Item.AddRange(firstSnapshot);
                context.SaveChanges();
                return Content(Boolean.TrueString);
            }
            return Content(Boolean.FalseString);
        }
        #endregion

        #endregion

        #region Private Helpers
        //public static string AttributeValueNull(this XElement element, string attributeName)
        //{
        //    if (element == null)
        //        return "";
        //    else
        //    {
        //        XAttribute attr = element.Attribute(attributeName);
        //        return attr == null ? "" : attr.Value;
        //    }
        //}

        //public static string ElementValueNull(this XElement element)
        //{
        //    if (element != null)
        //        return element.Value;
        //    return "";
        //}

        private string JiraFilterUrlBuilder()
        {
            string url = string.Empty;
            url = url + GetJiraURL();
            url = url + JiraFilterUrlEndPointPart1;
            url = url + GetJiraFilterId() + "/SearchRequest-" + GetJiraFilterId() + ".xml" + JiraFilterUrlEndPointPart2 + "os_username=" + GetUserName() + "&os_password=" + GetPassword();
            return url;
            //string url = @"https://jira.ges.symantec.com/sr/jira.issueviews:searchrequest-xml/<filterid>/SearchRequest-<filterid>.xml?tempMax=9999&os_username=uid&os_password=pwd";
        }

        private List<RequestDependencyListModel> GetStoryDepdencyList(int? fromTeamId, int? toTeamId, int? status, int? needBySprintId, int? SosStatus)
        {
            var list = from doc in context.StoryDependency
                       join cs in context.Sprint on doc.NeedBySprintId equals cs.SprintId
                       where (needBySprintId == null || doc.NeedBySprintId == needBySprintId.Value)
                       join caa in context.Team on doc.ToTeamId equals caa.TeamId
                       where (toTeamId == null || doc.ToTeamId == toTeamId.Value)
                       join cpd in context.Team on doc.FromTeamId equals cpd.TeamId
                       where (fromTeamId == null || doc.FromTeamId == fromTeamId.Value)
                       join apd in context.Sprint on doc.NeedBySprintId equals apd.SprintId
                       where (needBySprintId == null || doc.NeedBySprintId == needBySprintId.Value)
                       select new RequestDependencyListModel()
                       {
                           DateCreated = doc.DateCreated,
                           RequestedByUser = doc.RequestedByUserId,
                           BlockedStory = doc.BlockedStory,
                           DependencyStory = doc.DependencyStory,
                           IsDeleted = doc.IsDeleted,
                           Description = doc.Description,
                           Detail = doc.Detail,
                           Status = doc.Status,
                           RejectReason = doc.RejectReason,
                           StoryDependencyId = doc.StoryDependencyId,
                           ToTeam = caa.TeamName,
                           IsToTeamStartTeam = caa.IsStarTeam,
                           FromTeam = cpd.TeamName,
                           NeedBySprint = apd.SprintName,
                           OwnedBy = doc.OwnedBy,
                           SoSComment = doc.SoSComment,
                           SoSStatus = doc.SoSStatus
                           //NeedBySprintList = GetNeedBySprintList()
                       };
            ViewBag.Sprints = GetNeedBySprintList();
            if (status != null)
            {
                list = list.Where(s => s.Status == (DependencyStatus)status).OrderBy(te => te.FromTeam).ThenBy(t => t.StoryDependencyId);
            }
            if (SosStatus != null)
            {
                list = list.Where(s => s.SoSStatus == (SoSStatus)SosStatus).OrderBy(te => te.FromTeam).ThenBy(t => t.StoryDependencyId);
            }
            return list.OrderBy(te => te.FromTeam).ThenBy(t => t.StoryDependencyId).ToList();
        }

        private List<RequestDependencyListModel> GetStoryDepdencyList()
        {
            var list = from doc in context.StoryDependency
                       from caa
                           in context.Team
                           .Where(c => c.TeamId == doc.ToTeamId)
                           .DefaultIfEmpty()
                       from cpd
                           in context.Team
                           .Where(pdc => pdc.TeamId == doc.FromTeamId)
                           .DefaultIfEmpty()
                       from apd
                           in context.Sprint
                           .Where(pda => pda.SprintId == doc.NeedBySprintId)
                           .DefaultIfEmpty()
                           //.Select(data => new RequestDependencyListModel
                       select new RequestDependencyListModel()
                       {
                           DateCreated = doc.DateCreated,
                           RequestedByUser = doc.RequestedByUserId,
                           BlockedStory = doc.BlockedStory,
                           DependencyStory = doc.DependencyStory,
                           IsDeleted = doc.IsDeleted,
                           Description = doc.Description,
                           Detail = doc.Detail,
                           Status = doc.Status,
                           RejectReason = doc.RejectReason,
                           StoryDependencyId = doc.StoryDependencyId,
                           ToTeam = caa.TeamName,
                           IsToTeamStartTeam = caa.IsStarTeam,
                           FromTeam = cpd.TeamName,
                           NeedBySprint = apd.SprintName,
                           OwnedBy = doc.OwnedBy,
                           SoSComment = doc.SoSComment,
                           SoSStatus = doc.SoSStatus
                           //NeedBySprintList = context.Sprint.ToList<Sprint>().ConvertAll<SelectListItem>(s => new SelectListItem
                           //{
                           //    Value = s.SprintId.ToString(),
                           //    Text = s.SprintName,
                           //    Selected = false
                           //}).ToList()
                       };
            ViewBag.Sprints = GetNeedBySprintList();
            return list.OrderBy(t => t.StoryDependencyId).ToList();
            //List<Sprint> sprintList = new List<Sprint>();
            //sprintList = context.Sprint.OrderBy(t => t.SprintId).ToList();
            //List<SelectListItem> sprints = sprintList
            //        .Select(s => new SelectListItem
            //        {
            //            Value = s.SprintId.ToString(),
            //            Text = s.SprintName,
            //            Selected = false
            //        }).ToList();

            //return sprints;
        }

        public bool UpdateDependency(RequestDependencyViewModel model)
        {
            StoryDependency stDepdency = context.Set<StoryDependency>().Find(model.StoryDependencyId);
            stDepdency.FromTeamId = model.FromTeamId;
            stDepdency.ToTeamId = model.ToTeamId;
            stDepdency.NeedBySprintId = model.NeedBySprintId;
            stDepdency.IsDeleted = false;
            stDepdency.RequestedByUserId = "1";
            stDepdency.DateCreated = DateTime.Now;//
            stDepdency.Description = model.Description;
            stDepdency.Detail = model.Detail;
            stDepdency.BlockedStory = model.BlockedStory;
            stDepdency.DependencyStory = model.DependencyStory;
            stDepdency.Status = DependencyStatus.Requested;

            context.Set<StoryDependency>().Attach(stDepdency);
            context.Entry<StoryDependency>(stDepdency).Property(p => p.Status).IsModified = true;
            context.Entry<StoryDependency>(stDepdency).Property(p => p.FromTeamId).IsModified = true;
            context.Entry<StoryDependency>(stDepdency).Property(p => p.ToTeamId).IsModified = true;
            context.Entry<StoryDependency>(stDepdency).Property(p => p.NeedBySprintId).IsModified = true;
            context.Entry<StoryDependency>(stDepdency).Property(p => p.IsDeleted).IsModified = true;
            context.Entry<StoryDependency>(stDepdency).Property(p => p.RequestedByUserId).IsModified = true;
            context.Entry<StoryDependency>(stDepdency).Property(p => p.DateCreated).IsModified = true;
            context.Entry<StoryDependency>(stDepdency).Property(p => p.Description).IsModified = true;
            context.Entry<StoryDependency>(stDepdency).Property(p => p.BlockedStory).IsModified = true;
            context.Entry<StoryDependency>(stDepdency).Property(p => p.DependencyStory).IsModified = true;
            context.Entry<StoryDependency>(stDepdency).Property(p => p.Detail).IsModified = true;

            bool result = Convert.ToBoolean(context.SaveChanges());
            return result;
        }

        private List<FeatureListModel> GetFeatureList()
        {
            var list = from doc in context.Feature
                       join cpd in context.Team on doc.FromTeamId equals cpd.TeamId
                       join apd in context.Sprint on doc.CompleteBySprintId equals apd.SprintId
                       select new FeatureListModel()
                       {
                           FeatureId = doc.FeatureId,
                           FeatureName = doc.FeatureName,
                           FromTeam = cpd.TeamName,
                           CompleteBySprint = apd.SprintName
                       };
            return list.OrderBy(t => t.FeatureId).ToList();
        }

        public bool UpdateFeature(FeatureViewModel model)
        {
            Features feature = context.Set<Features>().Find(model.FeatureId);
            feature.FromTeamId = model.FromTeamId;
            feature.CompleteBySprintId = model.CompleteBySprintId;
            feature.FeatureName = model.FeatureName;

            context.Set<Features>().Attach(feature);
            context.Entry<Features>(feature).Property(p => p.FromTeamId).IsModified = true;
            context.Entry<Features>(feature).Property(p => p.CompleteBySprintId).IsModified = true;
            context.Entry<Features>(feature).Property(p => p.FeatureName).IsModified = true;


            bool result = Convert.ToBoolean(context.SaveChanges());
            return result;
        }

        private int GetSprintsInPI()
        {
            var jiraSettings = ConfigurationManager.GetSection("JiraSettings") as NameValueCollection;
            int NumberOfSprints = 0;
            if (jiraSettings != null)
            {
                NumberOfSprints = int.Parse(jiraSettings["TotalSprintsInPI"].ToString());
            }
            return NumberOfSprints;
        }
        private DateTime GetStartDateOfPI()
        {
            var jiraSettings = ConfigurationManager.GetSection("JiraSettings") as NameValueCollection;
            DateTime StartOfPI = DateTime.Now;
            if (jiraSettings != null)
            {
                StartOfPI = DateTime.Parse(jiraSettings["StartDateofPI"].ToString());
            }
            return StartOfPI;
        }
        private DateTime GetEndDateOfPI()
        {
            var jiraSettings = ConfigurationManager.GetSection("JiraSettings") as NameValueCollection;
            DateTime EndOfPI = DateTime.Now;
            if (jiraSettings != null)
            {
                EndOfPI = DateTime.Parse(jiraSettings["EndDateofPI"].ToString());
            }
            return EndOfPI;
        }
        private List<DateTime> GetDatesBetween(DateTime startDate, DateTime endDate)
        {
            List<DateTime> allDates = new List<DateTime>();
            for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
                allDates.Add(date);
            return allDates;
        }

        private string GetJiraURL()
        {
            var jiraSettings = ConfigurationManager.GetSection("JiraSettings") as NameValueCollection;
            string JiraURL = string.Empty;
            if (jiraSettings != null)
            {
                JiraURL = jiraSettings["JiraURL"].ToString();
            }
            return JiraURL;
        }

        private string GetJiraFilterId()
        {
            var jiraSettings = ConfigurationManager.GetSection("JiraSettings") as NameValueCollection;
            string JiraFilterId = string.Empty;
            if (jiraSettings != null)
            {
                JiraFilterId = jiraSettings["JiraFilterId"].ToString();
            }
            return JiraFilterId;
        }

        private string GetUserName()
        {
            var jiraSettings = ConfigurationManager.GetSection("JiraSettings") as NameValueCollection;
            string UserName = string.Empty;
            if (jiraSettings != null)
            {
                UserName = jiraSettings["UserName"].ToString();
            }
            return UserName;
        }

        private string GetPassword()
        {
            var jiraSettings = ConfigurationManager.GetSection("JiraSettings") as NameValueCollection;
            string pwd = string.Empty;
            if (jiraSettings != null)
            {
                pwd = jiraSettings["Password"].ToString();
            }
            return pwd;
        }

        private string GetProjectKey(string TeamName)
        {
            switch (TeamName.ToLower())
            {
                case "starsix":
                    return "STARSIX";
                case "fortress":
                    return "FORTRESS";
                case "symulator":
                    return "SYMULATOR";
                case "teamx":
                    return "TEAMX";
                case "carrera":
                    return "CARRERA";
                case "skynet":
                    return "SKYNET";
                case "ringzero":
                    return "RINGZERO";
                case "alpha":
                    return "ALPHA";
                case "orbit":
                    return "ORBIT";
                case "bud":
                    return "BUD";
                case "starcm":
                    return "STARCM";
                case "uss":
                    return "USS";
                case "conteng":
                    return "CONTENG";
                case "rbcs":
                    return "SCRBCS";
                case "dbcs":
                    return "DBCS";
                default:
                    return string.Empty;
            }
        }

        private bool CreateAndLinkStory(string blockedStoryKey, string summary, int ToTeamID, string Description, int TargetSprint, out string DependentStoryKey, out String Status)
        {
            bool bSuccess = false;
            string ProjectKey = string.Empty;
            DependentStoryKey = string.Empty;
            Status = "Error";
            try
            {
                Team stTeam = context.Set<Team>().Find(ToTeamID);
                Sprint stSprint = context.Set<Sprint>().Find(TargetSprint);

                var restClient = new RestClient(jiraRestApiUrl)
                {
                    Authenticator = new HttpBasicAuthenticator(jiraUsername, jiraPassword)
                };
                var request = new RestRequest(JiraCreateIssueResource, Method.POST);
                ProjectKey = GetProjectKey(stTeam.TeamName);
                if (string.IsNullOrEmpty(ProjectKey))
                {
                    bSuccess = false;
                    return bSuccess;
                }
                JiraIntegration.Models.Issue i = new JiraIntegration.Models.Issue();
                i.fields.summary = summary;
                i.fields.project = new JiraIntegration.Models.Project() { key = ProjectKey };
                i.fields.customfield_10000 = GetSprintID(stTeam.TeamName, stSprint.SprintName);
                i.fields.issuetype = new JiraIntegration.Models.IssueType() { name = "Story" };
                i.fields.description = Description;

                string json = request.JsonSerializer.Serialize(i);

                request.Method = Method.POST;
                request.AddHeader("Accept", "application/json");
                request.Parameters.Clear();
                request.AddParameter("application/json", json, ParameterType.RequestBody);
                IRestResponse RestResponse = restClient.Post(request);
                JObject obj = RestResponse.ContentType.Contains("application/json") ? JObject.Parse(RestResponse.Content) : null;
                DependentStoryKey = obj["key"].ToString();
                if (!RestResponse.Content.Contains("errorMessages"))
                {
                    bSuccess = LinkStories(blockedStoryKey, DependentStoryKey, bSuccess, restClient);
                }
                else
                {
                    Status = "Error: " + RestResponse.Content;
                }
                return bSuccess;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                bSuccess = false;
                return bSuccess;
            }
        }

        private static bool LinkStories(string blockedStoryKey, string DependentStoryKey, bool bSuccess, RestClient restClient)
        {
            bSuccess = true;
            //Link story code here
            var linkRequest = new RestRequest(JiraLinkIssueResource, Method.POST);
            issueLink issueLink = new issueLink();
            issueLink.type.name = "Dependency";
            issueLink.inwardIssue.key = blockedStoryKey;
            issueLink.outwardIssue.key = DependentStoryKey;
            string linkJsonRequest = linkRequest.JsonSerializer.Serialize(issueLink);
            linkRequest.Method = Method.POST;
            linkRequest.AddHeader("Accept", "application/json");
            linkRequest.Parameters.Clear();
            linkRequest.AddParameter("application/json", linkJsonRequest, ParameterType.RequestBody);
            IRestResponse linkResponse = restClient.Post(linkRequest);
            //TODO: Fix the code to check on response code / status code and not rely on the JSON being returned, 
            //use the status code as documented in JIRA API https://docs.atlassian.com/jira/REST/latest/#d2e1037
            if (linkResponse != null)
            {
                HttpStatusCode code = linkResponse.StatusCode;
                if (linkResponse.StatusDescription.ToUpper() != "CREATED")
                    bSuccess = false;
                //JObject objLinkResponse = linkResponse.ContentType.Contains("application/json") ? Newtonsoft::Newtonsoft.Json.Linq.JObject.Parse(linkResponse.Content) : null;

            }
            return bSuccess;
        }

        private string GetSprintID(string team, string sprintname)
        {
            return ConfigurationManager.AppSettings.Get(team + "_" + sprintname);
        }

        private void LogMessage(MessageLog log)
        {
            context.MessageLog.Add(log);
            bool result = Convert.ToBoolean(context.SaveChanges());
        }
        #endregion
    }
}