using System;
using System.Data.SqlClient;
using cw3.DTOs.Requests;
using cw3.DTOs.Responses;
using cw3.Controllers;

namespace cw3.Services
{
    public class SqlServerDbService : IStudentsDbService
    {
        public EnrollStudentResponse EnrollStudent(EnrollStudentRequest request)
        {
            using (SqlConnection connection = new SqlConnection("Data Source=db-mssql16.pjwstk.edu.pl; Initial Catalog=s19048;User ID=apbds19048;Password=admin"))
            {
                using (SqlCommand command = new SqlCommand())
                {
                    int idEnrollment;
                    command.Connection = connection;
                    connection.Open();
                    var tran = connection.BeginTransaction();
                    command.Transaction = tran;
                    try
                    {
                        command.CommandText = "select IdStudy " +
                                              "from studies where name=@name";
                        command.Parameters.AddWithValue("name", request.Studies);
                        var SqlDataReader = command.ExecuteReader();
                        int id;
                        if (!SqlDataReader.Read())
                        {
                            SqlDataReader.Close();
                            command.CommandText = "select idStudy " +
                                                  "from Studies " +
                                                  "where idStudy=(select max(IdStudy) from studies);";
                            SqlDataReader = command.ExecuteReader();
                            if (SqlDataReader.Read())
                            {
                                id = int.Parse(SqlDataReader["IdStudy"].ToString()) + 1;
                                SqlDataReader.Close();
                                command.CommandText = "Insert Into Studies (IdStudy,Name) values (@id,@name)";
                                command.Parameters.AddWithValue("id", id);
                                command.Parameters.AddWithValue("name", request.Studies);
                                command.ExecuteNonQuery();
                            }
                        }

                        SqlDataReader.Close();
                        command.CommandText = "select Enrollment.idStudy " +
                                              "from Enrollment " +
                                              "inner join Studies on Enrollment.IdStudy=Studies.idStudy " +
                                              "where Studies.name=@name and Enrollment.Semester=1";
                        command.Parameters.AddWithValue("name", request.Studies);
                        SqlDataReader = command.ExecuteReader();

                        if (!SqlDataReader.Read())
                        {
                            SqlDataReader.Close();
                            command.CommandText = "select idEnrollment " +
                                                  "from Enrollment " +
                                                  "where idEnrollment = (select max(idEnrollment) from Enrollment)";
                            SqlDataReader = command.ExecuteReader();

                            if (SqlDataReader.Read())
                            {
                                idEnrollment = int.Parse(SqlDataReader["idEnrollment"].ToString()) + 1;
                                SqlDataReader.Close();
                                command.CommandText = "select idStudy " +
                                                      "from Enrollment " +
                                                      "inner join Studies on Enrollment.IdStudy=Studies.idStudy " +
                                                      "where Studies.name=@name";
                                SqlDataReader = command.ExecuteReader();
                                int idStudy;
                                if (SqlDataReader.Read())
                                {
                                    idStudy = int.Parse(SqlDataReader["idStudy"].ToString()) + 1;
                                    SqlDataReader.Close();
                                    command.CommandText = "INSERT INTO Enrollment (IdEnrollment, Semester, IdStudy, StartDate) " +
                                                          "VALUES (@IdEnrollment,1,@IdStudy,@StartDate)";
                                    command.Parameters.AddWithValue("IdEnrollment", idEnrollment);
                                    command.Parameters.AddWithValue("StartDate", DateTime.Today);
                                    command.Parameters.AddWithValue("IdStudy", idStudy);
                                    command.ExecuteReader();

                                    SqlDataReader.Close();
                                    command.CommandText = "INSERT INTO Student(IndexNumber, FirstName, LastName, BirthDate, IdEnrollment) " +
                                                          "VALUES (@index, @fname, @lname, @birthDate, @idEnrollment";
                                    command.Parameters.AddWithValue("index", request.IndexNumber);
                                    command.Parameters.AddWithValue("fname", request.FirstName);
                                    command.Parameters.AddWithValue("lname", request.LastName);
                                    command.Parameters.AddWithValue("birthDate", request.BirthDate);
                                    command.Parameters.AddWithValue("idEnrollment", idEnrollment);
                                    command.ExecuteNonQuery();
                                }
                            }
                        }
                        tran.Commit();
                    }
                    catch (SqlException sql)
                    {
                        tran.Rollback();
                        Console.Write(sql);
                    }
                }
            }
            var response = new EnrollStudentResponse()
            {
                LastName = request.LastName,
                Semester = 1,
                StartDate = DateTime.Now
            };

            return response;
        }

        public Enrollment PromoteStudents(int semester, string studiesName)
        {
            var enrollment = new Enrollment();
            using (var connection = new SqlConnection("Data Source=db-mssql16.pjwstk.edu.pl; Initial Catalog=s19048;User ID=apbds19048;Password=admin"))
            {
                using (var command = new SqlCommand())
                {
                    command.Connection = connection;
                    connection.Open();

                    command.CommandText = "SELECT semester " +
                                          "FROM Enrollment " +
                                          "inner join Studies on Enrollment.IdStudy=Studies.idStudy " +
                                          "where Enrollment.Semester = @semester " +
                                          "AND Studies.name = (SELECT name from Studies where Name = @name)";
                    command.Parameters.AddWithValue("name", studiesName);
                    command.Parameters.AddWithValue("semester", semester);

                    var SqlDataReader = command.ExecuteReader();

                    if (!SqlDataReader.Read())
                    {
                        //return NotFound();
                    }
                    SqlDataReader.Close();
                    command.Parameters.Clear();

                    command.CommandText = "PromoteStudents";
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("Studies", studiesName);
                    command.Parameters.AddWithValue("Semester", semester);

                    SqlDataReader = command.ExecuteReader();
                    if (SqlDataReader.Read())
                    {
                        enrollment.IdEnrollment = int.Parse(SqlDataReader["IdEnrollment"].ToString());
                        enrollment.IdStudy = int.Parse(SqlDataReader["IdStudy"].ToString());
                        enrollment.Semester = int.Parse(SqlDataReader["Semester"].ToString());
                        enrollment.StartDate = DateTime.Parse(SqlDataReader["StartDate"].ToString());
                    }
                }
            }
            return enrollment;
        }

        Enrollment IStudentsDbService.PromoteStudents(int semester, string studies)
        {
            throw new NotImplementedException();
        }
    }
}
