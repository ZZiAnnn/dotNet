namespace ExamSchedulingSystem.db
{
    public class Room
    {
        public Guid id { get; set; }
        public string? name { get; set; }
        public Campus? campus { get; set; }//校区
        public int? buildingNo { get; set; }//楼号
        public int? roomNo { get; set; }//房间号

        public Guid getid()
        {
            return id;
        }
    }
}