namespace ExamSchedulingSystem
{
    public class Exam
    {
        Guid Id { get; set; }
        public string Name { get; set; }
        public DateTime RegistrationStartTime;//报名开始时间
        public DateTime RegistrationEndTime;//报名结束时间
        public DateTime ExamStartTime;//考试开始时间
        public DateTime ExamEndTime;//考试开始时间
        public DateTime ScheduleStartTime;//排考开始时间
        public DateTime ScheduleEndTime;//排考结束时间
    }
}
