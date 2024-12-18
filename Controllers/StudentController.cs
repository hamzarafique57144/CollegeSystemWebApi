using AutoMapper;
using CollegeAppWebAPI.Models;
using CollegeAppWebAPI.Models.Data;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CollegeAppWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        private readonly CollegeDbContext _context;
        private readonly ILogger<StudentController> _logger;
        private readonly IMapper _mapper;

        public StudentController(CollegeDbContext context, ILogger<StudentController> logger, IMapper mapper)
        {
            _context = context;
            _logger = logger;
            _mapper = mapper;
        }

        /// <summary>
        /// Retrieves all students from the database.
        /// </summary>
        [HttpGet("AllStudents")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<StudentDTO>>> GetStudents()
        {
            try
            {
                var students = await _context.Students.ToListAsync();
                var studentDtos = _mapper.Map<IEnumerable<StudentDTO>>(students);
                return Ok(studentDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving all students.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred. Please try again later.");
            }
        }

        /// <summary>
        /// Retrieves a student by their unique ID.
        /// </summary>
        [HttpGet("{id:int}", Name = "GetStudentById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<StudentDTO>> GetStudentById(int id)
        {
            if (id <= 0)
                return BadRequest("The ID must be a positive integer greater than 0.");

            try
            {
                var student = await _context.Students.FindAsync(id);
                if (student == null)
                    return NotFound($"Student with ID {id} not found.");

                var studentDto = _mapper.Map<StudentDTO>(student);
                return Ok(studentDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving student by ID.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred. Please try again later.");
            }
        }

        /// <summary>
        /// Retrieves a student by their name.
        /// </summary>
        [HttpGet("{name:alpha}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<StudentDTO>> GetStudentByName(string name)
        {
            if (string.IsNullOrEmpty(name))
                return BadRequest("Name cannot be empty.");

            try
            {
                var student = await _context.Students.FirstOrDefaultAsync(s => s.StudentName == name);
                if (student == null)
                    return NotFound($"Student with name '{name}' not found.");

                var studentDto = _mapper.Map<StudentDTO>(student);
                return Ok(studentDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving student by name.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred. Please try again later.");
            }
        }

        /// <summary>
        /// Creates a new student.
        /// </summary>
        [HttpPost("Create")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<StudentDTO>> CreateStudent([FromBody] StudentDTO studentDto)
        {
            if (studentDto == null)
                return BadRequest("Invalid student data.");

            try
            {
                var newStudent = _mapper.Map<Student>(studentDto);

                await _context.Students.AddAsync(newStudent);
                await _context.SaveChangesAsync();

                var createdStudentDto = _mapper.Map<StudentDTO>(newStudent);
                return CreatedAtRoute("GetStudentById", new { id = createdStudentDto.Id }, createdStudentDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating a new student.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred. Please try again later.");
            }
        }

        /// <summary>
        /// Updates an existing student.
        /// </summary>
        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateStudent(int id, [FromBody] StudentDTO studentDto)
        {
            if (id <= 0 || studentDto == null)
                return BadRequest("Invalid input data.");

            try
            {
                // Retrieve the student entity from the database
                var student = await _context.Students.FirstOrDefaultAsync(n => n.Id == id);
                if (student == null)
                    return NotFound($"Student with ID {id} not found.");

                // Ignore the Id from the DTO and use the ID from the route
                studentDto.Id = id;

                // Map changes from DTO to the existing student entity
                _mapper.Map(studentDto, student);

                // Save changes to the database
                await _context.SaveChangesAsync();

                // Return the updated student data as a DTO
                return Ok(_mapper.Map<StudentDTO>(student));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating student.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred. Please try again later.");
            }
        }



        /// <summary>
        /// Applies partial updates to a student.
        /// </summary>
        [HttpPatch("{id:int}/PatchStudent")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> PatchStudent(int id, [FromBody] JsonPatchDocument<StudentDTO> patchDoc)
        {
            if (patchDoc == null)
                return BadRequest("Invalid patch document.");

            try
            {
                var student = await _context.Students.FindAsync(id);
                if (student == null)
                    return NotFound($"Student with ID {id} not found.");

                var studentDto = _mapper.Map<StudentDTO>(student);

                patchDoc.ApplyTo(studentDto, ModelState);
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                _mapper.Map(studentDto, student);
                await _context.SaveChangesAsync();

                return Ok(_mapper.Map<StudentDTO>(student));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while patching student.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred. Please try again later.");
            }
        }

        /// <summary>
        /// Deletes a student by ID.
        /// </summary>
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteStudent(int id)
        {
            if (id <= 0)
                return BadRequest("The ID must be a positive integer greater than 0.");

            try
            {
                var student = await _context.Students.FindAsync(id);
                if (student == null)
                    return NotFound($"Student with ID {id} not found.");

                _context.Students.Remove(student);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting student.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred. Please try again later.");
            }
        }
    }
}
