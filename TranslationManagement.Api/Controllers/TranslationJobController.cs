using System;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using External.ThirdParty.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TranslationManagement.Api.Controlers;

namespace TranslationManagement.Api.Controllers
{
    [ApiController]
    [Route("api/jobs/[action]")]
    public class TranslationJobController : ControllerBase
    {
        public class TranslationJob
        {
            public int Id { get; set; }
            public string CustomerName { get; set; }
            public string Status { get; set; }
            public string OriginalContent { get; set; }
            public string TranslatedContent { get; set; }
            public double Price { get; set; }
        }

        static class JobStatuses
        {
            internal static readonly string New = "New";
            internal static readonly string Inprogress = "InProgress";
            internal static readonly string Completed = "Completed";
        }

        private AppDbContext _context;                                          // everywhere we create new one, change tomorrow to dependency injection from one place
        private readonly ILogger<TranslatorManagementController> _logger;

        public TranslationJobController(IServiceScopeFactory scopeFactory, ILogger<TranslatorManagementController> logger)
        {
            _context = scopeFactory.CreateScope().ServiceProvider.GetService<AppDbContext>();
            _logger = logger;
        }

        [HttpGet]
        public IActionResult GetJobs()
        {
            var jobs = _context.TranslationJobs.ToArray();
            return Ok(jobs);
        }

        const double PricePerCharacter = 0.01;
        private void SetPrice(TranslationJob job)
        {
            job.Price = job.OriginalContent.Length * PricePerCharacter;
        }

        [HttpPost]
        public IActionResult CreateJob(TranslationJob job)
        {
            //job.Status = "New";         why not use existing value
            job.Status = JobStatuses.New;

            SetPrice(job);
            _context.TranslationJobs.Add(job);

            bool success = _context.SaveChanges() > 0;
            if (success)
            {
                var notificationSvc = new UnreliableNotificationService();      // this will throw exception sometimes, handle it properly

                bool response = false;
                do
                {
                    try
                    {
                        response = notificationSvc.SendNotification("Job created: " + job.Id).Result;
                    }
                    catch (Exception ex)
                    {
                        continue;
                    }
                    
                } while (!response);            // probably better exception handling

                _logger.LogInformation("New job notification sent");
            }

            return CreatedAtAction(nameof(GetJobs), new { id = job.Id }, job);
        }


        [HttpPost]
        public IActionResult CreateJobWithFile(IFormFile file, string customer)
        {
            //var reader = new StreamReader(file.OpenReadStream());       // better create newstreamreader for deadlocks
            string content;

            if (file.FileName.EndsWith(".txt"))
            {
                content = new StreamReader(file.OpenReadStream()).ReadToEnd();
            }
            else if (file.FileName.EndsWith(".xml"))
            {
                //var xdoc = XDocument.Parse(reader.ReadToEnd());
                var xdoc = XDocument.Load(file.OpenReadStream());

                content = xdoc.Root.Element("Content").Value;
                customer = xdoc.Root.Element("Customer").Value.Trim();
            }
            else
            {
                return BadRequest("Unsupported file");  // return action result code, uppercase sentence...
            }

            var newJob = new TranslationJob()
            {
                OriginalContent = content,
                TranslatedContent = "",
                CustomerName = customer,
            };

            SetPrice(newJob);

            return CreateJob(newJob);
        }

        [HttpPost]
        public IActionResult UpdateJobStatus(int jobId, int translatorId, string newStatus = "")
        {
            _logger.LogInformation("Job status update request received: " + newStatus + " for job " + jobId.ToString() + " by translator " + translatorId);         // a lot of concating
                
            if (typeof(JobStatuses).GetProperties().Count(prop => prop.Name == newStatus) == 0)
            {
                return BadRequest("Invalid status");
            }

            var job = _context.TranslationJobs.SingleOrDefault(j => j.Id == jobId);

            if (job == null)            // add also check if it is null 
            {
                return NotFound();
            }

            bool isInvalidStatusChange = (job.Status == JobStatuses.New && newStatus == JobStatuses.Completed) ||
                                         job.Status == JobStatuses.Completed || newStatus == JobStatuses.New;
            if (isInvalidStatusChange)
            {
                return BadRequest("Invalid status change");
            }

            job.Status = newStatus;
            _context.SaveChanges();

            return Ok("Updated");
        }
    }
}