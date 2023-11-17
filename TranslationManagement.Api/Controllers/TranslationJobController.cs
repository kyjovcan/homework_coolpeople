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
using TranslationManagement.Api.Models;

namespace TranslationManagement.Api.Controllers
{
    [ApiController]
    [Route("api/jobs")]
    public class TranslationJobController : ControllerBase
    {

        private readonly AppDbContext _context;
        private readonly ILogger<TranslationJobController> _logger;

        public TranslationJobController(AppDbContext context, ILogger<TranslationJobController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet("GetJobs")]
        public IActionResult GetJobs()
        {
            var jobs = _context.TranslationJobs.ToArray();
            return Ok(jobs);
        }

        const double PricePerCharacter = 0.01;
        private void SetPrice(TranslationJob job)
        {
            if (job.OriginalContent != null)
                job.Price = job.OriginalContent.Length * PricePerCharacter;
        }

        [HttpPost("CreateJob")]
        public IActionResult CreateJob([FromBody] TranslationJob job)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            job.Status = JobStatuses.New;                           //job.Status = "New" - why not use existing value
            SetPrice(job);

            try
            {
                var jobs = _context.TranslationJobs.ToArray();
                _context.TranslationJobs.Add(job);
                _context.SaveChanges();                             // dont > 0, whether it works, just continue, if not, return 500
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }

            var notificationSvc = new UnreliableNotificationService();      // this will throw exception sometimes
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

            } while (!response);                                    // probably better exception handling

            _logger.LogInformation("New job notification sent");


            return CreatedAtAction(nameof(GetJobs), new { id = job.Id }, job);  // return 201 created 
        }


        [HttpPost("CreateJobWithFile")]
        public IActionResult CreateJobWithFile(IFormFile file, string customer)
        {
            string content;
            string fileExtension = Path.GetExtension(file.FileName)?.ToLower();

            switch (fileExtension)
            {
                case ".txt":
                    content = new StreamReader(file.OpenReadStream()).ReadToEnd();
                    break;

                case ".xml":
                    var xdoc = XDocument.Load(file.OpenReadStream());
                    content = xdoc.Root.Element("Content")?.Value;
                    customer = xdoc.Root.Element("Customer")?.Value?.Trim();
                    break;
                                                                        // add more possible file types in the future easily
                default:
                    return BadRequest("Unsupported file");              // Return action result code, uppercase sentence...
            }

            var newJob = new TranslationJob()                           // can now be also created with constructor
            {
                OriginalContent = content,
                TranslatedContent = "",
                CustomerName = customer,
            };

            SetPrice(newJob);

            return CreateJob(newJob);
        }

        [HttpPost("UpdateJobStatus")]
        public IActionResult UpdateJobStatus(int jobId, int translatorId, string newStatus = "")
        {
            _logger.LogInformation("Job status update request received: " + newStatus + " for job " + jobId.ToString() + " by translator " + translatorId);                                             // a lot of concating

            if (!JobStatuses.IsValidStatus(newStatus))                  // move validation to model
            {
                return BadRequest("Invalid status");
            }

            var job = _context.TranslationJobs.SingleOrDefault(j => j.Id == jobId);

            if (job == null)                                            // add also check if it is null 
            {
                return NotFound();
            }

            if (JobStatuses.IsInvalidStatusChange(job, newStatus))       // move validation to model
            {
                return BadRequest("Invalid status change");
            }

            job.Status = newStatus;
            job.TranslatorId = translatorId;

            try
            {
                _context.SaveChanges();
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }

            return Ok("Updated");
        }
    }
}