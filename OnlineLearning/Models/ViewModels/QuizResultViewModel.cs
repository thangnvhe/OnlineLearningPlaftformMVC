using OnlineLearning.Models.Domains.CourseModels.QuizModels;
using System.Collections.Generic;

namespace OnlineLearning.Models.ViewModels
{
    public class QuizResultViewModel
    {
        public long QuizId { get; set; }
        public string QuizName { get; set; }
        public long CourseId { get; set; }
        public long ModuleId { get; set; }
        
        public int TotalQuestions { get; set; }
        public int CorrectAnswers { get; set; }
        public decimal Score { get; set; } // Điểm số (tỉ lệ phần trăm)
        
        public List<QuestionResultItem> QuestionResults { get; set; } = new List<QuestionResultItem>();
    }

    public class QuestionResultItem
    {
        public long QuestionId { get; set; }
        public string QuestionName { get; set; }
        public bool IsCorrect { get; set; }
        public List<OptionResultItem> SelectedOptions { get; set; } = new List<OptionResultItem>();
        public List<OptionResultItem> CorrectOptions { get; set; } = new List<OptionResultItem>();
    }
    
    public class OptionResultItem
    {
        public long OptionId { get; set; }
        public string OptionText { get; set; }
        public bool IsCorrect { get; set; }
    }
}
