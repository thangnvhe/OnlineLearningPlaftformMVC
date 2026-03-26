using System.Collections.Generic;

namespace OnlineLearning.Models.DTOs
{
    public class QuizSubmissionDTO
    {
        public long QuizId { get; set; }
        public long CourseId { get; set; }
        public long ModuleId { get; set; }
        public List<UserAnswerSubmission> UserAnswers { get; set; } = new List<UserAnswerSubmission>();
    }

    public class UserAnswerSubmission
    {
        public long QuestionId { get; set; }
        public long SelectedOptionId { get; set; }
    }
}