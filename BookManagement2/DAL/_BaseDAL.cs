using Microsoft.Data.SqlClient;

namespace BookManagement2.DAL
{
    public abstract class _BaseDAL
    {


        protected readonly IConfiguration _Configuration;


        public _BaseDAL(IConfiguration config)
        {
            _Configuration = config;
        }



        /// <summary>
        /// Tạo và mở kết nối cơ sở dữ liệu
        /// </summary>
        /// <returns></returns>
        protected SqlConnection GetConnection()
        {
            SqlConnection connection = new SqlConnection();
            connection.ConnectionString = _Configuration.GetConnectionString("MyDB");
            connection.Open();
            return connection;
        }
    }
}
