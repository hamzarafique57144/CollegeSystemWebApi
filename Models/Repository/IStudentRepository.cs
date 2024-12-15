namespace CollegeAppWebAPI.Models.Repository
{
    public interface IStudentRepository
    {
        IEnumerable<Student> GetAllStudents();
        Student GetStudentById(int id);
        Student GetStudentByName(string name); 
        bool DeleteStudent(int id);
        Student AddStudent(Student student);
        Student UpdateStudent(int id, Student updatedStudent);
    }
}
