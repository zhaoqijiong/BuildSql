using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BuildSql.Models;

namespace BuildSql.Windows {
    public partial class AdDbForm : Form {
        public AdDbForm() {
            InitializeComponent();

            DialogResult = DialogResult.Cancel;
        }

        private void BtnAdd_Click(object sender, EventArgs e) {
            var b = SqlHelper.TestConnect(txtSource.Text.Trim(), txtAccount.Text.Trim(), txtPass.Text.Trim());
            if (b) {
                DialogResult = DialogResult.OK;
                Close();
            }

           
        }

        public ConnectDef GetDefine() {
            
            return new ConnectDef() {
                Account = txtAccount.Text.Trim(),
                Pass = txtPass.Text.Trim(),
                Source = txtSource.Text.Trim(),
                Type = "SqlServer"
            };
        }

        private void BtnTest_Click(object sender, EventArgs e) {
            var b = SqlHelper.TestConnect(txtSource.Text.Trim(), txtAccount.Text.Trim(), txtPass.Text.Trim());

            MessageBox.Show(b.ToString());
        }
       
    }
}
