namespace APISEM13.Models
{
    public class Enrollment
    {
        public int EnrollmentID { get; set; }
        public DateTime Date { get; set; }

        // llaves foraneas
        public Student Student { get; set; }
        public int StudentID { get; set; }
        public Course Course { get; set; }
        public int CourseID { get; set; }
    }
}
