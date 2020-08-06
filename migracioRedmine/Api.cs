using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace migracioRedmine
{
    class Api
    {
        public static async Task<string> PostProject(Project project, List<Tuple<string, string>> tIds)
        {
            try
            {
                var baseAddress = new Uri("https://b3722b455a.fra2.easyredmine.com/");
                using (var httpClient = new HttpClient { BaseAddress = baseAddress })
                {
                    if (project.parent_id != null)
                    {
                        foreach (var lst in tIds)
                        {
                            if (lst.Item1.Equals(project.parent_id))
                                project.parent_id = lst.Item2;
                        }
                    }

                    String outString = String.Empty;

                    outString = Obj2Str(project);
                    using (var content = new StringContent(outString, Encoding.UTF8, "application/xml"))
                    {
                        var response = await httpClient.PostAsync("projects.xml?key=b17e9372ec58cd8a17190f83c8084bc9321ca12a", content);
                        if (!response.IsSuccessStatusCode)
                        {
                            bool rety = response.IsSuccessStatusCode;

                        }
                        string responseData = await response.Content.ReadAsStringAsync();

                        XmlSerializer serializer = new XmlSerializer(project.GetType());
                        StringReader rdr = new StringReader(responseData);
                        Project resultingMessage = (Project)serializer.Deserialize(rdr);
                        return resultingMessage.id;
                    }
                }
            }
            catch (Exception ex)
            {
                return "error";
            }
        }



        public static string Obj2Str(object obj)
        {
            XmlSerializer xs = null;
            //These are the objects that will free us from extraneous markup.
            XmlWriterSettings settings = null;
            XmlSerializerNamespaces ns = null;
            //We use a XmlWriter instead of a StringWriter.
            XmlWriter xw = null;
            String outString = String.Empty;

            settings = new XmlWriterSettings();
            settings.OmitXmlDeclaration = true;
            //To get rid of the default namespaces we create a new
            //set of namespaces with one empty entry.
            ns = new XmlSerializerNamespaces();
            ns.Add("", "");

            StringBuilder sb = new StringBuilder();
            xs = new XmlSerializer(obj.GetType());

            //We create a new XmlWriter with the previously created settings 
            //(to OmitXmlDeclaration).
            xw = XmlWriter.Create(sb, settings);
            //We call xs.Serialize and pass in our custom 
            //XmlSerializerNamespaces object.
            xs.Serialize(xw, obj, ns);
            xw.Flush();

            outString = sb.ToString();
            return outString;
        }
    }
}
