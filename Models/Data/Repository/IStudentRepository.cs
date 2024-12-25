namespace CollegeAppWebAPI.Models.Data.Repository
{
    public interface IStudentRepository :ICollegeRepository<Student>
    {
        Task<List<Student>> GetSudentByFeeStatus(int feeStatus);
    }
    
}
