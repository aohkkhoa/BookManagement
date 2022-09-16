using BookManagement2.Models.DTO;
using BookManagement2.Models.EF;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;

namespace BookManagement2.Repository.SQLSERVER
{
    public class BookRepository : _BaseRS,IBookRepository
    {
        private readonly BookDBContext _context;

        public BookRepository(BookDBContext context, IConfiguration config) : base(config)
        {
           _context = context;
        }
        public List<BookDetail> GetAll()
        {
            var bookList = (from b in _context.Books
                            join cate in _context.Categories on b.categoryId equals cate.categoryId
                            select new BookDetail()
                            {
                                bookDetailId = b.bookId,
                                title = b.title,
                                categoryName = cate.categoryName,

                            }).ToList();
            return bookList;
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
                cmd.CommandType = CommandType.Text;
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
        public List<BookDetail> List(int page, int pageSize)
        {
            List<BookDetail> data = new List<BookDetail>();
            using (SqlConnection cn = GetConnection())
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = @"SELECT *
                    FROM( 
                        SELECT b.bookId, b.title, c.categoryName, ROW_NUMBER() OVER(ORDER BY title) AS RowNumber 
                         FROM book b
                            INNER JOIN category c
                            ON b.CategoryId = c.CategoryId) AS s
                            WHERE s.RowNumber BETWEEN(@page -1)*@pageSize + 1 AND @page*@pageSize";
                cmd.CommandType = CommandType.Text;
                cmd.Connection = cn;

                cmd.Parameters.AddWithValue("@page", page);
                cmd.Parameters.AddWithValue("@pageSize", pageSize);

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

        public int Count()
        {
            int result = 0;

            using (SqlConnection cn = GetConnection())
            {
                SqlCommand cmd = cn.CreateCommand();
                cmd.CommandText = @"SELECT COUNT(*) FROM book";
                cmd.CommandType = CommandType.Text;

                result = Convert.ToInt32(cmd.ExecuteScalar());

                cn.Close();
            }

            return result;
        }
        public IActionResult GetById(int id)
        {
            throw new NotImplementedException();
        }
    }
}
