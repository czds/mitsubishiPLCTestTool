using Microsoft.Data.SqlClient;

namespace MauiCSharpInteropWebView.Fun
{
    public static class AidTool
    {
        public static string SqlErrorMsg = "";
        public static string SqlConnectionString = "";
        public class SqlDataReaderResult
        {
            public bool successed { get; set; }
            public System.Data.DataTable dataTable { get; set; }
            public string errMsg { get; set; }
        }

        //连接池方式操作数据库查询
        public static SqlDataReaderResult vra_pool_open(string queryString)
        {
            SqlDataReaderResult sqlResult = new SqlDataReaderResult();

            using (SqlConnection connection = new SqlConnection("Initial Catalog=xsqlpool1;;Max Pool Size=50; Min Pool Size=1;" + SqlConnectionString))
            {

                try { connection.Open(); }
                catch (Exception ex)
                {
                    sqlResult.successed = false;
                    sqlResult.errMsg = "数据库连接失败: " + ex.Message;

                    return sqlResult;
                };

                SqlDataAdapter sqlda = new SqlDataAdapter(queryString, connection);
                System.Data.DataSet sqlds = new System.Data.DataSet();

                try
                {
                    //缓存查询结果
                    sqlda.Fill(sqlds, "xdata");
                    sqlResult.dataTable = sqlds.Tables["xdata"];
                }
                catch (Exception ex)
                {
                    sqlResult.errMsg = ex.Message;
                    sqlResult.successed = false;
                    return sqlResult;
                }

                sqlResult.successed = true;
                return sqlResult;
            }
            //根据返回结果里面的successed判断查询是否成功

        }

        public static bool vra_pool_exec(string queryString)
        {
            //连接字符串 Initial Catalog=xsqlpool1; 中的 xsqlpool1 就是连接池名,不同名就代表新开另一个连接池
            using (SqlConnection connection = new SqlConnection("Integrated Security=SSPI;Initial Catalog=xsqlpool1;;Max Pool Size=50; Min Pool Size=1;" + SqlConnectionString))
            {

                try
                {
                    //不缓存查询结果，直接执行
                    new SqlCommand(queryString, connection).ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    //记录错误内容
                    SqlErrorMsg = ex.Message;
                    return false;
                }

                return true;
            }

        }
    }
}
