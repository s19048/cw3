using Microsoft.AspNetCore.Mvc;
using cw3.DTOs.Requests;
using cw3.Services;

namespace cw3.Controllers
{
    public class EnrollmentController : ControllerBase
    {
        private IStudentsDbService _service;

        public EnrollmentController(IStudentsDbService service)
        {
            _service = service;
        }


        [HttpPost]
        public IActionResult EnrollStudent(EnrollStudentRequest request)
        {
            return StatusCode(201, _service.EnrollStudent(request));
        }


        [HttpPost("{promotions}")]
        public IActionResult PromoteStudent(PromoteStudentRequest request)
        {
            return StatusCode(201, _service.PromoteStudents(request.Semester, request.Studies));
        }
    }
}
