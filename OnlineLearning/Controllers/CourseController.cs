using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnlineLearning.Models.Domains.UserModels;
using OnlineLearning.Services.Interfaces;
using static System.Net.WebRequestMethods;

namespace OnlineLearning.Controllers
{
	public class CourseController : Controller
	{
		private readonly ICourseService _courseService;
		private readonly ICourseEnrollmentService _courseEnrollmentService;
		private readonly ICategoryService _categoryService;

		public CourseController(ICourseService courseService, ICourseEnrollmentService courseEnrollmentService, ICategoryService categoryService)
		{
			_courseService = courseService;
			_courseEnrollmentService = courseEnrollmentService;
			_categoryService = categoryService;
		}



		public async Task<IActionResult> Index(string searchTerm = "", long? categoryId = null)
		{
			// Lấy danh sách tất cả các danh mục để hiển thị trong dropdown filter
			var categories = await _categoryService.GetAllCategoryAysnc();
			ViewBag.Categories = categories;

			// Lấy danh sách khóa học
			var listCourse = await _courseService.GetAllCourseByStatusAsync();

			// search
			if (!string.IsNullOrEmpty(searchTerm))
			{
				listCourse = listCourse.Where(c => c.CourseName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) || 
				                                  (c.Description != null && c.Description.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)))
									  .ToList();
			}

			// filter by category
			if (categoryId.HasValue)
			{
				listCourse = listCourse.Where(c => c.CourseCategories.Any(cc => cc.CategoryId == categoryId.Value)).ToList();
			}

			// Lưu lại các tham số tìm kiếm để hiển thị lại trên form
			ViewBag.SearchTerm = searchTerm;
			ViewBag.CategoryId = categoryId;

			return View(listCourse);
		}

		// COURSE DETAIL
		[HttpGet("Course/DetailsCourse/{courseId}")]
		public async Task<IActionResult> DetailsCourse(long? courseId)
		{
            long sessionCourseId;
            if (!HttpContext.Session.TryGetValue("CourseId", out var courseIdBytes) ||
                !long.TryParse(System.Text.Encoding.UTF8.GetString(courseIdBytes), out sessionCourseId))
            {
                // Nếu không có CourseId trong Session, sử dụng courseId từ URL và lưu vào Session
                sessionCourseId = courseId.Value;
            }

            long validCourseId = sessionCourseId;

            string actionNeeded = "";
			if (!HttpContext.Session.TryGetValue("UserId", out var userIdBytes) ||
				!long.TryParse(System.Text.Encoding.UTF8.GetString(userIdBytes), out var userId))
			{
				actionNeeded = "login";
			}
			else
			{
				if (!await _courseService.CheckPurchaseCourseAsync(userId, validCourseId))
				{
					actionNeeded = "purchase";
				}
				else if (!await _courseEnrollmentService.CheckCourseEnrollment(userId, validCourseId))
				{
					actionNeeded = "enroll";
				}
				else
				{
					actionNeeded = "resume";
				}
			}

            HttpContext.Session.SetString("CourseId", courseId.ToString());

            var course = await _courseService.GetCourseDetailAsync(validCourseId);
			if (course == null)
			{
				ViewBag.IsCourseExisted = false;
			}

			ViewBag.actionNeeded = actionNeeded;
			return View(course);
		}

	}
}
