using ExamSchedulingSystem.db;
using Microsoft.EntityFrameworkCore;

namespace ExamSchedulingSystem.Service
{
    public class SchedulingService 
    {
        private SqlContext _context;
        public SchedulingService(SqlContext sqlContext) 
        { 
            this._context = sqlContext;
        }
        public List<ScheResult> schedulingExam(Guid id)
        {
            List<Room> rooms = (from r in _context.Room
                               join er in _context.ExamRoom on r.id equals er.roomId
                               where er.examId == id
                               select r).ToList();
            Console.WriteLine(rooms);

            List<Teacher> teachers = _context.Teacher.Where(t => t.ExamId == id).ToList();
            Console.WriteLine(teachers);
            return schedule(id, teachers, rooms);
        }

        public List<ScheResult> schedule(Guid ExamId, List<Teacher> teachers,List<Room> rooms)
        {
            List<ScheResult> scheResults = new List<ScheResult>();

            // 每个校区分开分配
            foreach(Campus campus in Enum.GetValues(typeof(Campus)))
            {
                // 1 获取该校区的老师和考场
                var campusTeachers = teachers.Where(t => t.ExamCampus == campus).ToList();
                var campusRooms = rooms.Where(r => r.campus == campus).ToList();

                string res = canSchedule(campusTeachers,campusRooms);

                if(res!=""){
                    scheResults.Clear();
                    scheResults.Add(new ScheResult{
                        AllocationStatus = Result.无法分配,
                        AllocationRuleDescription = campus.ToString() + "无法分配, " + res
                    });
                    return scheResults;
                }
                
                // 2 随机打乱考场
                Random random = new Random();
                campusRooms = campusRooms.OrderBy(r => random.Next()).ToList();

                // 再遍历一次考场列表、如果前后考场考室连续且前考场与后考场的下一考场不连续、交换前后的考场
                for(int i = 0; i < campusRooms.Count - 1; i++)
                {
                    if(campusRooms[i].buildingNo == campusRooms[i + 1].buildingNo && campusRooms[i].roomNo + 1 == campusRooms[i + 1].roomNo)
                    {
                        if(i + 2 < campusRooms.Count && campusRooms[i + 1].buildingNo != campusRooms[i + 2].buildingNo)
                        {
                            var temp = campusRooms[i];
                            campusRooms[i] = campusRooms[i + 1];
                            campusRooms[i + 1] = temp;
                        }
                    }
                }

                // 3 数据准备
                int alreadyAllocatedTeacherCount = 0;
                int lastGender = 0;

                // 把老师分成新老两组老师
                List<Teacher> newTeachers = campusTeachers.Where(t => t.ParticipatedLastYear == false).ToList();
                List<Teacher> oldTeachers = campusTeachers.Where(t => t.ParticipatedLastYear == true).ToList();
                
                Dictionary<Guid, int> newDeptTeacherCount = new Dictionary<Guid, int>();
                Dictionary<Guid, int> oldDeptTeacherCount = new Dictionary<Guid, int>();
                // 分别记录两组老师的不同部门的剩余老师数量
                foreach(Guid deptId in campusTeachers.Select(t => t.DeptId).Distinct().ToList())
                {
                    newDeptTeacherCount.Add(deptId, newTeachers.Where(t => t.DeptId == deptId).Count());
                    oldDeptTeacherCount.Add(deptId, oldTeachers.Where(t => t.DeptId == deptId).Count());
                }

                // 4 依次为考场分配老师
                while(alreadyAllocatedTeacherCount != campusRooms.Count()){

                    // 4.1 找老人最多且还有新人在其他部门的部门
                    Guid maxOldDeptId = Guid.Empty;
                    int maxOldDeptTeacherCount = 0;
                    int solutionType = 0;
                    foreach(Guid deptId in campusTeachers.Select(t => t.DeptId).Distinct().ToList())
                    {
                        if(oldDeptTeacherCount[deptId] > maxOldDeptTeacherCount)
                        {
                            foreach(Guid deptId2 in campusTeachers.Select(t => t.DeptId).Distinct().ToList())
                            {
                                if(newDeptTeacherCount[deptId2] > 0 && deptId2 != deptId)
                                {
                                    maxOldDeptId = deptId;
                                    maxOldDeptTeacherCount = oldDeptTeacherCount[deptId];
                                    break;
                                }
                            }
                        }
                    }

                    // 4.2 次优解, 选老人最多且还有老人在其他部门的部门
                    if(maxOldDeptId == Guid.Empty){
                        solutionType = 1;
                        foreach(Guid deptId in campusTeachers.Select(t => t.DeptId).Distinct().ToList())
                        {
                            if(oldDeptTeacherCount[deptId] > maxOldDeptTeacherCount)
                            {
                                foreach(Guid deptId2 in campusTeachers.Select(t => t.DeptId).Distinct().ToList())
                                {
                                    if(oldDeptTeacherCount[deptId2] > 0 && deptId2 != deptId)
                                    {
                                        maxOldDeptId = deptId;
                                        maxOldDeptTeacherCount = oldDeptTeacherCount[deptId];
                                        break;
                                    }
                                }
                            }
                        }
                    }

                    // 4.3 无解
                    if(maxOldDeptId == Guid.Empty){
                        scheResults.Clear();
                        scheResults.Add(new ScheResult{
                            AllocationStatus = Result.无法分配,
                            AllocationRuleDescription = campus.ToString() + "无法分配, " + "无法找到合适的老师"
                        });
                        return scheResults;
                    }

                    // 选出考场
                    ScheResult scheResult = new ScheResult();
                    scheResult.ExamId = ExamId;
                    scheResult.ExamRoomId = campusRooms[alreadyAllocatedTeacherCount].getid();

                    // 找到该部门的所有监考过的老师
                    List<Teacher> maxOldDeptTeachers = oldTeachers.Where(t => t.DeptId == maxOldDeptId).ToList();

                    // 4.1.1 & 4.2.1从该部门的老师中尽量找到一个性别和lastGender不同的老师
                    Teacher select1 = maxOldDeptTeachers[0];
                    foreach(Teacher t in maxOldDeptTeachers){
                        if(t.Gender != lastGender){
                            select1 = t;
                            break;
                        }
                    }
                    scheResult.Invigilator1Id = select1.getid();
                    lastGender = select1.Gender;
                    // 移出 oldDeptTeacherCount 和 oldTeachers
                    oldDeptTeacherCount[maxOldDeptId]--;
                    oldTeachers.Remove(select1);

                    if(solutionType == 0){
                        // 4.1.2 找新人人数最多的不同部门
                        Guid maxNewDeptId = Guid.Empty;
                        int maxNewDeptTeacherCount = 0;
                        foreach(Guid deptId in campusTeachers.Select(t => t.DeptId).Distinct().ToList())
                        {
                            if(newDeptTeacherCount[deptId] > maxNewDeptTeacherCount && deptId != maxOldDeptId)
                            {
                                maxNewDeptId = deptId;
                                maxNewDeptTeacherCount = newDeptTeacherCount[deptId];
                            }
                        }

                        // 找到该部门的所有没监考过的老师
                        List<Teacher> maxNewDeptTeachers = newTeachers.Where(t => t.DeptId == maxNewDeptId).ToList();

                        // 从该部门的老师中尽量找到一个性别和select1不同的老师
                        Teacher select2 = maxNewDeptTeachers[0];
                        foreach(Teacher t in maxNewDeptTeachers){
                            if(t.Gender != select1.Gender){
                                select2 = t;
                                break;
                            }
                        }
                        scheResult.Invigilator2Id = select2.getid();
                        // 移出 newDeptTeacherCount 和 newTeachers
                        newDeptTeacherCount[maxNewDeptId]--;
                        newTeachers.Remove(select2);

                    }else{
                        // 4.2.2找老人人数最多的不同部门
                        Guid maxOldDeptId2 = Guid.Empty;
                        int maxOldDeptTeacherCount2 = 0;
                        foreach(Guid deptId in campusTeachers.Select(t => t.DeptId).Distinct().ToList())
                        {
                            if(oldDeptTeacherCount[deptId] > maxOldDeptTeacherCount2 && deptId != maxOldDeptId)
                            {
                                maxOldDeptId2 = deptId;
                                maxOldDeptTeacherCount2 = oldDeptTeacherCount[deptId];
                            }
                        }

                        // 找到该部门的所有的老师
                        List<Teacher> maxOldDeptTeachers2 = oldTeachers.Where(t => t.DeptId == maxOldDeptId2).ToList();

                        // 从该部门的老师中尽量找到一个性别和select1不同的老师
                        Teacher select2 = maxOldDeptTeachers2[0];
                        foreach(Teacher t in maxOldDeptTeachers2){
                            if(t.Gender != select1.Gender){
                                select2 = t;
                                break;
                            }
                        }
                        scheResult.Invigilator2Id = select2.getid();
                        // 移出 oldDeptTeacherCount 和 oldTeachers
                        oldDeptTeacherCount[maxOldDeptId2]--;
                        oldTeachers.Remove(select2);

                    }
                    
                    // 4.1.3 & 4.2.3添加到结果
                    scheResults.Add(scheResult);

                }

            }

            return scheResults;

        }

        public string canSchedule(List<Teacher> teachers, List<Room> rooms)
        {
            // 1 判断必须的条件
            if (rooms == null || teachers == null)
            {
                return "考场或老师为空";
            }
            // 2 报名老师数量>=需求人数
            if (teachers.Count < rooms.Count * 2)
            {
                return "报名老师数量不足";
            }
            // 3 除最大人数部门之外的所有部门总人数不能低于需求人数的一半
            int maxDeptTeacherCount = 0;
            foreach (Guid deptId in teachers.Select(t => t.DeptId).Distinct().ToList())
            {
                int deptTeacherCount = teachers.Where(t => t.DeptId == deptId).Count();
                if (deptTeacherCount > maxDeptTeacherCount)
                {
                    maxDeptTeacherCount = deptTeacherCount;
                }
            }
            int otherDeptTeacherCount = teachers.Count - maxDeptTeacherCount;
            if (otherDeptTeacherCount < rooms.Count)
            {
                return "除最大人数部门之外的所有部门总人数不足";
            }
            // 4 报名的监考过的老师>=考场数量
            var participatedTeachers = teachers.Where(t => t.ParticipatedLastYear == true).ToList();
            if (participatedTeachers.Count < rooms.Count)
            {
                return "报名的监考过的老师数量不足";
            }
            return "";
        }
    }
}
