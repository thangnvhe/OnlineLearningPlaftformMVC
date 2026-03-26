using OnlineLearning.Models.Domains.CourseModels.QuizModels;
using OnlineLearning.Models.Domains.UserCourseRelationship;
using OnlineLearning.Models.DTOs;
using System.Collections.Generic;

namespace OnlineLearning.Services.Interfaces
{
    public interface IUserAnswersService
    {
        Task<UserAnswer> CreateUserAnswerAsync(UserAnswersDTO userAnswersDTO);
    }
}
