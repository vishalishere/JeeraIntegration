using JiraIntegration.Models;

namespace JiraIntegration.DAL
{
    public class JiraDBInitalizer : System.Data.Entity.CreateDatabaseIfNotExists<JiraDbContext>
    //public class JiraDBInitalizer : System.Data.Entity.DropCreateDatabaseAlways<JiraDbContext>
    {
        protected override void Seed(JiraDbContext context)
        {
            
        }
    }    
}

