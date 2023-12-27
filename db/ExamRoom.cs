namespace ExamSchedulingSystem.db
{
    public class ExamRoom
    {
        public Guid id { get; set; }
        public Guid roomId { get; set; }
        public Guid examId { get; set; }
    }
}
