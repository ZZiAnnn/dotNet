namespace ExamSchedulingSystem.db
{

    public enum Campus
    {
        思明校区,
        海韵校区,
        翔安校区
    }

    public class Teacher
    {
        private Guid id { get; set; }
        public Guid DeptId { get; set; }//工作单位
        public int Gender { get; set; }
        public Campus ExamCampus { get; set; }//监考校区
        public bool ParticipatedLastYear { get; set; }
        public Guid ExamId { get; set; }
    }

}
