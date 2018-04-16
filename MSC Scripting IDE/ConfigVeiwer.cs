using MSC;
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
    public partial class ConfigVeiwer : Form
    {
        public Config Config;
        public ConfigVeiwer(Config config)
        {
            Config = config;
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Config.LoginURL = textBox1.Text;
            Config.Referer = textBox2.Text;
            Config.UserAgent = textBox4.Text;
            Config.ContectType = textBox3.Text;
            Config.AllowAutoRedirect = checkBox3.Checked;
            Config.Headers = textBox5.Text;
            Config.Cookies = textBox8.Text;
            Config.PostData = textBox7.Text;
            Config.DataSet = textBox10.Text;
            Config.KeepAlive = checkBox1.Checked;
            Config.TimeOut = int.Parse(textBox12.Text);
            Config.DecompressionGZip = checkBox2.Checked;
            DialogResult = DialogResult.OK;
            Close();
        }
        private void ConfigVeiwer_Load(object sender, EventArgs e)
        {
            textBox1.Text = Config.LoginURL;
            textBox2.Text = Config.Referer;
            textBox3.Text = Config.ContectType;
            textBox4.Text = Config.UserAgent;
            textBox5.Text = Config.Headers;
            textBox7.Text = Config.PostData;
            textBox8.Text = Config.Cookies;
            textBox10.Text = Config.DataSet;
            textBox12.Text = Config.TimeOut.ToString();
            checkBox1.Checked = Config.KeepAlive;
            checkBox2.Checked = Config.DecompressionGZip;
            checkBox3.Checked = Config.AllowAutoRedirect;
        }
        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }


    }
}
