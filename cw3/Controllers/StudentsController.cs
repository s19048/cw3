using System;
using cw3.DAL;
using cw3.Models;
using Microsoft.AspNetCore.Mvc;

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
        public IActionResult GetStudent(string orderBy)
        {
            return Ok(_dbService.GetStudents());
           // return $"Kowalski, Malewski, Andrzejewski sortowanie={orderBy}";
        }
        /*
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }
        */

        // GET: api/Students/5
        [HttpGet("{id}")]
        public IActionResult GetStudent(int id)
        {
            if (id == 1)
                return Ok("Kowalski");
            else if (id == 2)
                return Ok("Malewski");
            else
                return NotFound("Nie znaleziono studenta");
        }

        // POST: api/Students
        [HttpPost]
        public IActionResult CreateStudent(Student student )
        {
            student.IndexNumber = $"s{new Random().Next(1, 20000)}";
            return Ok(student);
        }
        // PUT: api/Students/5
        [HttpPut("{id}")]
        public IActionResult PutStudent(int id)
        {
            if (id == 1)
            {
                return Ok("Aktualizacja dokończona");
            }
            return NotFound("Nie znaleziono studenta o podanym id");

        }
        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public IActionResult RemoveStudent(int id)
        {
            if (id == 1)
            {
                return Ok("Usuwanie ukończone");
            }
            return NotFound("Nie znaleziono studenta o danym id");
        }
        
       
    }
}
