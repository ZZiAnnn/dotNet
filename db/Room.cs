namespace ExamSchedulingSystem.db
{
    public class Room
    {
        public Guid id { get; set; }
        private string? name { get; set; }
        public Campus? campus { get; set; }//У��
        private int? buildingNo { get; set; }//¥��
        private int? roomNo { get; set; }//�����
    }
}