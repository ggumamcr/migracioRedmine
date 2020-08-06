using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace migracioRedmine
{
    [XmlRoot("attach")]
    public class Attachments
    {
        public string id { get; set; }
        public string container_id { get; set; }
        public string container_type { get; set; }
        public string filename { get; set; }
        public string disk_filename { get; set; }
        public string filesize { get; set; }
        public string content_type { get; set; }
        public string digest { get; set; }
        public string downloads { get; set; }
        public string author_id { get; set; }
        public string created_on { get; set; }
        public string description { get; set; }
        public string disk_directory { get; set; }
    }
    [XmlRoot("upload")]
    public class AttachToken
    {
        public string id { get; set; }
        public string token { get; set; }
    }

}
