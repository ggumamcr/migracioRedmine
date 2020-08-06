using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace migracioRedmine
{
    [XmlRoot("time_entry")]
    public class TimeEntries
    {
        public string issue_id { get; set; }
        public string hours { get; set; }

    }
}
