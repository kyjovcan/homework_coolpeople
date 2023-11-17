using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TranslationManagement.Api.Models;

namespace TranslationManagement.Api.Services                        
{
    public class TranslatorManagementService 
    {
        private readonly AppDbContext _context;                         
        private readonly ILogger<TranslatorManagementService> _logger;

        public TranslatorManagementService(AppDbContext context, ILogger<TranslatorManagementService> logger)
        {
            _context = context;
            _logger = logger;
        }

        // MOVE DATA RELATED FUNCTIONALITY HERE
    }
}