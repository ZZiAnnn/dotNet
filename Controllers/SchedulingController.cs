using ExamSchedulingSystem.Service;
using Microsoft.AspNetCore.Mvc;

namespace ExamSchedulingSystem.Controllers
{
    public class SchedulingController : Controller
    {
        private SchedulingService schedulingService;
        public SchedulingController(SchedulingService schedulingService)
        {
            this.schedulingService = schedulingService;
        }

        [HttpPost("/scheduling/{id}")]
        public ActionResult SchedulingExam(Guid id)
        {
            var ret=this.schedulingService.schedulingExam(id);
            return Ok(new { 
                success="success",
                msg="Ok", 
                data=ret
            });
        }
    }
}