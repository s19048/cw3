using System;
using cw3.DAL;
using cw3.PartialModels;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using cw3.Services;
using Microsoft.AspNetCore.Authorization;
using cw3.DTOs.Requests;

namespace cw3.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentsController : ControllerBase
    {
        private readonly IDbService _dbService;
        private readonly IConfiguration _configuration;
        private readonly ILoginService _loginService;
        private readonly IStudentsDbService _studentsDbService;
        private readonly s19048Context _s19048Context;

        public StudentsController(IDbService dbService, IConfiguration configuration, LoginService loginService, IStudentsDbService studentsDbService,s19048Context s19048Context)
        {
            _dbService = dbService;
            _configuration = configuration;
            _loginService = loginService;
            _studentsDbService = studentsDbService;
            _s19048Context = s19048Context;
        }
        // GET: api/Students
        [Authorize]
        [HttpGet]
        public IActionResult GetStudents([FromServices]IDbService dbService)
        {
            var listOfStudents = new List<Models.Student>();
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
                        var student = new Models.Student();
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
        public IActionResult GetStudent(string IndexNumber)
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
                var student = new Models.Student();
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
        public IActionResult GetSemester(string indexNumber, int semester)
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

        [HttpGet]
        public IActionResult GetStudents()
        {
            return Ok(_studentsDbService.GetStudents());
        }

        // POST: api/Students
        [HttpPost]
        public IActionResult CreateStudent(Models.Student student )
        {
            student.IndexNumber = $"s{new Random().Next(1, 20000)}";
            return Ok(student);
        }
        // PUT: api/Students/5 
        [HttpPut("{IndexNumber}")]
        public IActionResult PutStudent(int IndexNumber)
        {
            if (IndexNumber == 1)
            {
                return Ok("Aktualizacja dokończona");
            }
            return NotFound("Nie znaleziono studenta o podanym id");
        }
        // DELETE: api/ApiWithActions/5
        [HttpDelete("{IndexNumber}")]
        public IActionResult RemoveStudent(int IndexNumber)
        {
            if (IndexNumber == 1)
            {
                return Ok("Usuwanie ukończone");
            }
            return NotFound("Nie znaleziono studenta o danym IndexNumber");
        }
        [Authorize]
        [HttpPost("login")]
        public IActionResult Login(LoginRequestDto requestDto)
        {
            return Ok(_loginService.Login(requestDto));
        }

        [Authorize(Roles = "Employee")]
        [HttpPost("refresh-token/{rToken}")]
        public IActionResult RefreshToken(string rToken)
        {
            return Ok(_loginService.RefreshToken(rToken));
        }
    }
}
