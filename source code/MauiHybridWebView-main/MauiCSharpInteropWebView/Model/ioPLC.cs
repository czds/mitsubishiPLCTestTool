using SQLite;
using static MauiCSharpInteropWebView.Model.dbClass;

namespace MauiCSharpInteropWebView.Model
{
    public static class ioPLC
    {
        private static SQLiteAsyncConnection db;

        public static async Task Init()
        {
            if (db != null)
                return;

            var databasePath = Path.Combine(FileSystem.AppDataDirectory, "MyPLCData.db");
            var options = new SQLiteConnectionString(databasePath, true, key: "pwd12321");
            db = new SQLiteAsyncConnection(options);
            await db.CreateTableAsync<PLC_Addr>();
            await db.CreateTableAsync<PLC_Variable>();
        }

        public static async Task AddNewPLC(string ipAddr, int port, int slot)
        {
            await Init();

            var plc = new PLC_Addr { };
            var id = await db.InsertAsync(plc);
        }

        public static async Task RemovePLC(string ipAddr, int id)
        {
            await Init();
            await db.DeleteAsync<PLC_Addr>(id);
        }

        public static async Task<IEnumerable<PLC_Addr>> GetPLC()
        {
            await Init();
            var plc = await db.Table<PLC_Addr>().ToListAsync();
            return plc;


            // 查询
            //var users = db.Table<User>().Where(u => v.Name.StartsWith("张")).ToList();
            //db.Execute("insert into User(Name) values (?)", "李四");
            //db.Query<User>("select * from User where Id = ?", 3);

            //public class Val
            //{
            //    public decimal Money { get; set; }
            //    public DateTime Date { get; set; }
            //}


            // db.Query<Val>("select \"Price\" as \"Money\", \"Time\" as \"Date\" from Valuation where StockId = ?", stock.Id);

        }




        public static async Task AddNewVariableItem(string ipAddr, int port, int slot)
        {
            await Init();
        }

        public static async Task RemoveVariableItem(string ipAddr, int port, int slot)
        {
            await Init();
        }


    }


}
