using Moq;
using OnlineLearning.Models.Domains.UserCourseRelationship;
using OnlineLearning.Models.DTOs;
using OnlineLearning.Repositories.Interfaces;
using OnlineLearning.Services.Implementations;
using Xunit;

namespace OnlineLearning.Tests.Services
{
    public class MyCourseServiceTest
    {
        private readonly Mock<IMyCourseRepository> _myCourseRepoMock;
        private readonly Mock<IWishlistRepository> _wishListRepoMock;
        private readonly MyCourseService _service;

        public MyCourseServiceTest()
        {
            _myCourseRepoMock = new Mock<IMyCourseRepository>();
            _wishListRepoMock = new Mock<IWishlistRepository>();
            _service = new MyCourseService(_myCourseRepoMock.Object, _wishListRepoMock.Object);
        }

        [Fact]
        public async Task AddToWishlistAsync_ShouldReturnSuccess_WhenCourseNotInWishlist()
        {
            // Arrange
            _wishListRepoMock.Setup(x => x.ExistingWishListAsync(1, 101)).ReturnsAsync((WishList?)null);

            // Act
            var result = await _service.AddToWishlistAsync(1, 101);

            // Assert
            Assert.True(result.Success);
            Assert.Equal("Course successfully added to wishlist.", result.Message);
            _wishListRepoMock.Verify(x => x.AddAsync(It.IsAny<WishList>()), Times.Once);
        }

        [Fact]
        public async Task AddToWishlistAsync_ShouldReturnFailure_WhenCourseAlreadyInWishlist()
        {
            // Arrange
            _wishListRepoMock.Setup(x => x.ExistingWishListAsync(1, 101))
                             .ReturnsAsync(new WishList());

            // Act
            var result = await _service.AddToWishlistAsync(1, 101);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("This course is already in your wishlist.", result.Message);
            _wishListRepoMock.Verify(x => x.AddAsync(It.IsAny<WishList>()), Times.Never);
        }

        [Fact]
        public async Task GetMyCourseList_ShouldReturnCourses_WhenDataExists()
        {
            // Arrange
            var mockData = new List<MyCourseDto> { new MyCourseDto { CourseId = 101, CourseName = "Sample Course" } };
            _myCourseRepoMock.Setup(x => x.GetCourseAndProgressById(1)).ReturnsAsync(mockData);

            // Act
            var result = await _service.GetMyCourseList(1);

            // Assert
            Assert.NotEmpty(result);
            Assert.Equal(101, result[0].CourseId);
        }

        [Fact]
        public async Task GetMyWishlist_ShouldReturnWishlist_WhenDataExists()
        {
            // Arrange
            var mockWishlist = new List<WishlistDTO> { new WishlistDTO { CourseId = 202, CourseName = "Wishlist Course" } };
            _myCourseRepoMock.Setup(x => x.GetMyWishListByUserId(1)).ReturnsAsync(mockWishlist);

            // Act
            var result = await _service.GetMyWishlist(1);

            // Assert
            Assert.NotEmpty(result);
            Assert.Equal(202, result[0].CourseId);
        }

        [Fact]
        public async Task RemoveWishlistAsync_ShouldCallRemoveMethodOnce()
        {
            // Act
            await _service.RemoveWishlistAsync(1, 303);

            // Assert
            _myCourseRepoMock.Verify(x => x.RmoveWishList(1, 303), Times.Once);
        }
    }
}
