using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace migracioRedmine
{
    [XmlRoot("easy_money")]
    public class Cost
    {
        public string entity_type { get; set; }
        public string entity_id { get; set; }
        public string price1 { get; set; }
        public string spent_on { get; set; }
        public string name { get; set; }
        [XmlIgnore]
        public string project_id { get; set; }
    }
    public class custom_fields
    {
        public string proveidor { get; set; }
        public string numalbara { get; set; }
    }
}
