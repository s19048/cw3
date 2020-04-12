using System;
using cw3.DAL;
using cw3.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Collections.Generic;

namespace cw3.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentsController : ControllerBase
    {
        private readonly IDbService _dbService;
        public StudentsController(IDbService dbService)
        {
            dbService = _dbService;
        }
        // GET: api/Students
        [HttpGet]
        public IActionResult GetStudents([FromServices]IDbService dbService)
        {
            var listOfStudents = new List<Student>();
            using (SqlConnection client = new SqlConnection("Data Source=db-mssql16.pjwstk.edu.pl; Initial Catalog=s19048; User ID=apbds19048; Password=admin"))
            {
                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = client;
                    command.CommandText = "select IndexNumber, FirstName, LastName, BirthDate, Name, Semester, Student.Idenrollment, Studies.name " +
                                          "from Student inner join Enrollment on Enrollment.IdEnrollment = Student.IdEnrollment " +
                                          "inner join Studies on Enrollment.IdStudy = Studies.IdStudy";
                    client.Open();
                    SqlDataReader SqlDataReader = command.ExecuteReader();
                    while (SqlDataReader.Read())
                    {
                        var student = new Student();
                        student.IndexNumber = SqlDataReader["IndexNumber"].ToString();
                        student.FirstName = SqlDataReader["FirstName"].ToString();
                        student.LastName = SqlDataReader["LastName"].ToString();
                        student.BirthDate = DateTime.Parse(SqlDataReader["BirthDate"].ToString());
                        student.Semester = int.Parse(SqlDataReader["Semester"].ToString());
                        student.IdEnrollment = int.Parse(SqlDataReader["Idenrollment"].ToString());
                        student.Studies = SqlDataReader["name"].ToString();
                        listOfStudents.Add(student);
                    }
                }
            }
            return Ok(listOfStudents);
        }
        [HttpGet("{IndexNumber}")]
        public IActionResult getStudent(string IndexNumber)
        {
            int id = int.Parse(IndexNumber);
            using (SqlConnection client = new SqlConnection("Data Source=db-mssql16.pjwstk.edu.pl; Initial Catalog=s19048; User ID=apbds19048; Password=admin"))
            using (SqlCommand command = new SqlCommand())
            {
            command.Connection = client;
            command.CommandText = " select IndexNumber, FirstName, LastName, BirthDate, IdEnrollment " +
                                  "from Student " +
                                  "where IndexNumber=@id";
            command.Parameters.AddWithValue("id", id);

            client.Open();
            SqlDataReader SqlDataReader = command.ExecuteReader();
            if (SqlDataReader.Read())
            {
                var student = new Student();
                student.IndexNumber = SqlDataReader["IndexNumber"].ToString();
                student.FirstName = SqlDataReader["FirstName"].ToString();
                student.LastName = SqlDataReader["LastName"].ToString();
                student.BirthDate = DateTime.Parse(SqlDataReader["BirthDate"].ToString());
                student.IdEnrollment = int.Parse(SqlDataReader["IdEnrollment"].ToString());
                return Ok(student);
            }
            return NotFound("Nie znaleziono studenta");
        }
    }
        [HttpGet("{IndexNumber}/{Semester}")]
        public IActionResult getSemester(string indexNumber, int semester)
        {
            int id = int.Parse(indexNumber);
            using (SqlConnection con = new SqlConnection("Data Source=db-mssql16.pjwstk.edu.pl; Initial Catalog=s19048; User ID=apbds19048; Password=admin"))
            using (SqlCommand com = new SqlCommand())
            {
                com.Connection = con;
                com.CommandText = " select Semester, IdStudy, StartDate from Student " +
                                  "inner join Enrollment on Student.IdEnrollment = Enrollment.IdEnrollment " +
                                  "where Enrollment.Semester=@semester and Student.IndexNumber = @id;";
                com.Parameters.AddWithValue("id", id);
                com.Parameters.AddWithValue("semester", semester);

                con.Open();
                SqlDataReader dr = com.ExecuteReader();
                if (dr.Read())
                {
                    Enrollment enrollment = new Enrollment();

                    enrollment.IdStudy = int.Parse(dr["IdStudy"].ToString());
                    enrollment.Semester = int.Parse(dr["Semester"].ToString());
                    enrollment.StartDate = DateTime.Parse(dr["StartDate"].ToString());
                    return Ok(enrollment);
                }
                return NotFound();
            }
        }
        /* public IActionResult GetStudent(string orderBy)
         {
             return Ok(_dbService.GetStudents());
            // return $"Kowalski, Malewski, Andrzejewski sortowanie={orderBy}";
         }

         public IEnumerable<string> Get()
         {
             return new string[] { "value1", "value2" };
         }
         */

        // GET: api/Students/5
        /*[HttpGet("{id}")]
        public IActionResult GetStudent(int id)
        {
            if (id == 1)
                return Ok("Kowalski");
            else if (id == 2)
                return Ok("Malewski");
            else
                return NotFound("Nie znaleziono studenta");
        }
        */
        // POST: api/Students
        [HttpPost]
        public IActionResult CreateStudent(Student student )
        {
            student.IndexNumber = $"s{new Random().Next(1, 20000)}";
            return Ok(student);
        }
        // PUT: api/Students/5 -- zamiana na indexNumber
        [HttpPut("{IndexNumber}")]
        public IActionResult PutStudent(int IndexNumber)
        {
            if (IndexNumber == 1)
            {
                return Ok("Aktualizacja dokończona");
            }
            return NotFound("Nie znaleziono studenta o podanym id");
        }
        // DELETE: api/ApiWithActions/5 -- zamiana na indexNumber
        [HttpDelete("{IndexNumber}")]
        public IActionResult RemoveStudent(int IndexNumber)
        {
            if (IndexNumber == 1)
            {
                return Ok("Usuwanie ukończone");
            }
            return NotFound("Nie znaleziono studenta o danym IndexNumber");
        }
    }
}
