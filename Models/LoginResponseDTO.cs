namespace CollegeAppWebAPI.Models
{
    public class LoginResponseDTO
    {
        // The username of the authenticated user
        public string UserName { get; set; }

        // The generated JWT token
        public string Token { get; set; }

        // The token's expiration time (optional, but useful for the client)
        public DateTime Expiration { get; set; }
    }
}
