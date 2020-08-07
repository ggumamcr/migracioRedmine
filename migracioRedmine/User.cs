using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace migracioRedmine
{
    [XmlRoot("users")]
    public class Users
    {
        [XmlElement("user")]
        public User User { get; set; }
    }

    public class User
    {
        public string id { get; set; }
        public string login { get; set; }
        public string admin { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }
    }
    [XmlRoot("membership")]
    public class Memberships
    {
        public string user_id { get; set; }
        public role role_ids { get; set; }
        [XmlIgnore]
        public string project_id { get; set; }
        [XmlIgnore]
        public string member_id { get; set; }
    }
    public class role
    {
        [XmlAttribute("type")]
        public string type { get; set; }
        public string role_id { get; set; }
    }
}
