using AutoMapper;
using CollegeAppWebAPI.Models;
using CollegeAppWebAPI.Models.Data;
using CollegeAppWebAPI.Models.Data.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace CollegeAppWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    
    [EnableCors(PolicyName = "AllowAll")]
    
    public class StudentController : ControllerBase
    {
        private readonly IStudentRepository _repository;
        private readonly ILogger<StudentController> _logger;
        private readonly IMapper _mapper;
        private APIResponse _apiResponse;

        public StudentController(IStudentRepository repository, ILogger<StudentController> logger, IMapper mapper,APIResponse apiResponse)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
            _apiResponse = apiResponse;
        }
        
        /// <summary>
        /// Retrieves all students from the database.
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpGet("AllStudents")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        
        public async Task<ActionResult<APIResponse>> GetStudents()
        {
            try
            {
                var students = await _repository.GetAllAsync();

                //var studentDtos = _mapper.Map<IEnumerable<StudentDTO>>(students);              
                _apiResponse.Data = _mapper.Map<IEnumerable<StudentDTO>>(students);
                _apiResponse.Status = true;
                _apiResponse.StatusCode = HttpStatusCode.OK;
                return Ok(_apiResponse);
            }
            catch (Exception ex)
            {
                _apiResponse.Error.Add(ex.Message);
                _apiResponse.Status = false;
                _apiResponse.StatusCode = HttpStatusCode.InternalServerError;
                _logger.LogError(ex, "Error occurred while retrieving all students.");
                return _apiResponse;
            }
        }

        /// <summary>
        /// Retrieves a student by their unique ID.
        /// </summary>
        [HttpGet("{id:int}", Name = "GetStudentById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> GetStudentById(int id)
        {
            if (id <= 0)
                return BadRequest("The ID must be a positive integer greater than 0.");

            try
            {
                var student = await _repository.GetAsync(student => student.Id == id);
                if (student == null)
                    return NotFound($"Student with ID {id} not found.");

                _apiResponse.Data = _mapper.Map<StudentDTO>(student);
                _apiResponse.Status = true;
                _apiResponse.StatusCode = HttpStatusCode.OK;
                return Ok(_apiResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving student by ID.");
                _apiResponse.Error.Add(ex.Message);
                _apiResponse.Status = false;
                _apiResponse.StatusCode = HttpStatusCode.InternalServerError;
                return _apiResponse;
            }
        }

        /// <summary>
        /// Retrieves a student by their name.
        /// </summary>
        [HttpGet("ByName/{name}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> GetStudentByName(string name)
        {
            if (string.IsNullOrEmpty(name))
                return BadRequest("The name must not be null or empty.");

            try
            {
                var student = await _repository.GetAsync(student => student.StudentName == name);
                if (student == null)
                    return NotFound($"Student with name '{name}' not found.");

                _apiResponse.Data = _mapper.Map<StudentDTO>(student);
                _apiResponse.Status = true;
                _apiResponse.StatusCode = HttpStatusCode.OK;
                return Ok(_apiResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving student by name.");
                _apiResponse.Error.Add(ex.Message);
                _apiResponse.Status = false;
                _apiResponse.StatusCode = HttpStatusCode.InternalServerError;
                return _apiResponse;
            }
        }

        /// <summary>
        /// Creates a new student.
        /// </summary>
        [HttpPost("Create")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<APIResponse>> CreateStudent([FromBody] StudentDTO studentDto)
        {
            if (!ModelState.IsValid)
            {
                _apiResponse.Status = false;
                _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                _apiResponse.Error = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList();
                return BadRequest(_apiResponse);
            }

            try
            {
                var student = _mapper.Map<Student>(studentDto);
                var createdStudent = await _repository.AddAsync(student);

                var result = _mapper.Map<StudentDTO>(createdStudent);
                studentDto.Id = result.Id;
                _apiResponse.Data = studentDto;
                _apiResponse.Status = true;
                _apiResponse.StatusCode = HttpStatusCode.OK;
                return CreatedAtRoute("GetStudentById", new { id = studentDto.Id },_apiResponse);
                
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating a new student.");
                _apiResponse.Error.Add(ex.Message);
                _apiResponse.Status = false;
                _apiResponse.StatusCode = HttpStatusCode.InternalServerError;
                return _apiResponse;
            }
        }

        /// <summary>
        /// Updates an existing student.
        /// </summary>
        [HttpPut]
        [Route("Update")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> UpdateStudent(StudentDTO dto)
        {
            if (dto == null || dto.Id <= 0)
            {
                return BadRequest("Invalid role data or ID.");
            }

            try
            {
                var existingRole = await _repository.GetAsync(r => r.Id == dto.Id, true);
                if (existingRole == null)
                {
                    return NotFound($"Role not found with ID: {dto.Id}");
                }

                // Preserve existing data and update properties from DTO
                _mapper.Map(dto, existingRole);
               

                await _repository.UpdateAsync(existingRole);

                _apiResponse.Data = _mapper.Map<StudentDTO>(existingRole);
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
        /// Applies partial updates to a student.
        /// </summary>
        [HttpPatch("{id:int}/PatchStudent")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<APIResponse>> PatchStudent(int id, [FromBody] JsonPatchDocument<StudentDTO> patchDoc)
        {
            if (patchDoc == null)
            {
                _apiResponse.Status = false;
                _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                _apiResponse.Error = new List<string> { "Patch document is null." };
                return BadRequest(_apiResponse);
            }

            try
            {
                var student = await _repository.GetAsync(n => n.Id == id);
                if (student == null)
                {
                    _apiResponse.Status = false;
                    _apiResponse.StatusCode = HttpStatusCode.NotFound;
                    _apiResponse.Error = new List<string> { "Student not found." };
                    return NotFound(_apiResponse);
                }

                var studentDto = _mapper.Map<StudentDTO>(student);
                patchDoc.ApplyTo(studentDto, ModelState);

                if (!ModelState.IsValid)
                {
                    _apiResponse.Status = false;
                    _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                    _apiResponse.Error = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList();
                    return BadRequest(_apiResponse);
                }

                _mapper.Map(studentDto, student);
                await _repository.UpdateAsync(student);

                _apiResponse.Data = _mapper.Map<StudentDTO>(student);
                _apiResponse.Status = true;
                _apiResponse.StatusCode = HttpStatusCode.OK;

                return Ok(_apiResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while patching the student.");
                _apiResponse.Status = false;
                _apiResponse.StatusCode = HttpStatusCode.InternalServerError;
                _apiResponse.Error.Add(ex.Message);
                return _apiResponse;
            }
        }
       


        /// <summary>
        /// Deletes a student by ID.
        /// </summary>
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<APIResponse>> DeleteStudent(int id)
        {
            if (id <= 0)
            {
                _apiResponse.Status = false;
                _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                _apiResponse.Error = new List<string> { "The ID must be a positive integer greater than 0." };
                return BadRequest(_apiResponse);
            }

            try
            {
                var student = await _repository.GetAsync(s => s.Id == id);
                if (student == null)
                {
                    _apiResponse.Status = false;
                    _apiResponse.StatusCode = HttpStatusCode.NotFound;
                    _apiResponse.Error = new List<string> { $"Student with ID {id} not found." };
                    return NotFound(_apiResponse);
                }

                await _repository.DeleteAsync(student);

                _apiResponse.Status = true;
                _apiResponse.StatusCode = HttpStatusCode.NoContent;

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting student.");
                _apiResponse.Status = false;
                _apiResponse.StatusCode = HttpStatusCode.InternalServerError;
                _apiResponse.Error.Add(ex.Message);
                return _apiResponse;
            }
        }

    }
}
