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
        [HttpGet(Name = "GetCustomStudents")]
        public List<Student> GetCustomStudents()
        {
            var response = _context.Students.ToList();
            return response;

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

        // 1
        [HttpPost(Name = "InsertCourse")]
        public void InsertCourse(Course course)
        {
            course.IsActive = true;
            _context.Courses.Add(course);
            _context.SaveChanges();
        }

        // 2
        [HttpDelete(Name = "DeleteCourse")]
        public void DeleteCourse(int Id)
        {
            var course = _context.Courses.Where(x => x.IsActive == true && x.CourseID == Id).FirstOrDefault();
            course.IsActive = false;
            _context.Entry(course).State = EntityState.Modified;
            _context.SaveChanges();
        }

        // 3
        [HttpPost(Name = "InsertGrade")]
        public void InsertGrade(Grade grade)
        {
            grade.IsActive = true;
            _context.Grades.Add(grade);
            _context.SaveChanges();
        }

        // 4
        [HttpDelete(Name = "DeleteGrade")]
        public void DeleteGrade(int Id)
        {
            var grade = _context.Grades.Where(x => x.IsActive == true && x.GradeID == Id).FirstOrDefault();
            grade.IsActive = false;
            _context.Entry(grade).State = EntityState.Modified;
            _context.SaveChanges();
        }

        // 5
        [HttpPost(Name = "InsertStudent")]
        public void InsertStudent(Student student)
        {
            _context.Students.Add(student);
            _context.SaveChanges();
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

        //7
        [HttpPost(Name = "UpdatePersonalData")]
        public void UpdatePersonalData(StudentRequestV3 request)
        {
            var student = _context.Students.Find(request.StudentID);

            student.FirstName = request.FirstName;
            student.LastName = request.LastName;

            _context.Entry(student).State = EntityState.Modified;
            _context.SaveChanges();
        }

        // 8
        [HttpPost(Name = "InsertByGrade")]
        public void InsertByGrade(StudentRequestV2 request)
        {
            var students = request.Students.Select(x => new Student 
            {
                Email = x.Email,
                FirstName = x.FirstName,
                LastName = x.LastName,
                Phone = x.Phone,
                Grade = x.Grade,
                GradeID = request.GradeID
            }).ToList();

            _context.Students.AddRange(students); //inserta en grupo
            _context.SaveChanges();
        }

        // 9
        [HttpDelete(Name = "DeleteCourseList")]
        public void DeleteCourseList(StudentRequestV4 request)
        {
            var course = request.Courses.Select(x => x.CourseID).ToList();

            var courseToDelete = _context.Courses.Where(x => course.Contains(x.CourseID)).ToList();
            courseToDelete.ForEach(x => x.IsActive = false);

            _context.Courses.UpdateRange(courseToDelete);
            _context.SaveChanges();
        }

        // 10
        [HttpPost(Name = "InsertEnrollment")]
        public void InsertEnrollment(StudentRequestV5 request)
        {
            var student = _context.Students.FirstOrDefault(x => x.StudentID == request.StudentID);

            var courseid = request.Courses.Select(x => x.CourseID).ToList();

            var course= _context.Courses.Where(x => courseid.Contains(x.CourseID)).ToList();

            var enrollments = course.Select(course => new Enrollment
            {
                Student = student,
                Course = course,
                Date = DateTime.Now

            }).ToList();

            _context.Enrollments.AddRange(enrollments);
            _context.SaveChanges();
        }
    }
}
