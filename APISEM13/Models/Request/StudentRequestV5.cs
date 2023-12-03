namespace APISEM13.Models.Request
{
    public class StudentRequestV5
    {
        public int StudentID { get; set; }
        public List<Course> Courses { get; set; }
    }
}
