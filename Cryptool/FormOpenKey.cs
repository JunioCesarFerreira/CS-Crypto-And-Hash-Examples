using System;
using System.Windows.Forms;

namespace Cryptool
{
    public partial class FormOpenKey : Form
    {
        private string[] Result = null;

        public FormOpenKey()
        {
            InitializeComponent();
        }

        internal string[] Open()
        {
            this.ShowDialog();
            return Result;
        }

        private void BT_Open_Click(object sender, EventArgs e)
        {
            Result = CryptoDb.Open(TB_Password.Text);
            this.Close();
        }

        private void TB_Password_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                BT_Open.PerformClick();
            }
        }

        private void FormOpenKey_Load(object sender, EventArgs e)
        {
            TB_Password.Focus();
        }
    }
}
