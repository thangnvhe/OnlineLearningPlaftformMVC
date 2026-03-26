

namespace OnlineLearning.Models.DTOs
{
    public class UserAnswersDTO
    {
        public long UserId { get; set; }
        public long QuestionId { get; set; }
        public long OptionId { get; set; }
        public bool IsCorrect { get; set; }
    }
}
