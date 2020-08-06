using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using Ubiety.Dns.Core.Records;

namespace migracioRedmine
{
    class DBconnection
    {
        private MySqlConnection connection;
        private string server;
        private string database;
        private string uid;
        private string password;

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
            List<Project> pList = new List<Project>();
            pList = ListProjects();

            List<Tuple<string, string>> tIds = new List<Tuple<string, string>>();
            TextWriter tw = new StreamWriter("projects.txt");
            foreach (Project oprg in pList)
            {
                Tuple<string, string> ids = new Tuple<string, string>(oprg.id, await Api.PostProject(oprg, tIds));
                tIds.Add(ids);
                tw.WriteLine(ids.Item1 + " " + ids.Item2);
                InsertIdProject(ids.Item2, ids.Item1);
            }
            tw.Close();
            MessageBox.Show("ProjectsCreated");

        }

        public bool InsertIdProject(string new_id, string old_id)
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

        public List<Project> ListProjects()
        {
            string query = "SELECT * FROM projects WHERE id<100";

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
    }

}