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
            //return StatusCode(201, _service.EnrollStudent(request));
            try
            {
                return Ok(_dbService.CreateStudentWithStudies(request));
            }
            catch (DbServiceException e)
            {
                if (e.Type == DbServiceExceptionTypeEnum.NotFound)
                    return NotFound(e.Message);
                else if (e.Type == DbServiceExceptionTypeEnum.ValueNotUnique)
                    return BadRequest(e.Message);
                else
                    return StatusCode(500);
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }


        [HttpPost("{promotions}")]
        public IActionResult PromoteStudent(PromoteStudentRequest request)
        {
           // return StatusCode(201, _service.PromoteStudents(request.Semester, request.Studies));
           if (!_dbService.CheckIfEnrollmentExists(request.Studies, request.Semester))
                return NotFound("Enrollment not found.");
            try
            {
                return Ok(_dbService.PromoteStudents(request.Studies, request.Semester));
            }
            catch (DbServiceException e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.ToString());
            }
        }
    }
}
