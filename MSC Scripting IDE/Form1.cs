using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MSC.Script;
using MSC;
using System.Threading;
using MSC.Brute;

namespace MSC_Scripting_IDE
{
    struct DoWork
    {
        public Instruction inst;
        public MethodType type;
    }
    public partial class Form1 : Form
    {
        MSC.Script.Body Body;
        MSC.Logger Logger;
        MethodParser Parser;

        public Form1()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                Clipboard.SetText(listView1.SelectedItems[0].SubItems[1].Text);
            }catch { }
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            if(listView1.SelectedItems.Count <= 0)
            {
                ((ContextMenuStrip)sender).Items[0].Enabled = false;
                ((ContextMenuStrip)sender).Items[1].Enabled = false;
            }
            else
            {
                ((ContextMenuStrip)sender).Items[0].Enabled = true;
                ((ContextMenuStrip)sender).Items[1].Enabled = true;
            }
        }

        private void richTextBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
            {
                //CompileScript();
            }
        }

        private bool CompileScript()
        {
            string[] lines = richTextBox1.Lines;
            Body = new MSC.Script.Body();
            Logger = new MSC.Logger();
            Logger.OnMessageReceived += Logger_OnMessageReceived;
            richTextBox1.Clear();
            richTextBox2.Clear();
            treeView1.Nodes.Clear();
            try
            {
                Body.Initialize(lines);
                foreach (MSC.Script.Method method in Body.Methods)
                {
                    List<TreeNode> nodes = new List<TreeNode>();
                    foreach (MSC.Script.Instruction inst in method.Instructions)
                    {
                        TreeNode node = new TreeNode(inst.ToString());
                        nodes.Add(node);
                    }
                    TreeNode trnode = new TreeNode(method.IndexPous + "- " + method.Type + "(" + method.Instructions.Count + ")", nodes.ToArray());
                    treeView1.Nodes.Add(trnode);
                }
                richTextBox1.Lines = lines;
                return true;
            }catch (Exception ex)
            {
                Logger.AddMessage(ex.Message, Log.Type.Error);
                if(ex.Message.Contains("Error >"))
                {
                    int errorline = int.Parse(ex.Message.Split(':')[1]);
                    int i = 1;
                    foreach (string item in lines)
                    {
                        if (i == errorline)
                        {
                            richTextBox1.SelectionStart = richTextBox1.TextLength;
                            richTextBox1.SelectionLength = 0;
                            Color temp = richTextBox1.SelectionBackColor;

                            richTextBox1.SelectionBackColor = Color.Red;
                            if (i != lines.Length)
                                richTextBox1.AppendText(item + "\n");
                            else richTextBox1.AppendText(item);
                            richTextBox1.SelectionBackColor = temp;
                        }
                        else
                        {
                            if (i != lines.Length)
                                richTextBox1.AppendText(item + "\n");
                            else richTextBox1.AppendText(item);
                        }
                        i++;
                    }
                }
                return false;
            }
        }
        public void HighlightLine(RichTextBox richTextBox, int index, Color color)
        {
            richTextBox.SelectAll();
            richTextBox.SelectionBackColor = richTextBox.BackColor;
            var lines = richTextBox.Lines;
            if (index < 0 || index >= lines.Length)
                return;
            var start = richTextBox.GetFirstCharIndexFromLine(index);  // Get the 1st char index of the appended text
            var length = lines[index].Length;
            richTextBox.Select(start, length);                 // Select from there to the end
            richTextBox.SelectionBackColor = color;
        }
        private void AppendText(string text, Color color)
        {
            richTextBox2.SelectionStart = richTextBox2.TextLength;
            richTextBox2.SelectionLength = 0;

            richTextBox2.SelectionColor = color;
            richTextBox2.AppendText(text + "\n");
            richTextBox2.SelectionColor = richTextBox2.ForeColor;
        }

        private void Logger_OnMessageReceived(object sender, MSC.MessageReceivedArge e)
        {
            Color col = Color.Black;
            switch (e.log.typeT)
            {
                case Log.Type.Error:
                    col = Color.Red;
                    break;
                case Log.Type.Infomation:
                    col = Color.Blue;
                    break;
                case Log.Type.OutPut:
                    col = Color.Green;
                    break;
            }
            AppendText(e.log.GetMessage(true), col);
        }
        Thread th;
        private void button1_Click(object sender, EventArgs e)
        {
            if (CompileScript())
            {
                Logger.AddMessage("Methods installed");
                Parser = new MethodParser();
                Parser.CMD = new CommendLine();
                Parser.CMD.OutPuter.OnMessageReceived += Logger_OnMessageReceived;
                th = new Thread(new ThreadStart(StartDebugger));
                th.IsBackground = true;
                th.Start();
                button1.Enabled = false;
                button2.Enabled = true;
                button3.Enabled = true;
                richTextBox1.ReadOnly = true;
            }
        }
        private void StartDebugger()
        {
            ErrorExp = false;
            foreach(MSC.Script.Method method in Body.Methods)
            {
                if (ErrorExp)
                    return;
                switch (method.Type)
                {
                    case MethodType.Config:
                        Parser.Controller.NewConfigDef();
                        Logger.AddMessage("New configdef created");
                        ExecuteInstructions(method);
                        break;
                    case MethodType.Request:
                        Parser.Controller.NewRequestDef();
                        Logger.AddMessage("New requestdef created");
                        ExecuteInstructions(method);
                        break;
                    case MethodType.Print:
                    case MethodType.Base:
                        ExecuteInstructions(method);
                        break;
                }
            }
            StopDebugger();
        }
        bool Waitforestep = true;
        DoWork work;
        int onindex = 0;
        bool ErrorExp = false;
        public void ExecuteInstructions(MSC.Script.Method method)
        {
            int i = 1;
            foreach (Instruction inst in method.Instructions)
            {
                Waitforestep = true;
                label1.Text = "Current instruction: (" + i + ") " + inst.ToString();
                work.inst = inst;
                work.type = method.Type;
                button3.Enabled = true;
                HighlightLine(richTextBox1, inst.LineIndex, Color.FromArgb(171, 97, 107));
                onindex = inst.LineIndex;
                while (Waitforestep)
                {
                    Thread.Sleep(100);
                }
                button3.Enabled = false;
                try
                {
                    Parser.ExecuteInstruction(work.inst, work.type);
                    HighlightLine(richTextBox1, inst.LineIndex, richTextBox1.BackColor);
                    i++;
                }
                catch(Exception ex)
                {
                    Logger.AddMessage(ex.Message, Log.Type.Error);
                    HighlightLine(richTextBox1, inst.LineIndex, Color.OrangeRed);
                    ErrorExp = true;
                }
                
                UpdateStatus();
            }
        }
        private void StopDebugger()
        {
            try
            {
                if (th.IsAlive)
                {
                    th.Abort();
                    Logger.AddMessage("Debugger thread aborted");
                }
            }catch { }
            Waitforestep = true;
            listView1.Items.Clear();
            listView2.Items.Clear();
            listView3.Items.Clear();
            HighlightLine(richTextBox1, onindex, richTextBox1.BackColor);
            button3.Enabled = false;
            button2.Enabled = false;
            button1.Enabled = true;
            richTextBox1.ReadOnly = false;
            label1.Text = "";
        }

        private void UpdateStatus()
        {
            listView1.Items.Clear();
            foreach (string item in Parser.Controller.MemoryString)
            {
                List<string> row = new List<string>();
                row.Add((listView1.Items.Count + 1).ToString());
                row.Add(item);
                ListViewItem itema = new ListViewItem(row.ToArray());
                listView1.Items.Add(itema);
            }

            listView2.Items.Clear();
            foreach (MSC.Script.ConfigDef item in Parser.Controller.ConfigDefes)
            {
                List<string> row = new List<string>();
                row.Add((listView2.Items.Count + 1).ToString());
                row.Add(item.GetConfig().LoginURL);
                row.Add(item.GetConfig().LoginURL);
                ListViewItem itema = new ListViewItem(row.ToArray());
                listView2.Items.Add(itema);
            }

            listView3.Items.Clear();
            foreach (MSC.Script.RequestDef item in Parser.Controller.RequestDefes)
            {
                List<string> row = new List<string>();
                row.Add((listView3.Items.Count + 1).ToString());
                row.Add(item.GetSourcePage());
                row.Add(item.GetLocation());
                row.Add(item.GetCookies());
                ListViewItem itema = new ListViewItem(row.ToArray());
                listView3.Items.Add(itema);
            }
        }

        private void richTextBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            Waitforestep = false;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            StopDebugger();
        }

        private void editToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            ListViewItem item = listView2.SelectedItems[0];
            Config config = Parser.Controller.ConfigDefes[item.Index].GetConfig();
            ConfigVeiwer co = new MSC_Scripting_IDE.ConfigVeiwer(config);
            if(co.ShowDialog() == DialogResult.OK)
            {
                Parser.Controller.ConfigDefes[item.Index].SetConfig(config);
                UpdateStatus();
            }
        }

        private void editToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ListViewItem item = listView1.SelectedItems[0];
            string value = Parser.Controller.MemoryString[item.Index];
            EditMemoryString co = new MSC_Scripting_IDE.EditMemoryString(value);
            if (co.ShowDialog() == DialogResult.OK)
            {
                Parser.Controller.MemoryString[item.Index] = co.value;
                UpdateStatus();
            }
        }

        private void editToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            ListViewItem item = listView3.SelectedItems[0];
            RequestManage value = Parser.Controller.RequestDefes[item.Index].GetManage();
            EditRequest co = new MSC_Scripting_IDE.EditRequest(value);
            co.ShowDialog();
        }

        private void contextMenuStrip2_Opening(object sender, CancelEventArgs e)
        {
            if (listView2.SelectedItems.Count <= 0)
            {
                ((ContextMenuStrip)sender).Items[0].Enabled = false;
            }
            else
            {
                ((ContextMenuStrip)sender).Items[0].Enabled = true;
            }
        }

        private void contextMenuStrip3_Opening(object sender, CancelEventArgs e)
        {
            if (listView3.SelectedItems.Count <= 0)
            {
                ((ContextMenuStrip)sender).Items[0].Enabled = false;
            }
            else
            {
                ((ContextMenuStrip)sender).Items[0].Enabled = true;
            }
        }
    }
}
