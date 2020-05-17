using System;
using System.Data.SqlClient;
using cw3.DTOs.Requests;
using cw3.DTOs.Responses;
using cw3.Controllers;
using cw3.PartialModels;
using System.Collections.Generic;
using System.Linq;


namespace cw3.Services
{
    public class SqlServerDbService : IStudentsDbService
    {
        private readonly s19048Context _dbContext;

        public SqlServerDbService()
        {
        }

        public SqlServerDbService(s19048Context context)
        {
            this._dbContext = context;
        }
        public bool CheckIndex(string index)
        {
            int id = int.Parse(index);
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
            public string DeleteStudent(string index)
            {
                if (index == null)
                {
                    return "Bad student index";
                }

                var student = _dbContext.Student.FirstOrDefault(student => student.IndexNumber.Equals(index));
                if (student == null)
                    return "Student not found";

                _dbContext.Remove(student);
                _dbContext.SaveChanges();
                return "OK";
            }

        public EnrollStudentResponse EnrollStudent(EnrollStudentRequest request)
        {
            using (var transaction = _dbContext.Database.BeginTransaction())
            {
                if (_dbContext.Studies.FirstOrDefault(st => st.Name == request.Studies) == null)
                { return null; }

                int idStudy = _dbContext.Studies.Where(st => st.Name == request.Studies).Select(st => st.IdStudy).SingleOrDefault();

                PartialModels.Enrollment enrollment = _dbContext.Enrollment.FirstOrDefault(e => (e.IdStudy == idStudy) && (e.Semester == 1));

                if (enrollment == null)
                {
                    int maxId = _dbContext.Enrollment.Max(e => e.IdEnrollment);

                    enrollment = new PartialModels.Enrollment();
                    enrollment.IdEnrollment = maxId + 1;
                    enrollment.Semester = 1;
                    enrollment.IdStudy = idStudy;
                    enrollment.StartDate = DateTime.Now;
                    _dbContext.Enrollment.Add(enrollment);
                }

                var StudentExist = _dbContext.Student.FirstOrDefault(student => student.IndexNumber.Equals(request.IndexNumber));
                if (StudentExist != null)
                { return null; }

                //"BirthDate": "1993-03-30"
                Student student = new Student();
                student.IndexNumber = request.IndexNumber;
                student.FirstName = request.FirstName;
                student.LastName = request.LastName;
                student.BirthDate = request.BirthDate;
                student.IdEnrollment = enrollment.IdEnrollment;
                _dbContext.Student.Add(student);

                _dbContext.SaveChanges();
                transaction.Commit();

                var response = new EnrollStudentResponse()
                {
                    LastName = request.LastName,
                    Semester = 1,
                    StartDate = DateTime.Now
                };

                return response;
            }
        }

                public Student GetStudent(string IndexNumber)
                {
                    throw new NotImplementedException();
                }

                public IEnumerable<Student> GetStudents()
                {
                    return _dbContext.Student.ToList();
                }

                //public PartialModels.Enrollment PromoteStudents(int semester, string studiesName)
                //{
                //    var enrollment = new PartialModels.Enrollment();
                //    using (var connection = new SqlConnection("Data Source=db-mssql16.pjwstk.edu.pl; Initial Catalog=s19048;User ID=apbds19048;Password=admin"))
                //    {
                //        using (var command = new SqlCommand())
                //        {
                //            command.Connection = connection;
                //            connection.Open();

                //            command.CommandText = "SELECT semester " +
                //                                  "FROM Enrollment " +
                //                                  "inner join Studies on Enrollment.IdStudy=Studies.idStudy " +
                //                                  "where Enrollment.Semester = @semester " +
                //                                  "AND Studies.name = (SELECT name from Studies where Name = @name)";
                //            command.Parameters.AddWithValue("name", studiesName);
                //            command.Parameters.AddWithValue("semester", semester);

                //            var SqlDataReader = command.ExecuteReader();

                //            if (!SqlDataReader.Read())
                //            {
                //                //return NotFound();
                //            }
                //            SqlDataReader.Close();
                //            command.Parameters.Clear();

                //            command.CommandText = "PromoteStudents";
                //            command.CommandType = System.Data.CommandType.StoredProcedure;
                //            command.Parameters.AddWithValue("Studies", studiesName);
                //            command.Parameters.AddWithValue("Semester", semester);

                //            SqlDataReader = command.ExecuteReader();
                //            if (SqlDataReader.Read())
                //            {
                //                enrollment.IdEnrollment = int.Parse(SqlDataReader["IdEnrollment"].ToString());
                //                enrollment.IdStudy = int.Parse(SqlDataReader["IdStudy"].ToString());
                //                enrollment.Semester = int.Parse(SqlDataReader["Semester"].ToString());
                //                enrollment.StartDate = DateTime.Parse(SqlDataReader["StartDate"].ToString());
                //            }
                //        }
                //    }
                //    return enrollment;
                //}

      
                    public string UpdateStudent(UpdateStudentRequest request)
                    {
                        var student = _dbContext.Student.FirstOrDefault(student => student.IndexNumber.Equals(request.IndexNumber));
                        if (student == null)
                        {
                            return "Student not found";
                        }

                        student.FirstName = request.FirstName != null ? request.FirstName : student.FirstName;
                        student.LastName = request.LastName != null ? request.LastName : student.LastName;
                        student.BirthDate = request.BirthDate != null ? request.BirthDate : student.BirthDate;
                        _dbContext.Update(student);
                        _dbContext.SaveChanges();
                        return "OK";
                    }

        public PromoteStudentResponse PromoteStudents(int semester, string studies)
        {
            using (var transaction = _dbContext.Database.BeginTransaction())
            {
                int idStudy = _dbContext.Studies
                .Where(st => st.Name == studies)
                .Select(st => st.IdStudy).SingleOrDefault();

                PartialModels.Enrollment enrollment = _dbContext.Enrollment.FirstOrDefault(e => e.IdStudy == idStudy && e.Semester == semester);

                if (enrollment == null)
                {
                    return null;
                }

                int oldIdEnrollment = enrollment.IdEnrollment;
                enrollment = _dbContext.Enrollment.FirstOrDefault(e => e.IdStudy == idStudy && e.Semester == semester + 1);

                if (enrollment == null)
                {
                    int maxId = _dbContext.Enrollment.Max(e => e.IdEnrollment);
                    enrollment = new PartialModels.Enrollment();
                    enrollment.IdEnrollment = maxId + 1;
                    enrollment.Semester = semester + 1;
                    enrollment.IdStudy = idStudy;
                    enrollment.StartDate = DateTime.Now;
                    _dbContext.Enrollment.Add(enrollment);
                    _dbContext.SaveChanges();

                }

                var students = _dbContext.Student.Where(s => s.IdEnrollment == oldIdEnrollment).ToList();

                foreach (Student student in students)
                {
                    student.IdEnrollment = enrollment.IdEnrollment;
                }

                _dbContext.SaveChanges();
                transaction.Commit();
                var response = new PromoteStudentResponse()
                {
                    Study = studies,
                    NewIdStudy = enrollment.IdStudy,
                    NewSemester = enrollment.Semester
                };

                return response;
            }
        }
       
    } 
}

