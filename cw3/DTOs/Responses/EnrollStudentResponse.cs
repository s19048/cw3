using System;

namespace cw3.DTOs.Responses
{
    public class EnrollStudentResponse
    {
        public string LastName { get; set; }

        public int Semester { get; set; }

        public DateTime StartDate { get; set; }
    }
}
