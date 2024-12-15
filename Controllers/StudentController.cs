using CollegeAppWebAPI.Models;
using CollegeAppWebAPI.Models.Repository;
using CollegeAppWebAPI.Services;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace CollegeAppWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController : Controller
    {
        private readonly IStudentRepository studentRepository;

        public StudentController(IStudentRepository studentRepository)
        {
            this.studentRepository = studentRepository;
        }
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<Student>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Route("AllStudents")]
        public ActionResult<IEnumerable<StudentDTO>> GetStudent()
        {
            try
            {
                var students = studentRepository.GetAllStudents();
                var studentDtos = students.ToDtoList();
                return Ok(students);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred. Please try again later.");
            }
        }
        [HttpGet("{id:int}", Name = "GetStudentById")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Student))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<StudentDTO> GetStudentById(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest("The ID must be a positive integer greater than 0.");
                }

                var student = studentRepository.GetStudentById(id);
                if (student == null)
                {
                    return NotFound();
                }

                // Use the extension method to map the entity to a DTO
                var studentDto = student.ToDto();

                return Ok(studentDto);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred. Please try again later.");
            }
        }

        [HttpGet("{name:alpha}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Student))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]


        public ActionResult<StudentDTO> GetStudentByName(string name)
        {
            
            
            if (string.IsNullOrEmpty(name))
                return BadRequest();
            var student = studentRepository.GetStudentByName(name);
            
            if (student == null)
            {
                return NotFound($"Student with name {name} is not found"); // Return 404 if the student is not found
            }
            var studentDto = student.ToDto();
            return Ok(studentDto);
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult DeleteStudent(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest("The ID must be a positive integer greater than 0.");
                }

                var success = studentRepository.DeleteStudent(id);
                if (!success)
                {
                    return NotFound();
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred. Please try again later.");
            }
        }
        [HttpPost]
        [Route("Create")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(Student))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        public ActionResult<StudentDTO> CreateStudent([FromBody] StudentDTO studentDto)
        {
            try
            {
                if (studentDto == null || string.IsNullOrWhiteSpace(studentDto.StudentName))
                {
                    return BadRequest("Invalid student data.");
                }
                // Validate DateOfBirth
                if (studentDto.DateOfBirth > DateTime.Now)
                {
                    return BadRequest("Date of Birth cannot be in the future.");
                }

                // Optional: Add more date range checks if needed
                if (studentDto.DateOfBirth < DateTime.Now.AddYears(-120))
                {
                    return BadRequest("Date of Birth is too far in the past.");
                }
                // Map DTO to entity
                var newStudent = studentDto.ToEntity();

                // Save to repository
                var createdStudent = studentRepository.AddStudent(newStudent);

                // Map entity to DTO
                var createdStudentDto = createdStudent.ToDto();

                return CreatedAtRoute("GetStudentById", new { id = createdStudentDto.Id }, createdStudentDto);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred. Please try again later.");
            }
        }
        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult UpdateStudent(int id, [FromBody] StudentDTO studentDto)
        {
            try
            {
                if (studentDto == null || id <= 0)
                {
                    return BadRequest("Invalid student data.");
                }

                var existingStudent = studentRepository.GetStudentById(id);
                if (existingStudent == null)
                {
                    return NotFound($"Student with ID {id} not found.");
                }

                // Map DTO to entity and update repository
                var updatedStudent = studentDto.ToEntity();
                var updatedEntity = studentRepository.UpdateStudent(id, updatedStudent);

                if (updatedEntity == null)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, "Update failed.");
                }

                // Return updated DTO
                var updatedStudentDto = updatedEntity.ToDto();
                return Ok(updatedStudentDto);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred.");
            }
        }

        [HttpPatch("{id:int}/PatchStudent")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult PatchStudent(int id, [FromBody] JsonPatchDocument<StudentDTO> patchDoc)
        {
            try
            {
                if (patchDoc == null || id <=0)
                {
                    return BadRequest("Invalid patch document.");
                }

                var existingStudent = studentRepository.GetStudentById(id);
                if (existingStudent == null)
                {
                    return NotFound($"Student with ID {id} not found.");
                }

                // Map entity to DTO
                var studentDto = existingStudent.ToDto();

                // Apply the patch
                patchDoc.ApplyTo(studentDto, ModelState);

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Validate DTO (optional)
                if (string.IsNullOrWhiteSpace(studentDto.StudentName))
                {
                    return BadRequest("Student name cannot be empty.");
                }

                // Map DTO back to entity
                var updatedStudent = studentDto.ToEntity();

                // Update repository
                var updatedEntity = studentRepository.UpdateStudent(id, updatedStudent);

                return Ok(updatedEntity.ToDto());
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred.");
            }
        }



    }
}
