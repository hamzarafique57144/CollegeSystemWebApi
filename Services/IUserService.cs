using CollegeAppWebAPI.Models;

namespace CollegeAppWebAPI.Services
{
    public interface IUserService
    {
        Task<bool> CreateUserAsync(UserDTO dto);
        Task<List<UserReadOnlyDTO>> GetUserAsync();
        Task<UserReadOnlyDTO> GetUserById(int id);
        Task<UserReadOnlyDTO> GetUserByName(string name);
        Task<bool> UpdateUserAsync(UserDTO dto);
        Task<bool> DeleteUserAsync(int userId);
        (string PasswordHash, string Salt) CreatePasswordHashWithSalt(string password);
    }
}
