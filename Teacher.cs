namespace ExamSchedulingSystem
{

    public enum Campus
    {
        思明校区,
        海韵校区,
        翔安校区
    }

    public class Teacher
    {
        public Guid id { get; set; }
        public Guid DeptId { get; set; }//工作单位
        public string Gender { get; set; }
        public Campus ExamCampus { get; set; }//监考校区
        public bool ParticipatedLastYear { get; set; }
        public int ExamId { get; set; }
    }

}
