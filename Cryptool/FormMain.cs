using System;
using System.IO;
using System.Xml.Linq;
using System.Threading;
using System.Diagnostics;
using System.Windows.Forms;
using System.Collections.Generic;

namespace Cryptool
{
    public partial class FormMain : Form
    {
        private delegate void DelegatePrintProcess(string text);
        private readonly DelegatePrintProcess delegatePrintProcess;
        private delegate void DelegateEnableRun(bool enable);
        private readonly DelegateEnableRun delegateEnableRun;
        private readonly List<int> Pids = new List<int>();
        private string[] ListProcess;

        public FormMain()
        {
            InitializeComponent();
            delegatePrintProcess = DelPrintProcess;
            delegateEnableRun = DelEnableRun;
        }

        private void DelEnableRun(bool enable)
        {
            Button_Run.Enabled = enable;
        }

        private void DelPrintProcess(string text)
        {
            TextBox_ProcessCount.Text = text;
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            XElement config = XElement.Parse(File.ReadAllText(Application.StartupPath + "\\config.xml"));
            List<string> pathproc = new List<string>();
            foreach (XElement X in config.Element("ListProcess").Elements("Process"))
            {
                ComboBox_Processes.Items.Add(X.Attribute("name").Value);
                pathproc.Add(Application.StartupPath + "\\" + X.Attribute("file").Value);
            }
            ListProcess = pathproc.ToArray();
        }

        private void BT_AddSource_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                Title = "Add File",
                Multiselect = true
            };
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                foreach (string S in openFileDialog.FileNames)
                {
                    listBox1.Items.Add(S);
                }
            }
        }

        private void BT_Run_Click(object sender, EventArgs e)
        {
            Invoke(delegatePrintProcess, "beginning...");
            Invoke(delegateEnableRun, false);
            Cursor.Current = Cursors.WaitCursor;
            this.TopMost = true;
            foreach (string source in listBox1.Items)
            {
                string destiny = TB_Destitny.Text + "\\" + Path.GetFileNameWithoutExtension(source) + TB_Extension.Text;
                string[] args = {
                    CB_Cmd.Text,
                    "\""+source+"\"",
                    "\""+destiny+"\"",
                    TB_cryptosalt.Text,
                    TB_cryptokey.Text
                    };
                Process P = Process.Start(
                    ListProcess[ComboBox_Processes.SelectedIndex], 
                    string.Join(" ", args)
                    );
                Pids.Add(P.Id);
            }
            this.TopMost = false;
            new Thread(Thread_MonitorProcess).Start();
            Cursor.Current = Cursors.Default;
            listBox1.Items.Clear();
        }

        private void Thread_MonitorProcess()
        {
            Invoke(delegatePrintProcess, Pids.Count.ToString() + " processes running");
            while (Pids.Count > 0)
            {
                foreach (int pid in Pids)
                {
                    try
                    {
                        Process.GetProcessById(pid);
                    }
                    catch
                    {
                        Pids.Remove(pid);
                        Invoke(delegatePrintProcess, Pids.Count.ToString() + " processes running");
                        break;
                    }
                }
            }
            Invoke(delegatePrintProcess, "finished");
            Invoke(delegateEnableRun, true);
        }

        private void BT_Abort_Click(object sender, EventArgs e)
        {
            int[] Ppids = Pids.ToArray();
            foreach (int pid in Ppids)
            {
                try { Process.GetProcessById(pid).Kill(); } catch { }
            }
        }

        private void BT_SearchDestiny_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                TB_Destitny.Text = folderBrowserDialog.SelectedPath;
            }
            listBox1.Items.Clear();
        }

        private void BT_GenKey_Click(object sender, EventArgs e)
        {
            FormGenerateKey genKey = new FormGenerateKey();
            string[] Gen = genKey.Generate();
            if (Gen != null)
            {
                TB_cryptokey.Text = Gen[0];
                TB_cryptosalt.Text = Gen[1];
            }
        }

        private void BT_OpenKey_Click(object sender, EventArgs e)
        {
            FormOpenKey openKey = new FormOpenKey();
            string[] Open = openKey.Open();
            if (Open != null)
            {
                TB_cryptokey.Text = Open[0];
                TB_cryptosalt.Text = Open[1];
            }
        }

        #region 
        private readonly bool[] enableRun = { false, false, true };
        private void CB_Processes_SelectedIndexChanged(object sender, EventArgs e)
        {
            enableRun[0] = true;
            if (enableRun[0] && enableRun[1] && enableRun[2])
            {
                Button_Run.Enabled = true;
                BT_Abort.Enabled = true;
                enableRun[2] = false;
            }
        }

        private void CB_Cmd_SelectedIndexChanged(object sender, EventArgs e)
        {
            enableRun[1] = true;
            if (enableRun[0] && enableRun[1] && enableRun[2])
            {
                Button_Run.Enabled = true;
                BT_Abort.Enabled = true;
                enableRun[2] = false;
            }
        }

        private void BT_Clear_Click(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
        }

        private void ListBox1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            listBox1.Items.RemoveAt(listBox1.SelectedIndex);
        }

        private void ListBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                listBox1.Items.RemoveAt(listBox1.SelectedIndex);
            }
        }
        #endregion
    }
}
