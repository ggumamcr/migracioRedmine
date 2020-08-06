using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace migracioRedmine
{
    [XmlRoot("issue")]
    public class Issue
    {
        public string id { get; set; }
        public string project_id { get; set; }
        public string tracker_id { get; set; }
        public string status_id { get; set; }
        public string priority_id { get; set; }
        public string author_id { get; set; }
        public string assigned_to_id { get; set; }
        public string subject { get; set; }
        public string description { get; set; }
        public string done_ratio { get; set; }
        public string estimated_hours { get; set; }
        public uploads uploads { get; set; }

    }
    public class uploads
    {
        [XmlAttribute("type")]
        public string type { get; set; }
        [XmlElement("")]
        public upload[] upload { get; set; }
    }

    [XmlRoot("")]
    public class upload
    {
        public string token { get; set; }
        public string filename { get; set; }
        public string content_type { get; set; }
    }
    public class IssueReturn
    {
        public string id { get; set; }
    }
}
