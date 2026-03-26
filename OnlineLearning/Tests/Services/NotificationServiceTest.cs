using Microsoft.AspNetCore.Mvc;
using Moq;
using OnlineLearning.Areas.Admin.Controllers;
using OnlineLearning.Enums;
using OnlineLearning.Models.Domains.CourseModels;
using OnlineLearning.Repositories.Interfaces;
using OnlineLearning.Services.Implementations.Admin;
using OnlineLearning.Services.Interfaces.Admin;
using Xunit;

namespace OnlineLearning.Tests.Services
{
    public class NotificationServiceTest
    {
        private readonly Mock<INotificationRepository> _notificationRepoMock;
        private readonly NotificationService _service;

        public NotificationServiceTest()
        {
            _notificationRepoMock = new Mock<INotificationRepository>();
            _service = new NotificationService(_notificationRepoMock.Object);
        }

        [Fact]
        public async Task AddNotification_ShouldCallRepositoryAddAsync()
        {
            // Arrange
            var notification = new Notification
            {
                NotificationId = 1,
                CourseId = 101,
                CourseName = "Introduction to C#",
                CourseUrl = "/courses/introduction-to-csharp",
                CreatedAt = DateTime.Now.AddDays(-10),
                IsRead = false,
                IsCleared = false,
                NotificationType = NotificationType.Add
            };

            // Act
            await _service.AddNotification(notification);

            // Assert
            _notificationRepoMock.Verify(repo => repo.AddAsync(notification), Times.Once);
        }

        [Fact]
        public async Task ClearAll_ShouldCallRepositoryClearAll()
        {
            // Act
            await _service.ClearAll();

            // Assert
            _notificationRepoMock.Verify(repo => repo.ClearAll(), Times.Once);
        }

        [Fact]
        public async Task GetAllNotifications_ShouldReturnListOfNotifications()
        {
            // Arrange
            var mockNotifications = new List<Notification> { new Notification(), new Notification() };
            _notificationRepoMock.Setup(repo => repo.GetAllNotificationAysnc()).ReturnsAsync(mockNotifications);

            // Act
            var result = await _service.GetAllNotifications();

            // Assert
            Assert.Equal(mockNotifications.Count, result.Count);
        }

        [Fact]
        public async Task IsReaded_ShouldUpdateNotificationAsRead()
        {
            // Arrange
            var notification = new Notification { NotificationId = 1, CourseId = 123, IsRead = false };
            _notificationRepoMock.Setup(repo => repo.GetNotificationByCourseId(123, false)).ReturnsAsync(notification);

            // Act
            await _service.IsReaded(123, false);

            // Assert
            Assert.True(notification.IsRead);
            _notificationRepoMock.Verify(repo => repo.UpdateAsync(notification), Times.Once);
        }

        [Fact]
        public async Task MarkAllRead_ShouldCallMarkAllReadWithNotifications()
        {
            // Arrange
            var mockNotifications = new List<Notification> { new Notification(), new Notification() };
            _notificationRepoMock.Setup(repo => repo.GetNotificationIsNotRead()).ReturnsAsync(mockNotifications);

            // Act
            await _service.MarkAllRead();

            // Assert
            _notificationRepoMock.Verify(repo => repo.MarkAllRead(mockNotifications), Times.Once);
        }

        [Fact]
        public async Task MarkAllRead_ShouldNotCallMarkAllRead_WhenNoUnreadNotifications()
        {
            // Arrange
            _notificationRepoMock.Setup(repo => repo.GetNotificationIsNotRead()).ReturnsAsync(new List<Notification>());

            // Act
            await _service.MarkAllRead();

            // Assert
            _notificationRepoMock.Verify(repo => repo.MarkAllRead(It.IsAny<List<Notification>>()), Times.Never);
        }
    }
}
