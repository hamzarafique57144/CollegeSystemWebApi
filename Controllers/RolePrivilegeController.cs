using AutoMapper;
using CollegeAppWebAPI.Models.Data.Repository;
using CollegeAppWebAPI.Models.Data;
using CollegeAppWebAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace CollegeAppWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolePrivilegeController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ICollegeRepository<RolePrivilege> _rolePrivilegeRepository;
        private readonly APIResponse _apiResponse;

        public RolePrivilegeController(IMapper mapper, ICollegeRepository<RolePrivilege> rolePrivilegeRepository)
        {
            _mapper = mapper;
            _rolePrivilegeRepository = rolePrivilegeRepository;
            _apiResponse = new APIResponse();
        }

        /// <summary>
        /// Creates a new RolePrivilege.
        /// </summary>
        [HttpPost]
        [Route("Create")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> CreateRolePrivilege(RolePrivilegeDTO dto)
        {
            if (dto == null)
            {
                return BadRequest("RolePrivilege data cannot be null.");
            }

            try
            {
                RolePrivilege rolePrivilege = _mapper.Map<RolePrivilege>(dto);
                rolePrivilege.IsDeleted = false;
                rolePrivilege.CreatedDate = DateTime.Now;
                rolePrivilege.ModifiedDate = DateTime.Now;

                var result = await _rolePrivilegeRepository.AddAsync(rolePrivilege);
                dto.Id = result.Id;

                _apiResponse.Data = dto;
                _apiResponse.Status = true;
                _apiResponse.StatusCode = HttpStatusCode.Created;

                return CreatedAtRoute("GetRolePrivilegeById", new { id = dto.Id }, _apiResponse);
            }
            catch (Exception ex)
            {
                _apiResponse.Status = false;
                _apiResponse.StatusCode = HttpStatusCode.InternalServerError;
                _apiResponse.Error.Add(ex.Message);
                return _apiResponse;
            }
        }

        /// <summary>
        /// Retrieves all RolePrivileges.
        /// </summary>
        [HttpGet("All")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> GetAllRolePrivileges()
        {
            try
            {
                var rolePrivileges = await _rolePrivilegeRepository.GetAllAsync();
                _apiResponse.Data = _mapper.Map<IEnumerable<RolePrivilegeDTO>>(rolePrivileges);
                _apiResponse.Status = true;
                _apiResponse.StatusCode = HttpStatusCode.OK;

                return Ok(_apiResponse);
            }
            catch (Exception ex)
            {
                _apiResponse.Status = false;
                _apiResponse.StatusCode = HttpStatusCode.InternalServerError;
                _apiResponse.Error.Add(ex.Message);
                return _apiResponse;
            }
        }

        /// <summary>
        /// Retrieves a RolePrivilege by its ID.
        /// </summary>
        [HttpGet("{id:int}", Name = "GetRolePrivilegeById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> GetRolePrivilegeById(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid ID.");
            }

            try
            {
                var rolePrivilege = await _rolePrivilegeRepository.GetAsync(rp => rp.Id == id);
                if (rolePrivilege == null)
                {
                    return NotFound("RolePrivilege not found.");
                }

                _apiResponse.Data = _mapper.Map<RolePrivilegeDTO>(rolePrivilege);
                _apiResponse.Status = true;
                _apiResponse.StatusCode = HttpStatusCode.OK;

                return Ok(_apiResponse);
            }
            catch (Exception ex)
            {
                _apiResponse.Status = false;
                _apiResponse.StatusCode = HttpStatusCode.InternalServerError;
                _apiResponse.Error.Add(ex.Message);
                return _apiResponse;
            }
        }
        [HttpGet]
        [Route("GetByName/{name}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> GetRolePrivilegeByName(string name)
        {
            try
            {
                var rolePrivilege = await _rolePrivilegeRepository.GetAsync(rp => rp.RolePrivilegeName.Contains(name));
                if (rolePrivilege == null)
                {
                    _apiResponse.Status = false;
                    _apiResponse.StatusCode = HttpStatusCode.NotFound;
                    _apiResponse.Error.Add($"RolePrivilege with name '{name}' not found.");
                    return NotFound(_apiResponse);
                }

                _apiResponse.Data = _mapper.Map<RolePrivilegeDTO>(rolePrivilege);
                _apiResponse.Status = true;
                _apiResponse.StatusCode = HttpStatusCode.OK;

                return Ok(_apiResponse);
            }
            catch (Exception ex)
            {
                _apiResponse.Status = false;
                _apiResponse.StatusCode = HttpStatusCode.InternalServerError;
                _apiResponse.Error.Add(ex.Message);
                return _apiResponse;
            }
        }

        /// <summary>
        /// Updates an existing RolePrivilege.
        /// </summary>
        [HttpPut]
        [Route("Update")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> UpdateRolePrivelge(RolePrivilegeDTO dto)
        {
            if (dto == null || dto.Id <= 0)
            {
                return BadRequest("Invalid role data or ID.");
            }

            try
            {
                var existingRole = await _rolePrivilegeRepository.GetAsync(r => r.Id == dto.Id, true);
                if (existingRole == null)
                {
                    return NotFound($"Role not found with ID: {dto.Id}");
                }

                // Preserve existing data and update properties from DTO
                _mapper.Map(dto, existingRole);
                existingRole.ModifiedDate = DateTime.Now;

                await _rolePrivilegeRepository.UpdateAsync(existingRole);

                _apiResponse.Data = _mapper.Map<RolePrivilegeDTO>(existingRole);
                _apiResponse.Status = true;
                _apiResponse.StatusCode = HttpStatusCode.OK;

                return Ok(_apiResponse);
            }
            catch (Exception ex)
            {
                _apiResponse.Status = false;
                _apiResponse.StatusCode = HttpStatusCode.InternalServerError;
                _apiResponse.Error.Add(ex.Message);
                return _apiResponse;
            }
        }

        /// <summary>
        /// Deletes a RolePrivilege by ID.
        /// </summary>
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> DeleteRolePrivilege(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid ID.");
            }

            try
            {
                var rolePrivilege = await _rolePrivilegeRepository.GetAsync(rp => rp.Id == id);
                if (rolePrivilege == null)
                {
                    return NotFound("RolePrivilege not found.");
                }

                await _rolePrivilegeRepository.DeleteAsync(rolePrivilege);

                _apiResponse.Status = true;
                _apiResponse.StatusCode = HttpStatusCode.NoContent;

                return NoContent();
            }
            catch (Exception ex)
            {
                _apiResponse.Status = false;
                _apiResponse.StatusCode = HttpStatusCode.InternalServerError;
                _apiResponse.Error.Add(ex.Message);
                return _apiResponse;
            }
        }

        [HttpGet("GetAllRolePrivilegesByRoleId/{roleId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> GetAllRolePrivilegesByRoleId(int roleId)
        {
            try
            {
                var rolePrivileges = await _rolePrivilegeRepository.GetAllByFilterAsync(rp => rp.RoleID == roleId && !rp.IsDeleted);
                if (rolePrivileges == null || !rolePrivileges.Any())
                {
                    _apiResponse.Status = false;
                    _apiResponse.StatusCode = HttpStatusCode.NotFound;
                    _apiResponse.Error.Add($"No RolePrivileges found for RoleId {roleId}.");
                    return NotFound(_apiResponse);
                }

                _apiResponse.Data = _mapper.Map<IEnumerable<RolePrivilegeDTO>>(rolePrivileges);
                _apiResponse.Status = true;
                _apiResponse.StatusCode = HttpStatusCode.OK;

                return Ok(_apiResponse);
            }
            catch (Exception ex)
            {
                _apiResponse.Status = false;
                _apiResponse.StatusCode = HttpStatusCode.InternalServerError;
                _apiResponse.Error.Add(ex.Message);
                return _apiResponse;
            }
        }

    }
}
