using System.ComponentModel.DataAnnotations;

namespace CollegeAppWebAPI.Models
{
    public class RolePrivilegeDTO
    {
        public int Id { get; set; }
        public int RoleId { get; set; }
        [Required]
        public string RolePrivilegeName { get; set; }
        public string Description { get; set; }
        [Required]
        public bool Active { get; set; }
    }
}
