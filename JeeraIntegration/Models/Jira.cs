using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JiraIntegration.Models
{
    public class type
    {
        public string name { get; set; }
    }
    public class inwardIssue
    {
        public string key { get; set; }
    }
    public class outwardIssue
    {
        public string key { get; set; }
    }
    public class issueLink
    {
        public type type { get; set; }
        public inwardIssue inwardIssue { get; set; }
        public outwardIssue outwardIssue { get; set; }
        public issueLink()
        {
            type = new type();
            inwardIssue = new inwardIssue();
            outwardIssue = new outwardIssue();
        }
    }
    
    public class Issue
    {
        public Fields fields { get; set; }
        public issueLink issueLink { get; set; }
        public Issue()
        {
            fields = new Fields();
            issueLink = new issueLink();
        }
    }
    public class Fields
    {
        public Project project { get; set; }
        public string summary { get; set; }
        public string description { get; set; }
        public string customfield_10000 { get; set; }
        //public string Sprint { get; set; }
        public IssueType issuetype { get; set; }

        public Fields()
        {
            project = new Project();
            issuetype = new IssueType();
        }
    }

    public class Project
    {
        public string key { get; set; }
    }
    public class IssueType
    {
        public string name { get; set; }
    }

    public class DependencyStoryCreated
    {
        public string id{ get; set; }
        public string key{ get; set; }
        public string self{ get; set; }
    }
}