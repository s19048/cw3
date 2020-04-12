using System.Collections.Generic;
using cw3.Models;

namespace cw3.DAL
{
    public class MockDbService : IDbService
    {
        private static IEnumerable<Student> _students;
        static MockDbService()
        {
            _students = new List<Student>
            {
                new Student{IndexNumber="11", FirstName="Michal", LastName="Malarski"},
                new Student{IndexNumber="22", FirstName="Magda", LastName="Gessler"},
                new Student{IndexNumber="33", FirstName="Krzysztof", LastName="Ibisz"}
            };

        }
        public IEnumerable<Student> GetStudents()
        {
            return _students;
        }
    }
}
