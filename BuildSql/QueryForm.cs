using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BuildSql {
    public partial class QueryForm : Form {
        public QueryForm() {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e) {
            DialogResult = DialogResult.OK;

            Close();
        }

        public string Sql {
            get {
                return txtSql.Text;
            }
            
        }
    }
}
