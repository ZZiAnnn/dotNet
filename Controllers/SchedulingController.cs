using ExamSchedulingSystem.Service;
using Microsoft.AspNetCore.Mvc;

namespace ExamSchedulingSystem.Controllers
{
    [Route("/api/user/{action}")]
    public class SchedulingController : Controller
    {
        private SchedulingService schedulingService;
        public SchedulingController(SchedulingService schedulingService)
        {
            this.schedulingService = schedulingService;
        }

        [HttpGet]
        public ActionResult SchedulingExam()
        {
            schedulingService.schedulingExam(examId);
            return Ok(new { success="success", msg="msg", data = "1" });
        }
    }
}