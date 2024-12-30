using CollegeAppWebAPI.Models.Data.Repository;
using CollegeAppWebAPI.Models.Data;
using AutoMapper;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using CollegeAppWebAPI.Models;

namespace CollegeAppWebAPI.Services
{
    public class UserService :IUserService
    {
        private readonly ICollegeRepository<User> _userRepository;
        private readonly IMapper mapper;

        public UserService(ICollegeRepository<User> userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            this.mapper = mapper;
        }
        public async Task<bool> CreateUserAsync(UserDTO dto)
        {
            ArgumentNullException.ThrowIfNull(nameof(dto));
            var existingUser = await _userRepository.GetAsync(u => u.Username.Equals(dto.Username));
            if (existingUser != null)
            {
                throw new Exception("The username is already taken");
            }
            User user = mapper.Map<User>(dto);
            user.IsDeleted = false;
            user.CreatedDate = DateTime.Now;
            user.ModifiedDate = DateTime.Now;
            if (!string.IsNullOrEmpty(dto.Password))
            {
                var PasswordHash = CreatePasswordHashWithSalt(dto.Password);
                user.Password = PasswordHash.PasswordHash;
                user.PasswordSalt = PasswordHash.Salt;
            }
            await _userRepository.AddAsync(user);
            return true;
           
        }
        public async Task<List<UserReadOnlyDTO>> GetUserAsync()
        {
            var users = await _userRepository.GetAllByFilterAsync(u => !u.IsDeleted);
            return mapper.Map<List<UserReadOnlyDTO>>(users);
        }

        public async Task<UserReadOnlyDTO> GetUserById(int id)
        {
            var user = await _userRepository.GetAsync( u => !u.IsDeleted && u.Id == id);
            return mapper.Map<UserReadOnlyDTO>(user);

        }
        public async Task<UserReadOnlyDTO> GetUserByName(string name)
        {
            var user = await _userRepository.GetAsync(u => !u.IsDeleted && u.Username.Equals(name));
            return mapper.Map<UserReadOnlyDTO>(user);

        }
        public async Task<bool> UpdateUserAsync(UserDTO dto)
        {
            ArgumentNullException.ThrowIfNull(dto, nameof(dto));
            var existingUser = await _userRepository.GetAsync(u => !u.IsDeleted && u.Id == dto.Id);
            if (existingUser == null)
            {
                throw new Exception($"User not found with id: {dto.Id}");
            }
            var user = mapper.Map<User>(dto);
            user.ModifiedDate = DateTime.Now;
            //We should make a seprate endpoint to update the password
            //In this endpoind, only user's information should be updated
            //But for now I am updating the password here
            if (!string.IsNullOrEmpty(dto.Password))
            {
                var PasswordHash = CreatePasswordHashWithSalt(dto.Password);
                user.Password = PasswordHash.PasswordHash;
                user.PasswordSalt = PasswordHash.Salt;
            }

            await _userRepository.UpdateAsync(user);
            return true;
        }
        public async Task<bool> DeleteUserAsync(int userId)
        {
            if (userId <= 0)
            {
                throw new ArgumentException(nameof(userId));
            }
            var existingUser = await _userRepository.GetAsync(u => !u.IsDeleted && u.Id == userId);
            if (existingUser == null)
            {
                throw new Exception($"User not found with id: {userId}");
            }
            //Hard Delete --In hard delete, we delete the record
            //Soft Delete --In soft delete, we do not delete the record, we 
            //update the taget by making IsDeleted field true,we are just marking as deleting
            existingUser.IsDeleted = true;
            await _userRepository.UpdateAsync(existingUser);
            return true;
        }
        public (string PasswordHash, string Salt) CreatePasswordHashWithSalt(string password)
        {
            //Create the salt
            var salt = new byte[128 / 8];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }
            //Create Password Hash
            var hash = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 10000,
                numBytesRequested: 256 / 8
                ));
            return (hash, Convert.ToBase64String(salt));
        }
    }
}
