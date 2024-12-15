using CollegeAppWebAPI.Models;

namespace CollegeAppWebAPI.Services
{
    public static class Mapper
    {
        public static StudentDTO ToDto(this Student student)
        {
            return new StudentDTO
            {
                Id = student.Id,
                StudentName = student.StudentName,
                Address = student.Address,
                Email = student.Email,
                DateOfBirth = student.DateOfBirth
            };
        }
        public static IEnumerable<StudentDTO> ToDtoList(this IEnumerable<Student> students)
        {
            return students.Select(student => student.ToDto());
        }
        // Map StudentDTO to Student
        public static Student ToEntity(this StudentDTO studentDto)
        {
            return new Student
            {
                Id = studentDto.Id,
                StudentName = studentDto.StudentName,
                Address = studentDto.Address,
                Email = studentDto.Email,
                DateOfBirth = studentDto.DateOfBirth
            };
        }
    }
}
