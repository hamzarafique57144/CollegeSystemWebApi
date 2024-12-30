using AutoMapper;
using CollegeAppWebAPI.Models.Data.Repository;
using CollegeAppWebAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using CollegeAppWebAPI.Services;
using CollegeAppWebAPI.Models.Data;
using System.Net;

namespace CollegeAppWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        
        private readonly ILogger<UserController> _logger;
        
        private APIResponse _apiResponse;
        private readonly IUserService userService;

        public UserController(ILogger<UserController> logger,APIResponse apiResponse,IUserService userService)
        {
            
            _logger = logger;
       
            _apiResponse = apiResponse;
            this.userService = userService;
        }
        [HttpPost]
        [Route("Create")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> CreateUserAsync(UserDTO dto)
        {
            try
            {
                var userCreated =await userService.CreateUserAsync(dto);

                _apiResponse.Data = userCreated;
                _apiResponse.Status = true;
                _apiResponse.StatusCode = HttpStatusCode.OK;
                return Ok(_apiResponse);
            }
            catch (Exception ex)
            {
                _apiResponse.Error.Add(ex.Message);
                _apiResponse.Status = false;
                _apiResponse.StatusCode = HttpStatusCode.InternalServerError;              
                return _apiResponse;
            }
        }
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet]
        [Route("All",Name ="GetAllUsers")]
        public async Task<ActionResult<APIResponse>> GetUsers()
        {
            try
            {
                var users = await userService.GetUserAsync();

                //var studentDtos = _mapper.Map<IEnumerable<StudentDTO>>(students);              
                _apiResponse.Data = users;
                _apiResponse.Status = true;
                _apiResponse.StatusCode = HttpStatusCode.OK;
                return Ok(_apiResponse);
            }
            catch (Exception ex)
            {
                _apiResponse.Error.Add(ex.Message);
                _apiResponse.Status = false;
                _apiResponse.StatusCode = HttpStatusCode.InternalServerError;
                _logger.LogError(ex, "Error occurred while retrieving all users.");
                return _apiResponse;
            }
        }

        /// <summary>
        /// Retrieves a student by their unique ID.
        /// </summary>
        [HttpGet("{id:int}", Name = "GetUsesrById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> GetUserById(int id)
        {
            if (id <= 0)
                return BadRequest("The ID must be a positive integer greater than 0.");

            try
            {
                var user = await userService.GetUserById(id);
                if (user == null)
                    return NotFound($"User with ID {id} not found.");

                _apiResponse.Data = user;
                _apiResponse.Status = true;
                _apiResponse.StatusCode = HttpStatusCode.OK;
                return Ok(_apiResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving user by ID.");
                _apiResponse.Error.Add(ex.Message);
                _apiResponse.Status = false;
                _apiResponse.StatusCode = HttpStatusCode.InternalServerError;
                return _apiResponse;
            }
        }

        /// <summary>
        /// Retrieves a student by their name.
        /// </summary>
        [HttpGet("ByName/{userName}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> GetUserByName(string userName)
        {
            if (string.IsNullOrEmpty(userName))
                return BadRequest("The name must not be null or empty.");

            try

            {
                var user = await userService.GetUserByName(userName);
                if (user == null)
                    return NotFound($"User with name '{userName}' not found.");

                _apiResponse.Data = user;
                _apiResponse.Status = true;
                _apiResponse.StatusCode = HttpStatusCode.OK;
                return Ok(_apiResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving user by name.");
                _apiResponse.Error.Add(ex.Message);
                _apiResponse.Status = false;
                _apiResponse.StatusCode = HttpStatusCode.InternalServerError;
                return _apiResponse;
            }
        }
        [HttpPut]
        [Route("Update")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> UpdateUserAsync(UserDTO dto)
        {
            if (dto == null || dto.Id <= 0)
            {
                return BadRequest("Invalid role data or ID.");
            }

            try
            {                       
               var user = await userService.UpdateUserAsync(dto);

                _apiResponse.Data = user;
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
        [HttpDelete]
        [Route("Delete/{id}", Name = "DeleteUserById")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<APIResponse>> DeleteUserAsync(int id)
        {           
            try
            {
                var user = await userService.DeleteUserAsync(id);                               
                _apiResponse.Status = true;
                _apiResponse.Data = user;
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
