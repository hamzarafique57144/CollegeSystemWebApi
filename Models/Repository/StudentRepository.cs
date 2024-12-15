
namespace CollegeAppWebAPI.Models.Repository
{
    public class StudentRepository : IStudentRepository
    {
public List<Student> students = new List<Student>
{
    new Student
    {
        Id = 1,
        StudentName = "Alice",
        Email = "abc@gmail.com",
        Address = "Mansehra, Pakistan",
        DateOfBirth = new DateTime(2000, 5, 15) // Example date
    },
    new Student
    {
        Id = 2,
        StudentName = "Bob",
        Email = "xyz@gmail.com",
        Address = "Abbotabad, Pakistan",
        DateOfBirth = new DateTime(1998, 8, 20) // Example date
    },
    new Student
    {
        Id = 3,
        StudentName = "Charlie",
        Email = "mno@gmail.com",
        Address = "Mansehra, Pakistan",
        DateOfBirth = new DateTime(1995, 3, 10) // Example date
    }
};

        public IEnumerable<Student> GetAllStudents()
        {
            return students;

        }
        public Student GetStudentById(int id)
        {   
            return students.FirstOrDefault(s => s.Id == id);
        }
        public Student GetStudentByName(string name)
        {
            return students.FirstOrDefault(s => s.StudentName.Equals(name, StringComparison.OrdinalIgnoreCase));
        }

        public bool DeleteStudent(int id)
        {
            var student = GetStudentById(id);
            if (student != null)
            {
                students.Remove(student);
                return true;
            }
            return false;
        }
        public Student AddStudent(Student student) 
        {
            // Generate a new unique ID
            student.Id = students.Any() ? students.Max(s => s.Id) + 1 : 1;

            // Add the student to the list
            students.Add(student);

            return student;
        }

        public Student UpdateStudent(int id, Student updatedStudent)
        {
            var student = GetStudentById(id);
            if (student != null)
            {
                student.StudentName = updatedStudent.StudentName;
                student.Email = updatedStudent.Email;
                student.Address = updatedStudent.Address;
                student.DateOfBirth = updatedStudent.DateOfBirth;
                return student;
            }
            return null; // Return null if student is not found
        }

    }
}
