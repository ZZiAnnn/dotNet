namespace ExamSchedulingSystem
{
    public class Room
    {
        Guid id { get; set; }
        public string? name { get; set; }
        public int? campus { get; set; }//У��
        public int? buildingNo { get; set; }//¥��
        public int? roomNo { get; set; }//�����
    }
}