using cw3.Controllers;
using cw3.DTOs.Requests;
using cw3.DTOs.Responses;
using cw3.Models;

namespace cw3.Services
{
    public interface IStudentsDbService
    {
        EnrollStudentResponse EnrollStudent(EnrollStudentRequest enrollStudentRequest);
        Enrollment PromoteStudents(int semester, string studies);

        Student GetStudent(string IndexNumber);
        object GetStudent();
    }
}
