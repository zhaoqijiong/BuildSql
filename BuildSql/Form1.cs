using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using System.Xml.Serialization;
using BuildSql.Models;
using BuildSql.Windows;

namespace BuildSql {
    public partial class Form1 : Form {
        private ConnectDefs connectDefs = new ConnectDefs();
        private TreeNode currentTreeNode;

        public Form1() {
            InitializeComponent();

            GetConfig();
        }

        private void GetConfig() {
            var s = new XmlSerializer(typeof(ConnectDefs));

            var configXml = "config.xml";

            if (!File.Exists(configXml)) {
                using (var fs = new System.IO.FileStream(configXml, FileMode.OpenOrCreate)) {
                    s.Serialize(fs, connectDefs);
                }

            }

            UpdateTree(configXml, s);
        }

        private void UpdateTree(string configXml, XmlSerializer s) {
            TreeSql.Nodes.Clear();

            using (var fs = new System.IO.FileStream(configXml, FileMode.Open)) {
                connectDefs = s.Deserialize(fs) as ConnectDefs;
                if (connectDefs != null)
                    foreach (var connectDef in connectDefs.Defines) {
                        var treeNode = new TreeNode(connectDef.Source) {Tag = connectDef};
                        TreeSql.Nodes.Add(treeNode);

                    }
            }
        }


        private void exitToolStripMenuItem_Click(object sender, EventArgs e) {
            Application.Exit();
        }

        private void OnClickAddDb(object sender, EventArgs e) {
            var f = new AdDbForm();
            var c = f.ShowDialog();

            if (c == DialogResult.OK) {
                connectDefs.Defines.Add(f.GetDefine());
                connectDefs.Save();
            }

        }

        private void TreeSql_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e) {
            var d = e.Node.Tag;

            if (d is ConnectDef) {
                OnClickConnectDef(d as ConnectDef, e.Node);
            }

            if (d is DbNode) {
                OnClickDbNode(d as DbNode, e.Node);
            }

            if (d is TableNode) {
                OnClickTableNode(d as TableNode, e.Node);
            }
        }

        private void OnClickTableNode(TableNode tableNode, TreeNode n) {
            var r = SqlHelper.GetAllColumn(tableNode.TableName, tableNode.DbName, SqlHelper.MakeConnectStr(tableNode.Def));
            foreach (var db in r) {

                var s = string.Format("{0} {1} [{2}]", db.ColumnName, db.ColumnType, db.Length);
                var treeNode = new TreeNode(s) ;
                n.Nodes.Add(treeNode);
            }

            TxtCode.Text = OutEntityCClass(tableNode.TableName, r);


            if (!n.IsExpanded) {
                n.Expand();
            } 
        }

        private void OnClickDbNode(DbNode dbNode, TreeNode n) {
            var r = SqlHelper.GetAllTables(dbNode.DbName,SqlHelper.MakeConnectStr(dbNode.Def));
            foreach (var db in r) {
                var data = new TableNode() {
                    DbName = dbNode.DbName,
                    TableName = db,
                    Def = dbNode.Def
                };

                var treeNode = new TreeNode(db) { Tag = data };
                n.Nodes.Add(treeNode);
            }

            if (!n.IsExpanded) {
                n.Expand();
            }
        }

        private void OnClickConnectDef(ConnectDef d, TreeNode n) {
            

            var r = SqlHelper.GetAllDatabase(SqlHelper.MakeConnectStr(d));
            foreach (var db in r) {
                var data = new DbNode() {
                    DbName = db,
                    Def = d
                };

                var treeNode = new TreeNode(db) {Tag = data};


                n.Nodes.Add(treeNode);
            }

            if (!n.IsExpanded) {
                n.Expand();
            }
        }

        class DbNode {
            public string DbName = string.Empty;
            public ConnectDef Def = null;
        }

        class TableNode {
            public string DbName = string.Empty;
            public string TableName = string.Empty;
            public ConnectDef Def = null;
        }

        private void TreeSql_MouseDown(object sender, MouseEventArgs e) {
            if (e.Button == MouseButtons.Right) {
                Point p = new Point(e.X, e.Y);
                currentTreeNode = TreeSql.GetNodeAt(p);

                if (currentTreeNode != null) {
                    if (currentTreeNode.Tag is TableNode) {
                        contextMenuStrip1.Show(TreeSql.PointToScreen(p));
                        
                    }
                }
            }
        }

        private void ToolStripMenuItem_Click(object sender, EventArgs e) {
            var tableNode = currentTreeNode.Tag as TableNode;

            var r = SqlHelper.GetAllColumn(tableNode.TableName, tableNode.DbName, SqlHelper.MakeConnectStr(tableNode.Def));
            TxtCode.Text = OutEntityCClass(tableNode.TableName, r);
        }

        public string OutEntityCClass(string tableName, List<ColumnDef> controlTexts) {
            var sb = new StringBuilder();

            sb.AppendFormat("/// <summary>\r\n/// 数据库表{0}所对应的实体类\r\n/// </summary>\r\npublic class {0}Entity\r\n{{\r\n", tableName);

            foreach (var s in controlTexts) {
                sb.AppendFormat("\t/// <summary>\r\n\t///设置或返回值{0}\r\n\t/// </summary>\r\n", s.ColumnName);
                sb.AppendFormat("\tpublic {1} {0} {{ get; set; }}\r\n\r\n", s.ColumnName, SqlTypeToSharpType(s.ColumnType));
            }
            
            sb.Append("}\r\n");
            return sb.ToString();
        }

        public string SqlTypeToSharpType(string type) {
            switch (type.ToLower()) {
                case "int":
                    return "Int32";
                case "text":
                    return "String";
                case "bigint":
                    return "Int64";
                case "binary":
                    return "System.Byte[]";
                case "bit":
                    return "Boolean";
                case "char":
                    return "String";
                case "datetime":
                    return "System.DateTime";
                case "decimal":
                    return "System.Decimal";
                case "float":
                    return "System.Double";
                case "image":
                    return "System.Byte[]";
                case "money":
                    return "System.Decimal";
                case "nchar":
                    return "String";
                case "ntext":
                    return "String";
                case "numeric":
                    return "System.Decimal";
                case "nvarchar":
                    return "String";
                case "real":
                    return "System.Single";
                case "smalldatetime":
                    return "System.DateTime";
                case "smallint":
                    return "Int16";
                case "smallmoney":
                    return "System.Decimal";
                case "timestamp":
                    return "System.DateTime";
                case "tinyint":
                    return "System.Byte";
                case "uniqueidentifier":
                    return "System.Guid";
                case "varbinary":
                    return "System.Byte[]";
                case "varchar":
                    return "String";
                case "variant":
                    return "Object";
            }

            return "";
        }

        private void TxtCode_MouseDoubleClick(object sender, MouseEventArgs e) {
            Clipboard.SetDataObject(TxtCode.Text); 
        }
    }
}
