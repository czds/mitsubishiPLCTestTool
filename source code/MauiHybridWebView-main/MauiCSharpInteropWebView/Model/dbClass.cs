using SQLite;
using System.Net;

namespace MauiCSharpInteropWebView.Model
{
    public class dbClass
    {
        public class PLC_Addr
        {
            [PrimaryKey, AutoIncrement]
            public int Id { get; set; }

            public IPAddress IP { get; set; }

            public string Type { get; set; }
        }

        public class PLC_Variable
        {
            [PrimaryKey, AutoIncrement]
            public int Id { get; set; }

            [Indexed]
            public string Remark { get; set; }

            public string Addr { get; set; }

            public int Type { get; set; }

            public int Length { get; set; }


            public string Value { get; set; }

            public string WriteValue { get; set; }

            public DateTime Time { get; set; }

        }

    }
}
