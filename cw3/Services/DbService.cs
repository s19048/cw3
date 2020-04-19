using System.Data.SqlClient;

namespace Cw3
{
    internal class DbService
    {
        public bool CheckIndex(string IndexNumber)
        {
            int id = int.Parse(IndexNumber);
            using (SqlConnection client = new SqlConnection("Data Source=db-mssql16.pjwstk.edu.pl; Initial Catalog=s19048;User ID=apbds19048;Password=admin"))
            using (SqlCommand command = new SqlCommand())
            {
                command.Connection = client;
                command.CommandText = " select IndexNumber, FirstName, LastName, BirthDate, IdEnrollment from Student where IndexNumber=@id";
                command.Parameters.AddWithValue("id", id);

                client.Open();
                SqlDataReader SqlDataReader = command.ExecuteReader();
                if (SqlDataReader.Read())
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
    }
}