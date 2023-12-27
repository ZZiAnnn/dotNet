namespace ExamSchedulingSystem
{

    public enum Result
    {
        正确分配,
        分配但有问题,
        无法分配
    }

    public class ScheResult
    {
        public Guid Id { get; set; }
        // 考试id
        public Guid ExamId { get; set; }

        // 考场id
        public Guid ExamRoomId { get; set; }

        // 监考1报名ID 
        public Guid Invigilator1Id { get; set; }

        // 监考2报名ID 
        public Guid Invigilator2Id { get; set; }

        // 是否正确分配
        public Result AllocationStatus { get; set; }

        // 分配规则描述
        public string AllocationRuleDescription { get; set; }
    }

}