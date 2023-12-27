namespace ExamSchedulingSystem
{
    public class Room
    {
        Guid id { get; set; }
        public string? name { get; set; }
        public int? campus { get; set; }//校区
        public int? buildingNo { get; set; }//楼号
        public int? roomNo { get; set; }//房间号
    }
}