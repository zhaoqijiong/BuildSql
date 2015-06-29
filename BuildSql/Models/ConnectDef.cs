using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace BuildSql.Models {
    [Serializable]
    public class ConnectDef {

        public string Type { get; set; }

        public string Source { get; set; }

        public string Account { get; set; }

        public string Pass { get; set; }
    }

    [Serializable]
    public class ConnectDefs {
        public List<ConnectDef> Defines = new List<ConnectDef>();

        public void Save() {
            var s = new XmlSerializer(typeof(ConnectDefs));

            var configXml = "config.xml";

            using (var fs = new System.IO.FileStream(configXml, FileMode.OpenOrCreate)) {
                s.Serialize(fs, this);
            }


        }
    }

    public class ColumnDef {
        public string ColumnName = string.Empty;

        public string ColumnType = string.Empty;

        public int Length = 0;

        public bool AllowNull = false;

    }
}
