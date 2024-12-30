using AutoMapper;
using CollegeAppWebAPI.Models;
using CollegeAppWebAPI.Models.Data;
using CollegeAppWebAPI.Models.Data.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace CollegeAppWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ICollegeRepository<Role> _roleRepository;
        private readonly APIResponse _apiResponse;

        public RoleController(IMapper mapper, ICollegeRepository<Role> roleRepository)
        {
            _mapper = mapper;
            _roleRepository = roleRepository;
            _apiResponse = new APIResponse();
        }

        /// <summary>
        /// Creates a new role in the system.
        /// </summary>
        /// <param name="dto">The role data to create.</param>
        /// <returns>The created role.</returns>
        [HttpPost]
        [Route("Create")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> CreateRole(RoleDTO dto)
        {
            if (dto == null)
            {
                return BadRequest("Role data cannot be null.");
            }

            try
            {
                Role role = _mapper.Map<Role>(dto);
                role.IsDeleted = false;
                role.CreatedDate = DateTime.Now;
                role.ModifiedDate = DateTime.Now;

                var result = await _roleRepository.AddAsync(role);
                dto.Id = result.Id;

                _apiResponse.Data = dto;
                _apiResponse.Status = true;
                _apiResponse.StatusCode = HttpStatusCode.Created;

                return CreatedAtRoute("GetRoleById", new { id = dto.Id }, _apiResponse);
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
        /// Retrieves all roles.
        /// </summary>
        /// <returns>A list of all roles.</returns>
        [HttpGet]
        [Route("All", Name = "GetAllRoles")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> GetRolesAsync()
        {
            try
            {
                var roles = await _roleRepository.GetAllAsync();
                _apiResponse.Data = _mapper.Map<List<RoleDTO>>(roles);
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
        /// Retrieves a role by its ID.
        /// </summary>
        /// <param name="id">The ID of the role.</param>
        /// <returns>The role with the specified ID.</returns>
        [HttpGet("{id:int}", Name = "GetRoleById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<APIResponse>> GetRoleById(int id)
        {
            if (id <= 0)
            {
                return BadRequest("The ID must be a positive integer.");
            }

            try
            {
                var role = await _roleRepository.GetAsync(r => r.Id == id);
                if (role == null)
                {
                    return NotFound($"Role not found with ID: {id}");
                }

                _apiResponse.Data = _mapper.Map<RoleDTO>(role);
                _apiResponse.Status = true;
                _apiResponse.StatusCode = HttpStatusCode.OK;

                return Ok(_apiResponse);
            }
            catch (Exception ex)
            {
                _apiResponse.Status = false;
                _apiResponse.StatusCode = HttpStatusCode.InternalServerError;
                _apiResponse.Error.Add(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, _apiResponse);
            }
        }

        /// <summary>
        /// Updates an existing role.
        /// </summary>
        /// <param name="dto">The role data to update.</param>
        /// <returns>The updated role.</returns>
        [HttpPut]
        [Route("Update")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> UpdateRole(RoleDTO dto)
        {
            if (dto == null || dto.Id <= 0)
            {
                return BadRequest("Invalid role data or ID.");
            }

            try
            {
                var existingRole = await _roleRepository.GetAsync(r => r.Id == dto.Id, true);
                if (existingRole == null)
                {
                    return NotFound($"Role not found with ID: {dto.Id}");
                }

                // Preserve existing data and update properties from DTO
                _mapper.Map(dto, existingRole);
                existingRole.ModifiedDate = DateTime.Now;

                await _roleRepository.UpdateAsync(existingRole);

                _apiResponse.Data = _mapper.Map<RoleDTO>(existingRole);
                _apiResponse.Status = true;
                _apiResponse.StatusCode = HttpStatusCode.OK;

                return Ok(_apiResponse);
            }
            catch (Exception ex)
            {
                _apiResponse.Status = false;
                _apiResponse.StatusCode = HttpStatusCode.InternalServerError;
                _apiResponse.Error.Add(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, _apiResponse);
            }
        }
        [HttpDelete]
        [Route("Delete/{id}",Name ="DeleteRoleById")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<APIResponse>> DeleteRole(int id)
        {
            if (id <= 0)
            {
               
                return BadRequest(_apiResponse);
            }

            try
            {
                var role = await _roleRepository.GetAsync(s => s.Id == id);
                if (role == null)
                {
                     
                    return NotFound($"Role with ID {id} not found.");
                }

                await _roleRepository.DeleteAsync(role);

                _apiResponse.Status = true;
                _apiResponse.StatusCode = HttpStatusCode.NoContent;

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
