namespace ExamSchedulingSystem.db
{
    public class Room
    {
        public Guid id { get; set; }
        private string? name { get; set; }
        public Campus? campus { get; set; }//校区
        private int? buildingNo { get; set; }//楼号
        private int? roomNo { get; set; }//房间号
    }
}