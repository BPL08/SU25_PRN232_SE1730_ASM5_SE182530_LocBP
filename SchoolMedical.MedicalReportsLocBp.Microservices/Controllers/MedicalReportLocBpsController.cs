using MassTransit;
using MassTransit.Transports;
using Microsoft.AspNetCore.Mvc;
using SchoolMedical.BusinessObject.Shared.Models.LocBP.Models;
using SchoolMedical.Common.Shared.Models.LocBP;
using System.Threading.Tasks;





// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SchoolMedical.MedicalReportsLocBp.Microservices.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MedicalReportLocBpsController : ControllerBase
    {
        // GET: api/<MedicalReportLocBpsController>


        private readonly ILogger _logger;
        private readonly IBus _bus;
        private static List<SchoolMedical.BusinessObject.Shared.Models.LocBP.Models.MedicalReportsLocBp> MedicalReportsLocs = new List<SchoolMedical.BusinessObject.Shared.Models.LocBP.Models.MedicalReportsLocBp>
            {
                new SchoolMedical.BusinessObject.Shared.Models.LocBP.Models.MedicalReportsLocBp
                {
                    FkStudentProfileLocBpid = 1,
                    MedicalReportsLocBpid = 1,
                    IncidentDate = DateTime.Now.AddDays(10),
                    Location = "School Playground",
                    Description = "Minor injury during recess",
                    ActionTaken = "First aid administered",
                    ReportedDate = DateTime.Now.AddDays(9),
                    WitnessName = "John Doe",
                    ParentNotified = true,
                    ResolutionDate = DateTime.Now.AddDays(8),
                    Status = true,
                    SeverityLevel = 2
                }
            };
        public class MedicalReportDeletedEvent
        {
            public int MedicalReportsLocBpid { get; set; }
            public string EventType { get; set; } = "DELETE";
        }

        public MedicalReportLocBpsController(IBus bus,ILogger<MedicalReportLocBpsController> logger )
        {
            _bus = bus;
            _logger = logger;
           
        }
        [HttpGet]
        public IEnumerable<SchoolMedical.BusinessObject.Shared.Models.LocBP.Models.MedicalReportsLocBp> Get()
        {
            return MedicalReportsLocs;
        }

        // GET api/<MedicalReportLocBpsController>/5
        [HttpGet("{id}")]
        public SchoolMedical.BusinessObject.Shared.Models.LocBP.Models.MedicalReportsLocBp Get(int id)
        {
            return MedicalReportsLocs.Find(o=>o.MedicalReportsLocBpid == id);
        }

        // POST api/<MedicalReportLocBpsController>
        [HttpPost]
        public async Task<IActionResult> Post(SchoolMedical.BusinessObject.Shared.Models.LocBP.Models.MedicalReportsLocBp medical)
        {
            if (medical != null)
            {
                Uri uri = new Uri("rabbitmq://localhost/medicalQueue");
                var endPoint = await _bus.GetSendEndpoint(uri);

                await endPoint.Send(medical);

                string messageLog = string.Format("[{0}] PUBLISH data to RabbitMQ.orderQueue: {1}", DateTime.Now, Utilities.ConvertObjectToJSONString(medical));

                Utilities.WriteLoggerFile(messageLog);
                _logger.LogInformation("Medical report sent to the queue: {MedicalReport}", medical);




                MedicalReportsLocs.Add(medical);
                return Ok();
            }
            return BadRequest("Invalid medical report data.");
        }

        // PUT api/<MedicalReportLocBpsController>/5
      
            [HttpPut("{id}")]
            public async Task<IActionResult> Put(int id, [FromBody] SchoolMedical.BusinessObject.Shared.Models.LocBP.Models.MedicalReportsLocBp medical)
            {
                var existing = MedicalReportsLocs.FirstOrDefault(o => o.MedicalReportsLocBpid == id);
                if (existing == null)
                    return NotFound($"Medical report with ID {id} not found.");
                existing.FkStudentProfileLocBpid = medical.FkStudentProfileLocBpid;
                existing.IncidentDate = medical.IncidentDate;
                existing.Location = medical.Location;
                existing.Description = medical.Description;
                existing.ActionTaken = medical.ActionTaken;
                existing.ReportedDate = medical.ReportedDate;
                existing.WitnessName = medical.WitnessName;
                existing.ParentNotified = medical.ParentNotified;
                existing.ResolutionDate = medical.ResolutionDate;
                existing.Status = medical.Status;
                existing.SeverityLevel = medical.SeverityLevel;

                Uri uri = new Uri("rabbitmq://localhost/medicalQueue");
                var endPoint = await _bus.GetSendEndpoint(uri);
                await endPoint.Send(medical);

                string messageLog = $"[{DateTime.Now}] UPDATED and SENT via RabbitMQ: {Utilities.ConvertObjectToJSONString(medical)}";
                Utilities.WriteLoggerFile(messageLog);
                _logger.LogInformation("Medical report updated and sent to the queue: {MedicalReport}", medical);

                return Ok(existing);
            }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var report = MedicalReportsLocs.FirstOrDefault(o => o.MedicalReportsLocBpid == id);
                if (report == null)
                {
                    return NotFound($"Medical report with ID {id} not found.");
                }

           
                var deleteMessage = new MedicalReportDeletedEvent
                {
                    MedicalReportsLocBpid = id
                };

                Uri uri = new Uri("rabbitmq://localhost/medicalQueue");
                var endPoint = await _bus.GetSendEndpoint(uri);
                await endPoint.Send(deleteMessage);

                string messageLog = $"[{DateTime.Now}] DELETE message sent via RabbitMQ: {Utilities.ConvertObjectToJSONString(deleteMessage)}";
                Utilities.WriteLoggerFile(messageLog);
                _logger.LogInformation("Delete message sent to the queue for report ID: {Id}", id);

                MedicalReportsLocs.Remove(report);

                return Ok($"Medical report with ID {id} deleted.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting medical report with ID: {Id}", id);
                return StatusCode(500, $"An error occurred while deleting the medical report: {ex.Message}");
            }
        }






    }
}
