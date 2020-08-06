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
        public static string easyredmine = "https://b3722b455a.fra2.easyredmine.com/";
        public static string key = "? key = b17e9372ec58cd8a17190f83c8084bc9321ca12a";
        public static async Task<string> PostProject(Project project, List<Tuple<string, string>> tIds)
        {
            try
            {
                var baseAddress = new Uri(easyredmine);
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
                        var response = await httpClient.PostAsync("projects.xml" + key, content);
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

        public static async Task<Issue> PostIssue(Issue issue)
        {
            String outString = String.Empty;
            try
            {
                var baseAddress = new Uri(easyredmine);
                using (var httpClient = new HttpClient { BaseAddress = baseAddress })
                {
                    outString = Obj2Str(issue);
                    using (var content = new StringContent(outString, Encoding.UTF8, "application/xml"))
                    {
                        var response = await httpClient.PostAsync("issues.xml"+key, content);
                        if (!response.IsSuccessStatusCode)
                        {
                            bool rety = response.IsSuccessStatusCode;
                        }
                        string responseData = await response.Content.ReadAsStringAsync();

                        XmlSerializer serializer = new XmlSerializer(issue.GetType());
                        StringReader rdr = new StringReader(responseData);
                        Issue resultingMessage = (Issue)serializer.Deserialize(rdr);
                        return resultingMessage;
                    }
                }
            }
            catch (Exception ex)
            {
                issue.subject = "error" + issue.subject;
                return issue;
            }
        }

        public static async Task<AttachToken> PostUpload(Attachments att)
        {
            String outString = String.Empty;
            try
            {
                var baseAddress = new Uri(easyredmine);
                string folder = att.disk_directory.Replace('/', '\\');
                string filepath = @"C:\Users\gguma\Documents\files\" + folder + @"\" + att.disk_filename;

                byte[] data = File.ReadAllBytes(filepath);
                string text = Encoding.Default.GetString(data);

                using (var httpClient = new HttpClient { BaseAddress = baseAddress })
                {
                    using (var content = new StringContent(text, System.Text.Encoding.Default, "application/octet-stream"))

                    {
                        using (var response = await httpClient.PostAsync("uploads.xml"+key, content))
                        {
                            string responseData = await response.Content.ReadAsStringAsync();
                            AttachToken token = new AttachToken();
                            XmlSerializer serializer = new XmlSerializer(token.GetType());
                            StringReader rdr = new StringReader(responseData);
                            AttachToken resultingMessage = (AttachToken)serializer.Deserialize(rdr);
                            return resultingMessage;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public static async Task PostAttach(Attachments att, AttachToken token, string id)
        {
            String outString = String.Empty;
            try
            {
                var baseAddress = new Uri(easyredmine);

                using (var httpClient = new HttpClient { BaseAddress = baseAddress })
                {
                    using (var content = new StringContent("<attach><entity_type>" + att.container_type + "</entity_type><entity_id>" + id + "</entity_id ><attachments type='array'><attachment><token>" + token.token + "</token></attachment></attachments></attach>", System.Text.Encoding.Default, "application/octet-stream"))
                    {
                        using (var response = await httpClient.PostAsync("attach.xml"+key, content))
                        {
                            string responseData = await response.Content.ReadAsStringAsync();

                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        public static async Task<string> PostTimeEntry(TimeEntries time)
        {
            String outString = String.Empty;
            try
            {
                var baseAddress = new Uri(easyredmine);
                using (var httpClient = new HttpClient { BaseAddress = baseAddress })
                {
                    outString = Obj2Str(time);
                    using (var content = new StringContent(outString, Encoding.UTF8, "application/xml"))
                    {
                        var response = await httpClient.PostAsync("time_entries.xml"+key, content);
                        if (!response.IsSuccessStatusCode)
                        {
                            bool rety = response.IsSuccessStatusCode;
                        }
                        string responseData = await response.Content.ReadAsStringAsync();

                        XmlSerializer serializer = new XmlSerializer(time.GetType());
                        StringReader rdr = new StringReader(responseData);
                        TimeEntries resultingMessage = (TimeEntries)serializer.Deserialize(rdr);
                        return resultingMessage.hours;
                    }
                }
            }
            catch (Exception ex)
            {
                return "errorHours";
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
