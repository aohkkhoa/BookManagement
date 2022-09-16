using BookManagement2.Models.DTO;
using Microsoft.Data.SqlClient;
using System.Data;

namespace BookManagement2.DAL
{
    public class Test :_BaseDAL
    {
        public Test(IConfiguration config) : base(config)
        {
        }

        public List<BookDetail> List(int page, int pageSize, string searchValue)
        {
            if (searchValue != "")
                searchValue = "%" + searchValue + "%";

            List<BookDetail> data = new List<BookDetail>();
            using (SqlConnection cn = GetConnection())
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = @"SELECT *
                    FROM( 
                        SELECT b.bookId, b.title, c.categoryName, ROW_NUMBER() OVER(ORDER BY title) AS RowNumber 
                         FROM book b
                            INNER JOIN category c
                            ON b.CategoryId = c.CategoryId
                            WHERE(@searchValue = '')
                            OR( title LIKE @searchValue)) AS s
                            WHERE s.RowNumber BETWEEN(@page -1)*@pageSize + 1 AND @page*@pageSize";
                cmd.CommandType = CommandType.Text;
                cmd.Connection = cn;

                cmd.Parameters.AddWithValue("@page", page);
                cmd.Parameters.AddWithValue("@pageSize", pageSize);
                cmd.Parameters.AddWithValue("@searchValue", searchValue);

                using (SqlDataReader dbReader = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                {                                                                   
                    while (dbReader.Read())
                    {

                        data.Add(new BookDetail()
                        {
                            bookDetailId = Convert.ToInt32(dbReader["bookId"]),
                            title = Convert.ToString(dbReader["title"]),
                            categoryName = Convert.ToString(dbReader["categoryName"]),
                        }
                        );
                    }
                }

                cn.Close();
            }
            return data;
        }
        public int Count(string searchValue)
        {
            if (searchValue != "")
                searchValue = "%" + searchValue + "%";
            int result = 0;

            using (SqlConnection cn = GetConnection())
            {
                SqlCommand cmd = cn.CreateCommand();
                cmd.CommandText = @"SELECT COUNT(*) FROM book
                                    WHERE (@searchValue = '')
                                       OR ( title LIKE @searchValue)";
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@searchValue", searchValue);

                result = Convert.ToInt32(cmd.ExecuteScalar());

                cn.Close();
            }

            return result;
        }
    }
}
