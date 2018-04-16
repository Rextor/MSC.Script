using MSC.Brute;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MSC_Scripting_IDE
{
    public partial class EditRequest : Form
    {
        public RequestManage manage;
        public EditRequest(RequestManage _manage)
        {
            manage = _manage;
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void EditRequest_Load(object sender, EventArgs e)
        {
            richTextBox1.Text = manage.SourcePage;
            textBox12.Text = manage.CookiesString;
            if (manage.ErrorAst)
                label1.Text = "YES";
            else label1.Text = "NO";
            label2.Text = manage.StatusCode.ToString();
            textBox2.Text = manage.Headers.ToString();
            textBox3.Text = manage.Location;
            textBox1.Text = manage.RedirectedUrl;
        }
    }
}
