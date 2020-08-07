using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using Ubiety.Dns.Core.Records;
using mcrLog;
using K4os.Compression.LZ4.Internal;

namespace migracioRedmine
{
    class DBconnection
    {
        private MySqlConnection connection;
        private string server;
        private string database;
        private string uid;
        private string password;
        public string[] users;

        //Constructor
        public DBconnection()
        {
            Initialize();
        }

        //Initialize values
        private void Initialize()
        {
            server = "localhost";
            database = "bitnami_redmineplusagile";
            uid = "root";
            password = "Juliol2020";
            string connStr = "server=localhost;user=root;database=bitnami_redmineplusagile;port=3308;password=Juliol2020";

            connection = new MySqlConnection(connStr);
        }
        private bool OpenConnection()
        {
            try
            {
                connection.Open();
                return true;
            }
            catch (MySqlException ex)
            {
                //When handling errors, you can your application's response based on the error number.
                //The two most common error numbers when connecting are as follows:
                //0: Cannot connect to server.
                //1045: Invalid user name and/or password.
                switch (ex.Number)
                {
                    case 0:
                        MessageBox.Show("Cannot connect to server.  Contact administrator");
                        break;

                    case 1045:
                        MessageBox.Show("Invalid username/password, please try again");
                        break;
                }
                return false;
            }
        }

        private bool CloseConnection()
        {
            try
            {
                connection.Close();
                return true;
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }

        public async System.Threading.Tasks.Task SelectProjectAsync()
        {
            try
            {

                List<Project> pList = new List<Project>();
                pList = ListProjects();

                List<Tuple<string, string>> tIds = new List<Tuple<string, string>>();
                TextWriter tw = new StreamWriter("projects.txt");
                foreach (Project oprg in pList)
                {
                    Tuple<string, string> ids = new Tuple<string, string>(oprg.id, await Api.PostProject(oprg, tIds));
                    tIds.Add(ids);
                    tw.WriteLine(ids.Item1 + " " + ids.Item2);
                    if (ids.Item2 != "error")
                    {
                        InsertIdProject(ids.Item2, ids.Item1);
                    }
                }
                tw.Close();
                MessageBox.Show("ProjectsCreated");
            }
            catch(Exception ex)
            {
                NLog.LogError("Redmine", "DBconnection", "SelectProjectAsync", ex);
            }

        }
        public async System.Threading.Tasks.Task SelectIssueAsync()
        {
            try
            {

                List<Project> pList = new List<Project>();
                pList = ListProjects();

                List<Tuple<string, string>> tIds = new List<Tuple<string, string>>();
                TextWriter tw = new StreamWriter("Issues.txt");
                foreach (Project oprg in pList)
                {
                    Tuple<string, string> ids = new Tuple<string, string>(oprg.id, ListIdProjects(oprg.id));
                    tIds.Add(ids);
                    List<Issue> iList = new List<Issue>();
                    iList = ListIssue(ids.Item1, ids.Item2);
                    tw.WriteLine(ids.Item1 + " " + ids.Item2);

                    foreach (Issue i in iList)
                    {
                        List<Attachments> aList = new List<Attachments>();
                        aList = SelectAttachments(i.id);

                        if (aList != null)
                        {
                            int index = 0;
                            uploads uploads = new uploads();
                            uploads.type = "array";
                            uploads.upload = new upload[aList.Count];
                            foreach (Attachments a in aList)
                            {
                                AttachToken token = await Api.PostUpload(a);
                                upload upload = new upload();
                                upload.token = token.token;
                                upload.filename = a.filename;
                                upload.content_type = a.content_type;
                                uploads.upload[index] = upload;
                                i.uploads = uploads;
                                index++;
                            }
                        }
                        Issue returned = await Api.PostIssue(i);
                        InsertIdIssue(returned.id, i.id);
                        tw.WriteLine("   " + returned.subject);
                    }
                }

                MessageBox.Show("Issue Done");
            }
            catch (Exception ex)
            {
                NLog.LogError("Redmine", "DBconnection", "SelectIssueAsync", ex);
            }
        }

        public async System.Threading.Tasks.Task SelectTimeAsync()
        {
            try
            {

                List<Project> pList = new List<Project>();
                pList = ListProjects();

                List<Tuple<string, string>> tIds = new List<Tuple<string, string>>();
                TextWriter tw = new StreamWriter("Time.txt");
                foreach (Project oprg in pList)
                {
                    Tuple<string, string> ids = new Tuple<string, string>(oprg.id, ListIdProjects(oprg.id));
                    tIds.Add(ids);
                    List<Issue> iList = new List<Issue>();
                    iList = ListIssue(ids.Item1, ids.Item2);
                    tw.WriteLine(ids.Item1 + " " + ids.Item2);

                    foreach (Issue i in iList)
                    {
                        tw.WriteLine("   " + i.subject);

                        List<TimeEntries> tList = new List<TimeEntries>();
                        tList = ListTimeEntries(i.id, ListIdIssue(i.id), i.project_id);

                        foreach (TimeEntries Time in tList)
                        {
                            string returnedTime = await Api.PostTimeEntry(Time);
                            tw.WriteLine("        " + returnedTime);
                        }
                    }
                }

                MessageBox.Show("Done");
            }
            catch (Exception ex)
            {
                NLog.LogError("Redmine", "DBconnection", "SelectTimeAsync", ex);
            }
        }

        public bool InsertIdProject(string new_id, string old_id)
        {
            try
            {
                string query = "update projects set default_assigned_to_id = " + new_id + " where id = " + old_id;
                if (this.OpenConnection() == true)
                {
                    //Create Command
                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    //Create a data reader and Execute the command

                    cmd.ExecuteNonQuery();
                    this.CloseConnection();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                NLog.LogError("Redmine", "DBconnection", "InsertIdProject", ex);
                return false;
            }
        }

        public List<Project> ListProjects()
        {
            try
            {
                string query = "SELECT * FROM projects WHERE id<80";

                //Open connection
                if (this.OpenConnection() == true)
                {
                    //Create Command
                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    //Create a data reader and Execute the command
                    MySqlDataReader dataReader = cmd.ExecuteReader();

                    List<Project> pList = new List<Project>();
                    //Public lstReadOF_BBDD As New List(Of CLSLinFormulaDB)

                    //Read the data and store them in the list
                    while (dataReader.Read())
                    {
                        Project project = new Project();
                        project = ReadDB.ReadDBProject(dataReader);
                        if (project.id != null)
                        {
                            pList.Add(project);
                        }
                        else
                        {
                            bool note = false;
                        }
                    }

                    dataReader.Close();
                    this.CloseConnection();
                    return pList;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                NLog.LogError("Redmine", "DBconnection", "ListProjects", ex);
                return null;
            }
        }
        public string ListIdProjects(string id)
        {
            try
            {
                string query = "SELECT * FROM projects WHERE id=" + id;

                //Open connection
                if (this.OpenConnection() == true)
                {
                    //Create Command
                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    //Create a data reader and Execute the command
                    MySqlDataReader dataReader = cmd.ExecuteReader();
                    //Read the data and store them in the list
                    string new_id = null;
                    while (dataReader.Read())
                    {
                        new_id = dataReader["default_assigned_to_id"].ToString();
                    }

                    dataReader.Close();
                    this.CloseConnection();
                    return new_id;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                NLog.LogError("Redmine", "DBconnection", "ListIdProjects", ex);
                return null;
            }
        }


        public List<Issue> ListIssue(string id, string new_id)
        {
            try
            {
                string query = "SELECT * FROM issues WHERE project_id=" + id;

                //Open connection
                if (this.OpenConnection() == true)
                {
                    //Create Command
                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    //Create a data reader and Execute the command
                    MySqlDataReader dataReader = cmd.ExecuteReader();

                    List<Issue> iList = new List<Issue>();
                    //Read the data and store them in the list
                    while (dataReader.Read())
                    {
                        Issue issue = new Issue();
                        issue = ReadDB.ReadDBIssue(dataReader);
                        issue.project_id = new_id;
                        issue.priority_id = "12";
                        issue.author_id = "5";
                        if (issue != null)
                        {
                            iList.Add(issue);
                        }
                    }
                    dataReader.Close();
                    this.CloseConnection();

                    return iList;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                NLog.LogError("Redmine", "DBconnection", "ListIssue", ex);
                return null;
            }

        }
        public bool InsertIdIssue(string new_id, string old_id)
        {
            try
            {
                string query = "update issues set fixed_version_id = " + new_id + " where id = " + old_id;
                if (this.OpenConnection() == true)
                {
                    //Create Command
                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    //Create a data reader and Execute the command

                    cmd.ExecuteNonQuery();
                    this.CloseConnection();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                NLog.LogError("Redmine", "DBconnection", "InsertIdIssue", ex);
                return false;
            }

        }
        public string ListIdIssue(string id)
        {
            try
            {
                string query = "SELECT * FROM issues WHERE id=" + id;

                //Open connection
                if (this.OpenConnection() == true)
                {
                    //Create Command
                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    //Create a data reader and Execute the command
                    MySqlDataReader dataReader = cmd.ExecuteReader();
                    //Read the data and store them in the list
                    string new_id = null;
                    while (dataReader.Read())
                    {
                        new_id = dataReader["fixed_version_id"].ToString();
                    }

                    dataReader.Close();
                    this.CloseConnection();
                    return new_id;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                NLog.LogError("Redmine", "DBconnection", "ListIdIssue", ex);
                return "";
            }
        }

        public List<Attachments> SelectAttachments(string issue_id)
        {
            try
            {
                string query = "SELECT * FROM attachments WHERE container_type='Issue' and container_id=" + issue_id;


                //Open connection
                if (this.OpenConnection() == true)
                {
                    //Create Command
                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    //Create a data reader and Execute the command
                    MySqlDataReader dataReader = cmd.ExecuteReader();
                    List<Attachments> file = new List<Attachments>();
                    //Read the data and store them in the list
                    while (dataReader.Read())
                    {
                        Attachments att = new Attachments();
                        att.id = dataReader["id"].ToString();
                        att.container_id = dataReader["container_id"].ToString();
                        att.container_type = dataReader["container_type"].ToString();
                        att.filename = dataReader["filename"].ToString();
                        att.disk_filename = dataReader["disk_filename"].ToString();
                        att.filesize = dataReader["filesize"].ToString();
                        att.content_type = dataReader["content_type"].ToString();
                        att.digest = dataReader["digest"].ToString();
                        att.downloads = dataReader["downloads"].ToString();
                        att.author_id = dataReader["author_id"].ToString();
                        att.created_on = dataReader["created_on"].ToString();
                        att.description = dataReader["description"].ToString();
                        att.disk_directory = dataReader["disk_directory"].ToString();

                        if (att != null)
                        {
                            file.Add(att);
                        }
                    }

                    //close Data Reader
                    dataReader.Close();

                    //close Connection
                    this.CloseConnection();

                    //return list to be displayed
                    return file;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                NLog.LogError("Redmine", "DBconnection", "SelectAttachments", ex);
                return null;
            }
        }

        public List<TimeEntries> ListTimeEntries(string issue_id, string new_id, string project_id)
        {
            try
            {
                string query = "SELECT * FROM time_entries WHERE issue_id=" + issue_id;

                //Open connection
                if (this.OpenConnection() == true)
                {
                    //Create Command
                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    //Create a data reader and Execute the command
                    MySqlDataReader dataReader = cmd.ExecuteReader();
                    List<TimeEntries> tList = new List<TimeEntries>();

                    while (dataReader.Read())
                    {
                        TimeEntries tEntry = new TimeEntries();
                        tEntry.hours = dataReader["hours"].ToString();
                        tEntry.issue_id = new_id;
                        if (tEntry != null)
                        {
                            tList.Add(tEntry);
                        }
                    }
                    dataReader.Close();
                    this.CloseConnection();

                    return tList;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                NLog.LogError("Redmine", "DBconnection", "ListTimeEntries", ex);
                return null;
            }
        }

        public async Task<string[]> ListUsersAsync()
        {
            try
            {
                users = new string[26];

                for (int i = 0; i < 26; i++)
                {
                    string name = null;
                    string query = "SELECT * FROM users WHERE id=" + i;
                    if (this.OpenConnection() == true)
                    {
                        MySqlCommand cmd = new MySqlCommand(query, connection);
                        MySqlDataReader dataReader = cmd.ExecuteReader();

                        while (dataReader.Read())
                        {
                            name = dataReader["firstname"].ToString();
                        }
                        dataReader.Close();
                        this.CloseConnection();
                        if (name != null && name != "")
                        {
                            users[i] = await Api.User(name);
                        }
                    }
                }
                MessageBox.Show("Users Done");
                return users;
            }
            catch (Exception ex)
            {
                NLog.LogError("Redmine", "DBconnection", "ListUsers", ex);
                return null;
            }
        }

        public async System.Threading.Tasks.Task SelectMembershipsAsync()
        {
            try
            {

                List<Project> pList = new List<Project>();
                pList = ListProjects();

                List<Tuple<string, string>> tIds = new List<Tuple<string, string>>();
                TextWriter tw = new StreamWriter("Memberships.txt");
                foreach (Project oprg in pList)
                {
                    Tuple<string, string> ids = new Tuple<string, string>(oprg.id, ListIdProjects(oprg.id));
                    tIds.Add(ids);
                    List<Memberships> mList = new List<Memberships>();
                    mList = ListRoles(ids.Item1, ListMemberships(ids.Item1, ids.Item2));
                    tw.WriteLine(ids.Item1 + " " + ids.Item2);

                    foreach (Memberships i in mList)
                    {
                        await Api.PostProjectMembership(i);
                    }
                }

                MessageBox.Show("Memberships Done");
            }
            catch (Exception ex)
            {
                NLog.LogError("Redmine", "DBconnection", "SelectMembershipsAsync", ex);
            }
        }

        public List<Memberships> ListMemberships(string project_id, string new_id)
        {
            try
            {
                string query = "SELECT * FROM members WHERE project_id=" + project_id;

                //Open connection
                if (this.OpenConnection() == true)
                {
                    //Create Command
                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    //Create a data reader and Execute the command
                    MySqlDataReader dataReader = cmd.ExecuteReader();
                    List<Memberships> mList = new List<Memberships>();

                    while (dataReader.Read())
                    {
                        Memberships Member = new Memberships();
                        Member.member_id = dataReader["id"].ToString();
                        Member.project_id = new_id;
                        Member.user_id = dataReader["user_id"].ToString();
                        Member.user_id = users[int.Parse(Member.user_id)];
                        if (Member != null)
                        {
                            mList.Add(Member);
                        }
                    }
                    dataReader.Close();
                    this.CloseConnection();


                    return mList;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                NLog.LogError("Redmine", "DBconnection", "ListMemberships", ex);
                return null;
            }
        }
        public List<Memberships> ListRoles(string project_id, List<Memberships> mList)
        {
            try
            {
                List<Memberships> totalList = new List<Memberships>();
                foreach (Memberships m in mList)
                {
                    string query = "SELECT * FROM member_roles WHERE member_id=" + m.member_id;

                    //Open connection
                    if (this.OpenConnection() == true)
                    {
                        //Create Command
                        MySqlCommand cmd = new MySqlCommand(query, connection);
                        //Create a data reader and Execute the command
                        MySqlDataReader dataReader = cmd.ExecuteReader();
                        Memberships Member = m;
                        role Role = new role();
                        while (dataReader.Read())
                        {
                            string role = dataReader["role_id"].ToString();
                            if(role.Equals("7")||role.Equals("6"))
                            {
                                role = "11";
                            }
                            Role.role_id = getRole(Role.role_id, role);

                        }
                        Role.type = "array";
                        Member.role_ids = Role;
                        totalList.Add(Member);
                        dataReader.Close();
                        this.CloseConnection();

                    }
                }
                return totalList;
            }
            catch (Exception ex)
            {
                NLog.LogError("Redmine", "DBconnection", "ListRoles", ex);
                return null;
            }
        }

        public string getRole(string old, string newrole)
        {
            try
            {
                string role;
                if (old == null)
                {
                    role = newrole; //cap rol
                }
                else if (newrole.Equals("11"))
                {
                    role = newrole;
                }
                else if (newrole.Equals("3"))
                {
                    if (old.Equals("11"))
                    {
                        role = old;
                    }
                    else
                    {
                        role = newrole;
                    }
                }
                else if (newrole.Equals("4"))
                {
                    if (old.Equals("11") || old.Equals("3"))
                    {
                        role = old;
                    }
                    else
                    {
                        role = newrole;
                    }
                }
                else
                {
                    role = "4";
                }
                return role;
            }
            catch(Exception ex)
            {
                NLog.LogError("Redmine", "DBconnection", "getRole", ex);
                return "4";
            }
        }


    }

}