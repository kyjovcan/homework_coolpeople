using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using TranslationManagement.Api;
using TranslationManagement.Api.Controllers;
using TranslationManagement.Api.Models;
using Xunit;


namespace YourNamespace.Controllers
{
    public class TranslationJobControllerTests
    {
        private AppDbContext CreateDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "TranslationAppDatabase")
                .Options;

            return new AppDbContext(options);
        }

        [Fact]
        public void GetJobs_ReturnsAllJobs()
        {
            // Arrange
            var context = CreateDbContext();
            var loggerMock = new Mock<ILogger<TranslationJobController>>();
            var controller = new TranslationJobController(context, loggerMock.Object);

            // Act
            var result = controller.GetJobs();

            // Assert
            Assert.IsType<OkObjectResult>(result);
            var okResult = (OkObjectResult)result;
            var jobs = Assert.IsAssignableFrom<IEnumerable<TranslationJob>>(okResult.Value);
            Assert.NotNull(jobs);
        }

        [Fact]
        public void CreateJob_ReturnsCreatedResult()
        {
            // Arrange
            var context = CreateDbContext();
            var loggerMock = new Mock<ILogger<TranslationJobController>>();
            var controller = new TranslationJobController(context, loggerMock.Object);

            var newJob = new TranslationJob
            {
                CustomerName = "John Doe",
                Status = "New",
                OriginalContent = "Sample original content.",
                TranslatedContent = "",
                Price = 10.5,
                TranslatorId = 0,
            };

            // Act
            var result = controller.CreateJob(newJob);

            // Assert
            Assert.IsType<CreatedAtActionResult>(result);
            var createdResult = (CreatedAtActionResult)result;
            Assert.Equal(201, createdResult.StatusCode);
            Assert.NotNull(createdResult.Value);
            Assert.IsType<TranslationJob>(createdResult.Value);
        }

        [Fact]
        public void CreateJob_ReturnsBadRequestWhenModelStateIsInvalid()
        {
            // Arrange
            var context = CreateDbContext();
            var loggerMock = new Mock<ILogger<TranslationJobController>>();
            var controller = new TranslationJobController(context, loggerMock.Object);
            controller.ModelState.AddModelError("customerName", "Customer name is required.");

            // Act
            var result = controller.CreateJob(new TranslationJob());

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public void CreateJobWithFile_ReturnsBadRequestForUnsupportedFileType()
        {
            // Arrange
            var context = CreateDbContext();
            var loggerMock = new Mock<ILogger<TranslationJobController>>();
            var controller = new TranslationJobController(context, loggerMock.Object);


            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.FileName).Returns("unsupportedFileType.docx");

            // Act
            var result = controller.CreateJobWithFile(fileMock.Object, "customer");

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public void CreateJob_ReturnsInternalServerErrorWhenSaveChangesFails()
        {
            // Arrange
            var context = CreateDbContext();
            var loggerMock = new Mock<ILogger<TranslationJobController>>();
            var controller = new TranslationJobController(context, loggerMock.Object);

            // Act
            controller.CreateJob(new TranslationJob(0, "Jozo", "New", "asdf", "", 0, 0));
            var result = controller.CreateJob(new TranslationJob(1, "", "", "", "", 0, 0));

            // Assert
            Assert.IsType<ObjectResult>(result);
            var objectResult = (ObjectResult)result;
            Assert.Equal(500, objectResult.StatusCode);
        }

        [Fact]
        public void UpdateJobStatus_ReturnsOkForValidUpdate()
        {
            // Arrange
            var context = CreateDbContext();
            var loggerMock = new Mock<ILogger<TranslationJobController>>();
            var controller = new TranslationJobController(context, loggerMock.Object);

            var initJob = controller.CreateJob(new TranslationJob(0, "Jozo", "New", "asdf", "", 0, 0));
            var createdJob = (CreatedAtActionResult)initJob;
            TranslationJob createdJobValue = (TranslationJob)createdJob.Value;
            // Act
            var result = controller.UpdateJobStatus(createdJobValue.Id, 1, "In Progress");

            // Assert
            Assert.IsType<OkObjectResult>(result);
            var okResult = (OkObjectResult)result;
            Assert.Equal(200, okResult.StatusCode);
            Assert.Equal("Updated", okResult.Value);
        }

        [Fact]
        public void UpdateJobStatus_ReturnsBadRequestForInvalidStatus()
        {
            // Arrange
            var context = CreateDbContext();
            var loggerMock = new Mock<ILogger<TranslationJobController>>();
            var controller = new TranslationJobController(context, loggerMock.Object);

            // Act
            var result = controller.UpdateJobStatus(1, 1, "InvalidStatus");

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
            var badRequestResult = (BadRequestObjectResult)result;
            Assert.Equal(400, badRequestResult.StatusCode);
        }


        [Fact]
        public void UpdateJobStatus_ReturnsNotFoundForNonexistentJob()
        {
            // Arrange
            var context = CreateDbContext();
            var loggerMock = new Mock<ILogger<TranslationJobController>>();
            var controller = new TranslationJobController(context, loggerMock.Object);

            // Act
            var result = controller.UpdateJobStatus(99999999, 1, "New");

            // Assert
            Assert.IsType<NotFoundResult>(result);
            var notFoundResult = (NotFoundResult)result;
            Assert.Equal(404, notFoundResult.StatusCode);
        }

        [Fact]
        public void UpdateJobStatus_ReturnsBadRequestForInvalidStatusChange()
        {
            // Arrange
            var context = CreateDbContext();
            var loggerMock = new Mock<ILogger<TranslationJobController>>();
            var controller = new TranslationJobController(context, loggerMock.Object);

            var initJob = controller.CreateJob(new TranslationJob(0, "Jozo", "New", "asdf", "", 0, 0));
            var createdJob = (CreatedAtActionResult)initJob;
            TranslationJob createdJobValue = (TranslationJob)createdJob.Value;

            // Act
            var result = controller.UpdateJobStatus(createdJobValue.Id, 1, "Completed");

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
            var badRequestResult = (BadRequestObjectResult)result;
            Assert.Equal(400, badRequestResult.StatusCode);
        }
    }
}
