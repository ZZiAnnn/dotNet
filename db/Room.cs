namespace ExamSchedulingSystem.db
{
    public class Room
    {
        public Guid id { get; set; }
        public string? name { get; set; }
        public Campus? campus { get; set; }//У��
        public int? buildingNo { get; set; }//¥��
        public int? roomNo { get; set; }//�����

        public Guid getid()
        {
            return id;
        }
    }
}