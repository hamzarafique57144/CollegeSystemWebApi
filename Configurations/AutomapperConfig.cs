using AutoMapper;
using CollegeAppWebAPI.Models;
using CollegeAppWebAPI.Models.Data;

namespace CollegeAppWebAPI.Configurations
{
    public class AutomapperConfig : Profile
    {
        public AutomapperConfig()
        {
            CreateMap<Student, StudentDTO>().ReverseMap().ForMember(n => n.Address, opt => opt.MapFrom(x => string.IsNullOrEmpty(x.Address)?"No record is found": x.Address));
            CreateMap<RoleDTO, Role>().ReverseMap();
            CreateMap<RolePrivilegeDTO, RolePrivilege>().ReverseMap();
            CreateMap<UserDTO, User>().ReverseMap();
            CreateMap<UserReadOnlyDTO, User>().ReverseMap();
        }
    }
}
