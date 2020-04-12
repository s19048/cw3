using cw3.Controllers;
using cw3.DTOs.Requests;
using cw3.DTOs.Responses;

namespace cw3.Services
{
    public interface IStudentsDbService
    {
        EnrollStudentResponse EnrollStudent(EnrollStudentRequest enrollStudentRequest);
        Enrollment PromoteStudents(int semester, string studies);
    }
}
