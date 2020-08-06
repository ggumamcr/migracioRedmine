using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace migracioRedmine
{
    [XmlRoot("project")]
    public class Project
    {
        public string id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string homepage { get; set; }
        public string is_public { get; set; }
        public string parent_id { get; set; }
        public string created_on { get; set; }
        public string updated_on { get; set; }
        public string identifier { get; set; }
        public string status { get; set; }
        public string lft { get; set; }
        public string rgt { get; set; }
        public string inherit_members { get; set; }
        public string default_version_id { get; set; }
        public string default_assigned_to_id { get; set; }
    }
}
