using TelegramBotApi.Models;
using TelegramBotApi.DataContext;
using Microsoft.EntityFrameworkCore;
using TelegramBotApi.Controllers;
using Microsoft.AspNetCore.Mvc;


namespace UnitTestTelegramBotApi
{
    public class QuestionControllerTest
    {
        private readonly VoenkomContext _context;
        private readonly QuestionController _controller;

        public QuestionControllerTest()
        {
            var options = new DbContextOptionsBuilder<VoenkomContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new VoenkomContext(options);
            _controller = new QuestionController(_context);

            SeedTestData();
        }

        private void SeedTestData()
        {
            // Добавляем тестовые категории
            _context.Categories.Add(new Category { CategoryId = 1, CategoryName = "Test Category 1" });
            _context.Categories.Add(new Category { CategoryId = 2, CategoryName = "Test Category 2" });

            _context.SaveChanges();
        }

        /// <summary>
        /// Метод, тестирующий возврат всех вопросов (Questions). Метод Get контроллера Questions
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task Get_ReturnsAllQuestions()
        {

            // тестовые данные
            var expectedQuestions = new List<Question>
            {
                new Question
                {
                    QuestionId = 1,
                    CategoryId = 1,
                    QuestionText = "test q text 1",
                    FileName = "file name test 1",
                    FileData = "file data test 1",
                    PhotoName = "photo name test 2",
                    PhotoData = new byte[] { 0x01 }
                },
                new Question
                {
                    QuestionId = 2,
                    CategoryId = 2,
                    QuestionText = "test q text 2",
                    FileName = "file name test 2",
                    FileData = "file data test 2",
                    PhotoName = "photo name test 2",
                    PhotoData = new byte[] { 0x02 }
                }
            };

            await _context.Questions.AddRangeAsync(expectedQuestions);
            await _context.SaveChangesAsync();

            var result = await _controller.Get();

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedQuestions = Assert.IsType<List<Question>>(okResult.Value);

            Assert.Equal(expectedQuestions.Count, returnedQuestions.Count);
        }

        /// <summary>
        /// Метод, тестирующий возврат вопроса по его ИД. Метод GetById контроллера Questions
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task GetById_ReturnsQuestion()
        {
            int testId = 1;

            var expectedQuestion = new Question
            {
                QuestionId = 1,
                CategoryId = 1,
                FileName = "f name",
                QuestionText = "q text",
                FileData = "f data",
                PhotoName = "ph photo",
                PhotoData = new byte[] { 0x01, 0x02 }
            };

            await _context.Questions.AddAsync(expectedQuestion);
            await _context.SaveChangesAsync();

            var result = await _controller.GetById(testId);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var question = Assert.IsType<Question>(okResult.Value);

            Assert.Equal(testId, question.QuestionId);
        }

        /// <summary>
        /// Проверяет, что метод GetById контроллера возвращает результат NotFound 
        /// при запросе вопроса с несуществующим идентификатором.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task GetById_WithNonExistingId_ReturnsNotFound()
        {
            int nonExistingId = 999;

            var result = await _controller.GetById(nonExistingId);

            Assert.IsType<NotFoundResult>(result);
        }

        /// <summary>
        /// Метод, тестирующий добавление вопроса c корректными данным и возвратом созданного вопроса. Метод Add контроллера Questions
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task Add_WithValidData_ReturnsCreatedQuestion()
        {
            var dto = new QuestionDto
            {
                CategoryId = 1,
                QuestionText = "new q text",
                FileName = "new f name",
                FileData = "new f data",
                PhotoName = "new p name",
                PhotoData = new byte[] { 0x01 }
            };

            var result = await _controller.Add(dto);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var createdQuestion = Assert.IsType<Question>(okResult.Value);

            Assert.NotEqual(0, createdQuestion.QuestionId);
            Assert.Equal(1, _context.Questions.Count());
        }

        /// <summary>
        /// Метод, тестирующий добавление вопроса с некорретным ИД категории и возвратом ошибки BadRequest. Метод Add контроллера Question
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task Add_WithInvalidCategoryId_ReturnsBadRequest()
        {
            var dto = new QuestionDto
            {
                CategoryId = 999,
                QuestionText = "new q text",
                FileName = "new f name",
                FileData = "new f data",
                PhotoName = "new p name",
                PhotoData = new byte[] { 0x01 }
            };

            var result = await _controller.Add(dto);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);

            Assert.Equal("Такого CategoryId не существует.", badRequest.Value);
        }
    }
}