using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BuildSql.Models;

namespace BuildSql {
    class SqlHelper {
        public static bool TestConnect(string source, string account, string pass) {
            try {
                var connectionString = string.Format("Provider=SQLOLEDB.1;Password={0};Persist Security Info=True;User ID={1};Initial Catalog=master;Data Source={2}", pass,
                    account, source);
                OleDbConnection oleDbConnection = new OleDbConnection(connectionString);
                oleDbConnection.Open();

                oleDbConnection.Close();

                return true;
            } catch (Exception ex) {
                MessageBox.Show(ex.ToString());

                return false;
            }
        }

        public static DataTable Select(string sql, string connectionString) {
            DataSet dataSet = new DataSet();
            try {
                new OleDbDataAdapter(sql, connectionString).Fill(dataSet);
                return dataSet.Tables[0];
            } catch {
                return (DataTable)null;
            }
        }

        public static List<string> GetAllDatabase(string connectionString) {
            var s = new List<string>();

            DataTable dataTable = Select("SELECT Name FROM Master..SysDatabases ORDER BY Name", connectionString);
            foreach (DataRow dataRow in (InternalDataCollectionBase)dataTable.Rows) {
                s.Add(dataRow[0].ToString());
            }

            return s;
        }

        public static List<string> GetAllTables(string dataBaseName, string connectionString) {
            DataTable dataTable = Select("SELECT name,xtype From " + dataBaseName + ".dbo.sysobjects WHERE xtype = 'u' or xtype='v' ORDER BY xtype,name", connectionString);

            var s = (from DataRow dataRow in (InternalDataCollectionBase) dataTable.Rows select dataRow[0].ToString()).ToList();

            return s;
        }

        public static List<ColumnDef> GetAllColumn(string tableName, string dataBaseName, string connectionString) {
            DataTable dataTable = Select("SELECT o.name AS table_name, c.name AS column_name,t.name AS data_type ,c.length AS data_length, c.isnullable AS ifisnull  FROM  " + dataBaseName + ".dbo.syscolumns c INNER JOIN " + dataBaseName + ".dbo.sysobjects o ON o.id = c.id INNER JOIN " + dataBaseName + ".dbo.systypes t ON t.xusertype = c.xusertype WHERE o.name='" + tableName + "' ", connectionString);
            
            var s = (from DataRow n in (InternalDataCollectionBase)dataTable.Rows select new ColumnDef() {
                ColumnName = n[1].ToString(),
                ColumnType = n[2].ToString(),
                Length = int.Parse(n[3].ToString()),
                AllowNull = n[2].ToString().Equals("0")
            }).ToList();

            return s;
        }

        public static string MakeConnectStr(string source, string account, string pass) {
            return string.Format("Provider=SQLOLEDB.1;Password={0};Persist Security Info=True;User ID={1};Initial Catalog=master;Data Source={2}", pass,
                    account, source);
        }

        static public string MakeConnectStr(ConnectDef def) {
            return string.Format("Provider=SQLOLEDB.1;Password={0};Persist Security Info=True;User ID={1};Initial Catalog=master;Data Source={2}", def.Pass,
                    def.Account, def.Source);
        }

        
    }
}
