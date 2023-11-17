using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace TranslationManagement.Api.Services
{
    public class TranslationJobService : ControllerBase
    {

        private readonly AppDbContext _context;
        private readonly ILogger<TranslationJobService> _logger;

        public TranslationJobService(AppDbContext context, ILogger<TranslationJobService> logger)
        {
            _context = context;
            _logger = logger;
        }

        // MOVE DATA RELATED FUNCTIONALITY HERE
    }
}