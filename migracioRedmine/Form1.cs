using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace migracioRedmine
{
    public partial class Form1 : Form
    {
        private DBconnection dbConnect;
        public Form1()
        {
            InitializeComponent();
            dbConnect = new DBconnection();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            dbConnect.SelectProjectAsync();
        }
    }
}
