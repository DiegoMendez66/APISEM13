using APISEM13.Models;
using APISEM13.Models.Request;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APISEM13.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class StudentCustomController : ControllerBase
    {
        private readonly SchoolContext _context;

        public StudentCustomController(SchoolContext context)
        {
            _context = context;
        }

        // GET: api/student
        [HttpGet(Name ="GetByFilters")]
        public List<Student> GetByFilters(string firstName, string lastName, string email)
        {
            List<Student> response = _context.Students
                .Where(x => x.FirstName.Contains (firstName)
                && x.LastName.Contains(lastName)
                && x.Email.Contains(email)
                )
                .OrderByDescending(x => x.LastName)
                .ToList();
            return response;
        }

        [HttpGet(Name = "GetWithGrade")]
        public List<Student> GetWithGrade(string firstName, string grade)
        {
            List<Student> response = _context.Students.
                Include(x => x.Grade)
                .Where(x => x.FirstName.Contains(firstName)
                        || x.Grade.Name.Contains(grade))
                .OrderByDescending(x => x.LastName)
                .ToList();
            return response;
        }

        [HttpGet(Name = "GetEnrollment")]
        public List<Enrollment> GetEnrollment()
        {
           var response = _context.Enrollments.
                Include(x => x.Student)
                .ThenInclude(x => x.Grade)
                .ToList();
           return response;
        }

        // 6
        [HttpPost(Name = "UpdateContacts")]
        public void UpdateContacts(StudentRequestV1 request)
        {
            //Buscar al estudiante a editar
            var student = _context.Students.Find(request.Id);

            //Cambio los valores
            student.Email = request.Email;
            student.Phone = request.Phone;

            //Transacción
            _context.Entry(student).State = EntityState.Modified;
            _context.SaveChanges();
        }

        // 8
        [HttpPost(Name = "InsertByGrade")]
        public void InsertByGrade(StudentRequestV2 request)
        {
            /*foreach (var item in request.Students)
            {
                item.GradeID = request.GradeID;
                _context.Students.Add(item);
                _context.SaveChanges();
            }*/

            var students = request.Students.Select(x => new Student 
            {
                Email = x.Email,
                FirstName = x.FirstName,
                Phone = x.Phone,
                Grade = x.Grade,
                GradeID = request.GradeID
            }).ToList();

            _context.Students.AddRange(students); //inserta en grupo
            _context.SaveChanges();
        }
    }
}
