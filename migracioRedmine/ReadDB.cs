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

    }
}
