using System.ComponentModel.DataAnnotations;

namespace CollegeAppWebAPI.Models
{
    public class RoleDTO
    {
        public int Id { get; set; }
        [Required]
        public string RoleName { get; set; }
        public string Description { get; set; }
        [Required]
        public bool Active { get; set; }
        
    }
}
