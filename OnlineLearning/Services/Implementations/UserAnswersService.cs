using OnlineLearning.Models.Domains.CourseModels.QuizModels;
using OnlineLearning.Models.Domains.UserCourseRelationship;
using OnlineLearning.Models.DTOs;
using OnlineLearning.Repositories.Implementations;
using OnlineLearning.Repositories.Interfaces;
using OnlineLearning.Services.Interfaces;

namespace OnlineLearning.Services.Implementations
{
    public class UserAnswersService : IUserAnswersService
    {
        private readonly IUserAnswersRepository _userAnswersRepository;

        public UserAnswersService(IUserAnswersRepository userAnswersRepository)
        {
            _userAnswersRepository = userAnswersRepository;
        }

        public async Task<UserAnswer> CreateUserAnswerAsync(UserAnswersDTO userAnswersDTO)
        {
            var userAnswer = new UserAnswer
            {
                UserId = userAnswersDTO.UserId,
                QuestionId = userAnswersDTO.QuestionId,
                OptionId = userAnswersDTO.OptionId,
                IsCorrect = userAnswersDTO.IsCorrect,
                CreatedAt = DateTime.Now

            };
            return await _userAnswersRepository.AddAsync(userAnswer);
        }
    }
}
