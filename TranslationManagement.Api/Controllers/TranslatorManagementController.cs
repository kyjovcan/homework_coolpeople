using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TranslationManagement.Api.Models;

namespace TranslationManagement.Api.Controllers                         // namespace mismatch!!
{
    [ApiController]
    [Route("api/translators")]
    public class TranslatorManagementController : ControllerBase
    {
        private readonly AppDbContext _context;                         // i prefer shorter ones first
        private readonly ILogger<TranslatorManagementController> _logger;
        public static readonly string[] TranslatorStatuses = { "Applicant", "Certified", "Deleted" };


        public TranslatorManagementController(AppDbContext context, ILogger<TranslatorManagementController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet("GetTranslators")]
        public IActionResult GetTranslators()
        {
            var translators = _context.Translators.ToArray();
            return Ok(translators);
        }

        [HttpGet("GetTranslatorsByName")]
        public IActionResult GetTranslatorsByName(string name)
        {
            var translators = _context.Translators.Where(t => t.Name == name).ToArray();
            return Ok(translators);
        }


        [HttpPost("AddTranslator")]
        public IActionResult AddTranslator([FromBody] TranslatorModel translator)       // change parameter to be taken from body for validation
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Translators.Add(translator);

            try
            {
                _context.SaveChanges();                 // dont > 0, whether it works, just continue, if not, return 500
            }
            catch (Exception e)
            {
                return StatusCode(500, "Translator not added due to server error - " + e.Message);
            }
            return CreatedAtAction(nameof(GetTranslators), new { id = translator.Id }, translator);
        }

        [HttpPost("UpdateTranslatorStatus")]
        public IActionResult UpdateTranslatorStatus(int translatorId, string newStatus = "")
        {
            _logger.LogInformation("User status update request: " + newStatus + " for user id " + translatorId.ToString()); // would simplify, but should not touch logging

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