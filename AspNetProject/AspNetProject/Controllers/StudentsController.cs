using AspNetProject.UOW;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentAdminPortal.API.DomainModels;
using StudentAdminPortal.API.Repositories;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace StudentAdminPortal.API.Controllers
{
    [ApiController]
    public class StudentsController : Controller
    {
        //private readonly IStudentRepository studentRepository;
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly IImageRepository imageRepository;

        public StudentsController(IUnitOfWork unitOfWork, IMapper mapper,
            IImageRepository imageRepository)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.imageRepository = imageRepository;
        }

        [HttpGet]
        [Route("[controller]")]
        public IActionResult GetAllStudentsAsync()
        {
            var students =  unitOfWork.Student.GetMuliple(null,null, x => x.Include(g => g.Gender).Include(a => a.Address),true); 

            return Ok(mapper.Map<List<Student>>(students));
        }

        [HttpGet]
        [Route("[controller]/{studentId:guid}"), ActionName("GetStudentAsync")]
        public  IActionResult GetStudentAsync([FromRoute] Guid studentId)
        {
            // Fetch Student Details
            var student =  unitOfWork.Student.GetFirstOrDefault(s=>s.Id == studentId, null, x => x.Include(g => g.Gender).Include(a=>a.Address), true);
            
            // Return Student
            if (student == null)
            {
                return NotFound();
            }

            return Ok(mapper.Map<Student>(student));
        }

        //[HttpPut]
        //[Route("[controller]/{studentId:guid}")]
        //public async Task<IActionResult> UpdateStudentAsync([FromRoute] Guid studentId, [FromBody] UpdateStudentRequest request)
        //{
        //    if (await studentRepository.Exists(studentId))
        //    {
        //        // Update Details
        //        var updatedStudent = await studentRepository.UpdateStudent(studentId, mapper.Map<DataModels.Student>(request));

        //        if (updatedStudent != null)
        //        {
        //            return Ok(mapper.Map<Student>(updatedStudent));
        //        }
        //    }
        //    return NotFound();
        //}

        [HttpDelete]
        [Route("[controller]/{studentId:guid}")]
        public IActionResult DeleteStudentAsync([FromRoute] Guid studentId)
        {
            if (unitOfWork.Student.Exists(x => x.Id == studentId))
            {
                var student =  unitOfWork.Student.Delete(studentId);
                unitOfWork.Save();
                return Ok(mapper.Map<Student>(student));
            }

            return NotFound();
        }

        [HttpPost]
        [Route("[controller]/Add")]
        public async Task<IActionResult> AddStudentAsync([FromBody] AddStudentRequest request)
        {
            var student = await unitOfWork.Student.AddStudent(mapper.Map<DataModels.Student>(request));
            return CreatedAtAction(nameof(GetStudentAsync), new { studentId = student.Id },
                mapper.Map<Student>(student));
        }

        [HttpPost]
        [Route("[controller]/{studentId:guid}/upload-image")]
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
