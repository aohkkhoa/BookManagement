using BookManagement2.Models.DTO;
using Microsoft.Data.SqlClient;
using System.Data;

namespace BookManagement2.DAL
{
    public class BookDAL : _BaseDAL
    {
        public BookDAL(IConfiguration config) : base(config)
        {
            
        }


        public List<BookDetail> GetHigh()
        {
            List<BookDetail> data = new List<BookDetail>();
            using (SqlConnection cn = GetConnection())
            {
                SqlCommand cmd = cn.CreateCommand();
                cmd.CommandText = @"SELECT b.bookId,b.title, c.CategoryName
                                    FROM Book b
                                    INNER JOIN category c
                                    ON b.CategoryId = c.CategoryId";
                cmd.CommandType = System.Data.CommandType.Text;
                using (SqlDataReader dbReader = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                {
                    while (dbReader.Read())
                    {
                        data.Add(new BookDetail()
                        {
                            bookDetailId = Convert.ToInt32(dbReader["bookId"]),
                            title = Convert.ToString(dbReader["title"]),
                            categoryName = Convert.ToString(dbReader["CategoryName"]),
                        });
                    }
                }
                cn.Close();
            }
            return data;
        }
    }
}
