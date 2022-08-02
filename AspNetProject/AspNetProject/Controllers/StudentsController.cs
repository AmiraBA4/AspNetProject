using AspNetProject.UOW;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentAdminPortal.API.DomainModels;
using StudentAdminPortal.API.Repositories;

namespace StudentAdminPortal.API.Controllers
{
    [Route("[controller]")]
    [ApiController]  
    public class StudentsController : Controller
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly IImageRepository imageRepository;

        /// <summary>
        /// StudentsController
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="mapper"></param>
        /// <param name="imageRepository"></param>
        public StudentsController(IUnitOfWork unitOfWork, IMapper mapper,
            IImageRepository imageRepository)
        {
            this.unitOfWork = unitOfWork?? throw new ArgumentNullException(nameof(unitOfWork));
            this.mapper = mapper;
            this.imageRepository = imageRepository ?? throw new ArgumentNullException(nameof(imageRepository));
        }

        [HttpGet]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllStudentsAsync()
        {
            var students =  await unitOfWork.Student.GetMuliple(null,null, x => x.Include(g => g.Gender).Include(a => a.Address),true); 

            return Ok(mapper.Map<List<StudentModel>>(students));
        }

        [HttpGet("GetStudentAsync/{studentId:guid}")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetStudentAsync([FromRoute] Guid studentId)
        {
            // Fetch Student Details
            var student =  await unitOfWork.Student.GetFirstOrDefault(s=>s.Id == studentId, null, x => x.Include(g => g.Gender).Include(a=>a.Address), true);
            
            // Return Student
            if (student == null)
            {
                return NotFound();
            }

            return Ok(mapper.Map<StudentModel>(student));
        }

        [HttpPut]
        [Route("{studentId:guid}")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateStudentAsync([FromRoute] Guid studentId, [FromBody] UpdateStudentRequestModel request)
        {
            if ( unitOfWork.Student.Exists(x => x.Id == studentId))
            {
                // Update Details
                var updatedStudent = await unitOfWork.Student.UpdateStudent(studentId, mapper.Map<DataModels.Student>(request));

                if (updatedStudent != null)
                {
                    return Ok(mapper.Map<StudentModel>(updatedStudent));
                }
            }
            return NotFound();
        }

        [HttpDelete]
        [Route("{studentId:guid}")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteStudentAsync([FromRoute] Guid studentId)
        {
            if (unitOfWork.Student.Exists(x => x.Id == studentId))
            {
                var student =  await unitOfWork.Student.Delete(studentId);
                unitOfWork.Save();
                return Ok(mapper.Map<StudentModel>(student));
            }

            return NotFound();
        }

        [HttpPost]
        [Route("Add")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<IActionResult> AddStudentAsync([FromBody] AddStudentRequestModel request)
        {
            var student = await unitOfWork.Student.AddStudent(mapper.Map<DataModels.Student>(request));
            return CreatedAtAction(nameof(GetStudentAsync), new { studentId = student.Id },
                mapper.Map<StudentModel>(student));
        }

        [HttpPost]
        [Route("{studentId:guid}/upload-image")]
        public async Task<IActionResult> UploadImage([FromRoute] Guid studentId, IFormFile profileImage)
        {
            var validExtensions = new List<string>
            {
               ".jpeg",
               ".png",
               ".gif",
               ".jpg"
            };

            if (profileImage != null && profileImage.Length > 0)
            {
                var extension = Path.GetExtension(profileImage.FileName);
                if (validExtensions.Contains(extension))
                {
                    if (unitOfWork.Student.Exists(x=>x.Id == studentId))
                    {
                        var fileName = Guid.NewGuid() + Path.GetExtension(profileImage.FileName);

                        var fileImagePath = await imageRepository.Upload(profileImage, fileName);

                        if (await unitOfWork.Student.UpdateProfileImage(studentId, fileImagePath))
                        {
                            return Ok(fileImagePath);
                        }

                        return StatusCode(StatusCodes.Status500InternalServerError, "Error uploading image");
                    }
                }

                return BadRequest("This is not a valid Image format");
            }

            return NotFound();
        }
    }
}
