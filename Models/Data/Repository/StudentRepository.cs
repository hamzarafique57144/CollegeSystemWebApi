using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.Internal;

namespace CollegeAppWebAPI.Models.Data.Repository
{
    public class StudentRepository :CollegeRepository<Student>, IStudentRepository
    {
        private readonly CollegeDbContext _context;

        public StudentRepository(CollegeDbContext context) : base(context)
        {
            _context = context;
        }

        public Task<List<Student>> GetSudentByFeeStatus(int feeStatus)
        {
            return null;
        }
    }
}
