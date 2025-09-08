using MassTransit;
using SchoolMedical.Common.Shared.Models.LocBP;

namespace SchoolMedical.StudentProfileLocBp.Microservices.Consumers
{
    public class StudentProfileConsumer :IConsumer<SchoolMedical.BusinessObject.Shared.Models.LocBP.Models.MedicalReportsLocBp>
    {
        private readonly ILogger _logger;
       /* private List<SchoolMedical.BusinessObject.Shared.Models.LocBP.Models.MedicalReportsLocBp> MedicalReportsLocs { get; set; }*/
        public StudentProfileConsumer(ILogger<StudentProfileConsumer> logger)
        {
            _logger = logger;
      
        }

        public async Task Consume(ConsumeContext<SchoolMedical.BusinessObject.Shared.Models.LocBP.Models.MedicalReportsLocBp> context)
        {
            // Log the received message
            var data = context.Message;
            if (data != null)
            {
                string messageLog = string.Format("[{0}] CONSUME data from RabbitMQ.medicalQueue: {1}", DateTime.Now, Utilities.ConvertObjectToJSONString(data));
                Utilities.WriteLoggerFile(messageLog);
                _logger.LogInformation("Medical report sent to the queue: {MedicalReport}", data);



            }
        }
    }
}
