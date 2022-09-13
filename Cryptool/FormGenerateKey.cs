using System;
using System.Threading;
using System.Windows.Forms;

namespace Cryptool
{
    public partial class FormGenerateKey : Form
    {
        private bool stopThread = false;
        private bool generated = false;
        private readonly char[] b64 =
        {
            'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H',
            'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P',
            'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X',
            'Y', 'Z', 'a', 'b', 'c', 'd', 'e', 'f',
            'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n',
            'o', 'p', 'q', 'r', 's', 't', 'u', 'v',
            'w', 'x', 'y', 'z', '0', '1', '2', '3',
            '4', '5', '6', '7', '8', '9', '/', '+'
            };

        public FormGenerateKey()
        {
            InitializeComponent();
        }
        private void FormGenerateKey_Load(object sender, EventArgs e)
        {
            new Thread(ThreadGen).Start();
        }
        private void FormGenerateKey_FormClosing(object sender, FormClosingEventArgs e)
        {
            stopThread = true;
        }

        internal string[] Generate()
        {
            this.ShowDialog();
            if (generated)
            {
                return new string[]
                {
                TB_key.Text,
                TB_salt.Text
                };
            }
            else
            {
                return null;
            }
        }

        #region thread generate
        private void ThreadGen()
        {
            while (!stopThread)
            {
                char[] salt = new char[28];
                char[] key = new char[32];
                for (int i = 0; i < 28; i++)
                {
                    Random random = new Random();
                    Thread.Sleep(14);                    
                    salt[i]=b64[random.Next(0, 64)];
                    Thread.Sleep(1);
                }
                for (int i = 0; i < 32; i++)
                {
                    Random random = new Random();
                    Thread.Sleep(14);
                    key[i] = b64[random.Next(0, 64)];
                    Thread.Sleep(1);
                }
                Action action = delegate
                {
                    TB_key.Text = string.Join("", key);
                    TB_salt.Text = string.Join("", salt);
                };
                if (!stopThread) Invoke(action);
            }
        }
        #endregion

        private void BT_Register_Click(object sender, EventArgs e)
        {
            stopThread = true;
            Cursor.Current = Cursors.WaitCursor;
            if (TB_Password.Text == "" || TB_Password.Text == null)
            {
                MessageBox.Show("Informe uma senha");
            }
            else
            {
                Thread.Sleep(900);
                generated = CryptoDb.ValidateAndSave
                    (
                    TB_Password.Text,
                    TB_salt.Text,
                    TB_key.Text
                    );
                Cursor.Current = Cursors.Default;
                this.Close();
            }
            Cursor.Current = Cursors.Default;
        }

        private void TB_Password_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                BT_Register.PerformClick();
            }
        }
    }
}
