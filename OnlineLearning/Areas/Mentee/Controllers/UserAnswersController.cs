using Microsoft.AspNetCore.Mvc;
using OnlineLearning.Controllers;
using OnlineLearning.Models.DTOs;
using OnlineLearning.Models.ViewModels;
using OnlineLearning.Services.Interfaces;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using Microsoft.AspNetCore.Authorization;
using OnlineLearning.Enums;

namespace OnlineLearning.Areas.Mentee.Controllers
{
    [Area("Mentee")]
    [Authorize(Roles = nameof(RoleType.MENTEE))]
    public class UserAnswersController : Controller
    {
        private readonly IQuizService _quizService;
        private readonly IQuestionService _questionService;
        private readonly IUserAnswersService _userAnswerService;

        public UserAnswersController(IQuizService quizService, IQuestionService questionService, IUserAnswersService userAnswerService)
        {
            _quizService = quizService;
            _questionService = questionService;
            _userAnswerService = userAnswerService;
        }
        public async Task<IActionResult> DoQuiz(long courseId, long moduleId, long quizId)
        {
            // Khởi tạo model cho view
            var model = new QuestionViewModel
            {
                CourseId = courseId
            };

            // Lấy thông tin người dùng hiện tại
            var userId = HttpContext.Session.GetString("UserId");

            // Kiểm tra nếu người dùng đã làm bài quiz này chưa
            //var hasAttempted = await _userAnswerService.HasAttemptedQuizAsync(userId, quizId);
            //if (hasAttempted)
            //{
            //    // Nếu đã làm rồi, chuyển đến trang kết quả
            //    return RedirectToAction("Result", new { courseId, moduleId, quizId });
            //}

            QuizDTO quizDTO = null;
            // Lấy thông tin quiz
            quizDTO = await _quizService.GetQuizByIdAsync(quizId);
            if (quizDTO == null)
            {
                return NotFound();
            }

            var questions = await _questionService.GetAllQuestionsWithOptionsByQuizIdAsync(quizId);
            model.Questions = questions;
            model.Quiz = quizDTO;
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> SubmitQuiz(QuizSubmissionDTO submission)
        {
            try
            {
                if (submission == null || submission.UserAnswers == null || submission.UserAnswers.Count == 0)
                {
                    TempData["Error"] = "Không có câu trả lời nào được gửi lên";
                    return RedirectToAction("DoQuiz", new { courseId = submission.CourseId, moduleId = submission.ModuleId, quizId = submission.QuizId });
                }

                // Lấy ID người dùng từ session
                var userId = HttpContext.Session.GetString("UserId");
                if (string.IsNullOrEmpty(userId))
                {
                    return RedirectToAction("Login", "Account", new { area = "" });
                }

                // Lấy danh sách câu hỏi và đáp án đúng
                var questions = await _questionService.GetAllQuestionsWithOptionsByQuizIdAsync(submission.QuizId);
                
                // Nhóm các câu trả lời theo questionId
                var answersGroupedByQuestionId = submission.UserAnswers
                    .GroupBy(a => a.QuestionId)
                    .ToDictionary(g => g.Key, g => g.Select(a => a.SelectedOptionId).ToList());
                
                // Danh sách lưu kết quả cuối cùng
                var userAnswers = new List<UserAnswersDTO>();
                
                foreach (var questionId in answersGroupedByQuestionId.Keys)
                {
                    // Tìm câu hỏi tương ứng
                    var question = questions.FirstOrDefault(q => q.QuestionId == questionId);
                    if (question == null) continue;
                    
                    // Lấy danh sách optionId mà người dùng đã chọn
                    var selectedOptionIds = answersGroupedByQuestionId[questionId];
                    
                    // Lấy danh sách các đáp án đúng của câu hỏi
                    var correctOptionIds = question.Options
                        .Where(o => o.IsCorrect)
                        .Select(o => o.OptionId)
                        .ToList();
                    
                    // Kiểm tra xem người dùng đã chọn đúng và đủ tất cả các đáp án đúng chưa
                    // Điều kiện: 
                    // 1. Số lượng đáp án người dùng chọn = số lượng đáp án đúng
                    // 2. Tất cả đáp án người dùng chọn đều là đáp án đúng
                    bool isQuestionCorrect = correctOptionIds.Count == selectedOptionIds.Count && 
                                            selectedOptionIds.All(id => correctOptionIds.Contains(id));
                    
                    // Lưu kết quả cho từng đáp án người dùng đã chọn
                    foreach (var optionId in selectedOptionIds)
                    {
                        var option = question.Options.FirstOrDefault(o => o.OptionId == optionId);
                        if (option == null) continue;
                        
                        userAnswers.Add(new UserAnswersDTO
                        {
                            UserId = long.Parse(userId),
                            QuestionId = questionId,
                            OptionId = optionId,
                            // Đánh dấu là đúng nếu toàn bộ câu trả lời đúng, ngược lại là sai
                            IsCorrect = isQuestionCorrect
                        });
                    }
                }

                // Lưu tất cả câu trả lời vào database
                foreach (var userAnswer in userAnswers)
                {
                    await _userAnswerService.CreateUserAnswerAsync(userAnswer);
                }
                
                // Đếm số câu trả lời đúng
                var correctQuestionIds = userAnswers
                    .Where(a => a.IsCorrect)
                    .Select(a => a.QuestionId)
                    .Distinct()
                    .ToList();
                
                var totalCorrectAnswers = correctQuestionIds.Count;
                
                // Lưu kết quả vào TempData để hiển thị trên trang kết quả
                TempData["TotalQuestions"] = questions.Count;
                TempData["CorrectAnswers"] = totalCorrectAnswers;
                TempData["Score"] = questions.Count > 0 
                    ? (Math.Round((decimal)totalCorrectAnswers / questions.Count * 10, 2)).ToString() 
                    : "0";
                
                // Chuyển hướng đến trang kết quả
                return RedirectToAction("Result", new { 
                    quizId = submission.QuizId 
                });
            }
            catch (Exception ex)
            {
                // Ghi log lỗi ở đây nếu cần
                TempData["Error"] = "Đã xảy ra lỗi khi lưu câu trả lời: " + ex.Message;
                return RedirectToAction("DoQuiz", new { 
                    courseId = submission.CourseId, 
                    moduleId = submission.ModuleId, 
                    quizId = submission.QuizId 
                });
            }
        }

        public async Task<IActionResult> Result(long quizId)
        {
            try
            {
                var quiz = await _quizService.GetQuizByIdAsync(quizId);
                if (quiz == null)
                {
                    return NotFound();
                }

                var totalQuestions = TempData["TotalQuestions"] as int? ?? 0;
                var correctAnswers = TempData["CorrectAnswers"] as int? ?? 0;
                var scoreString = TempData["Score"] as string ?? "0";
                decimal score = decimal.Parse(scoreString);

                var model = new QuizResultViewModel
                {
                    QuizId = quizId,
                    QuizName = quiz.QuizName,
                    CourseId = quiz.CourseId,
                    ModuleId = quiz.ModuleId,
                    TotalQuestions = totalQuestions,
                    CorrectAnswers = correctAnswers,
                    Score = score
                };

                return View(model);
            }
            catch (Exception ex)
            {
                return View("Error"/*, new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier, ErrorMessage = ex.Message }*/);
            }
        }
    }
}
