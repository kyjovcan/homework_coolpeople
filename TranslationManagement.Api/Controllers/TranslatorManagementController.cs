using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace TranslationManagement.Api.Controllers          // namespace mismatch!!
{
    [ApiController]
    [Route("api/TranslatorsManagement/[action]")]
    public class TranslatorManagementController : ControllerBase
    {
        public class TranslatorModel        // move to models
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string HourlyRate { get; set; }
            public string Status { get; set; }
            public string CreditCardNumber { get; set; }
        }

        public static readonly string[] TranslatorStatuses = { "Applicant", "Certified", "Deleted" };

        private readonly AppDbContext _context; // i prefer shorter ones first
        private readonly ILogger<TranslatorManagementController> _logger;


        public TranslatorManagementController(AppDbContext context, ILogger<TranslatorManagementController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult GetTranslators()
        {
            var translators = _context.Translators.ToArray();
            return Ok(translators);
        }

        [HttpGet]
        public IActionResult GetTranslatorsByName(string name)
        {
            var translators = _context.Translators.Where(t => t.Name == name).ToArray();
            return Ok(translators);
        }


        [HttpPost]
        public IActionResult AddTranslator([FromBody] TranslatorModel translator)       // change parameter to be taken from body
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Translators.Add(translator);

            if (_context.SaveChanges() > 0)         // save here
                return CreatedAtAction(nameof(GetTranslators), new { id = translator.Id }, translator);
            else
                return StatusCode(500, "Translator not added due to server error");
        }

        [HttpPost]
        public IActionResult UpdateTranslatorStatus(int translatorId, string newStatus = "")
        {
            _logger.LogInformation("User status update request: " + newStatus + " for user id " + translatorId.ToString()); // would simplify, but should not touch logging

            //if (TranslatorStatuses.Where(status => status == newStatus).Count() == 0)
            //{
            //    throw new ArgumentException("unknown status");
            //}

            if (TranslatorStatuses.All(status => status != newStatus))
            {
                return BadRequest("Unknown status");
            }

            //var job = _context.Translators.Single(j => j.Id == Translator);        ????? why job in translators
            //job.Status = newStatus;
            //_context.SaveChanges();

            var translator = _context.Translators.SingleOrDefault(t => t.Id == translatorId);

            if (translator == null)
            {
                return NotFound();
            }

            translator.Status = newStatus;
            _context.SaveChanges();

            return Ok("Updated");
        }
    }
}