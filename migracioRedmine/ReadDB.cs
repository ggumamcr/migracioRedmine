using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace migracioRedmine
{
    class ReadDB
    {
        public static Project ReadDBProject(MySqlDataReader dataReader)
        {
            Project project = new Project();
            project.id = dataReader["id"].ToString();
            project.name = RemoveDiacritics(dataReader["name"].ToString());
            project.description = RemoveDiacritics(dataReader["description"].ToString());
            project.homepage = dataReader["homepage"].ToString();
            project.is_public = dataReader["is_public"].ToString();
            project.parent_id = dataReader["parent_id"].ToString();
            project.created_on = dataReader["created_on"].ToString();
            project.updated_on = dataReader["updated_on"].ToString();
            project.identifier = dataReader["identifier"].ToString();
            project.status = dataReader["status"].ToString();
            project.lft = dataReader["lft"].ToString();
            project.rgt = dataReader["rgt"].ToString();
            project.inherit_members = dataReader["inherit_members"].ToString();
            project.default_version_id = dataReader["default_version_id"].ToString();
            project.default_assigned_to_id = dataReader["default_assigned_to_id"].ToString();
            return project;
        }

        public static string RemoveDiacritics(string text)
        {
            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }

        public static Issue ReadDBIssue(MySqlDataReader dataReader)
        {
            Issue issue = new Issue();
            issue.id = dataReader["id"].ToString();
            issue.tracker_id = dataReader["tracker_id"].ToString();
            issue.project_id = dataReader["project_id"].ToString();
            issue.subject = dataReader["subject"].ToString();
            issue.description = dataReader["description"].ToString();
            issue.status_id = dataReader["status_id"].ToString();
            issue.assigned_to_id = dataReader["assigned_to_id"].ToString();
            issue.priority_id = dataReader["Priority_id"].ToString();
            issue.author_id = dataReader["author_id"].ToString();
            issue.done_ratio = dataReader["done_ratio"].ToString();
            issue.estimated_hours = dataReader["estimated_hours"].ToString();
            return issue;
        }

        public static Cost ReadDBCost(MySqlDataReader dataReader)
        {
            Cost cost = new Cost();
            cost.entity_type = "Issue";
            cost.entity_id = dataReader["issue_id"].ToString();
            cost.price1 = dataReader["preucost"].ToString();
            cost.spent_on = dataReader["data"].ToString();
            cost.name = dataReader["descripcio"].ToString();
            cost.project_id = dataReader["project_id"].ToString();
            custom_fields custom_fields = new custom_fields();
            custom_fields.numalbara = dataReader["numalbara"].ToString();
            custom_fields.proveidor = dataReader["numalbara"].ToString();
            return cost;
        }





    }
}
