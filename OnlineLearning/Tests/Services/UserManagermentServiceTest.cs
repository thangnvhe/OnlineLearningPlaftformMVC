using Moq;
using OnlineLearning.Models.Domains.Miscellaneous;
using OnlineLearning.Models.Domains.UserModels;
using OnlineLearning.Models.DTOs;
using OnlineLearning.Repositories.Interfaces;
using OnlineLearning.Services.Implementations;
using OnlineLearning.Services.Implementations.Admin;
using OnlineLearning.Services.Interfaces;
using OnlineLearning.Utils;
using Xunit;

namespace OnlineLearning.Tests.Services
{
    public class UserManagermentServiceTest
    {
        private readonly Mock<IUserRepository> _userRepoMock;
        private readonly Mock<IUserService> _userServiceMock;
        private readonly Mock<IFileUploadService> _fileUploadServiceMock;
        private readonly Mock<IUserRoleRepository> _userRoleRepoMock;
        private readonly Mock<IRoleRepository> _roleRepoMock;

        private readonly UserManagementServiceImpl _userService;

        public UserManagermentServiceTest()
        {
            _userRepoMock = new Mock<IUserRepository>();
            _userServiceMock = new Mock<IUserService>();
            _fileUploadServiceMock = new Mock<IFileUploadService>();
            _userRoleRepoMock = new Mock<IUserRoleRepository>();
            _roleRepoMock = new Mock<IRoleRepository>();
            _userService = new UserManagementServiceImpl(_userServiceMock.Object,
                _userRepoMock.Object,
                _fileUploadServiceMock.Object,
                _userRoleRepoMock.Object,
                _roleRepoMock.Object); 
        }

        [Fact]
        public async Task AddAsync_ShouldCallRepositoryAddAsync()
        {

            var user = new Admin_AddUserDto
            {
                Email = "john.doe@example.com",
                Password = PasswordUtils.HashPassword("1234567"),
                Dob = new DateOnly(1990, 1, 1),
                Fullname = "John Doe",
                Phone = "0987654321",
                Avatar_str = "/images/default.jpg",
                Gender = true,
            };

            _userServiceMock.Setup(s => s.GetUserByEmailAsync(user.Email)).ReturnsAsync((User)null);
            _userRepoMock.Setup(r => r.GetByPhoneAsync(user.Phone)).ReturnsAsync((User)null);
            await _userService.AddUser(user);


            _userRepoMock.Verify(repo => repo.AddAsync(It.IsAny<User>()), Times.Once);
            _userRoleRepoMock.Verify(r => r.AddAsync(It.IsAny<UserRole>()), Times.Once);
        }


        [Fact]
        public async Task DeleteUser_ShouldMarkUserAsDeleted()
        {
            var userId = 1;
            var user = new User { UserId = userId, IsDeleted = false, IsActived = true };

            _userRepoMock.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(user);

            await _userService.DeleteUser(userId);

            Assert.True(user.IsDeleted);
            Assert.False(user.IsActived);
            _userRepoMock.Verify(r => r.UpdateAsync(user), Times.Once);
        }

        [Fact]
        public async Task UpdateUser_ShouldUpdateUserSuccessfully()
        {
            var userId = 1;
            var userDto = new Admin_AddUserDto
            {
                Id = userId,
                Email = "updated@example.com",
                Fullname = "Updated User",
                Phone = "987654321",
                RoleId = 2
            };
            var existingUser = new User { UserId = userId, Email = "old@example.com" };

            _userRepoMock.Setup(r => r.GetUsersWithRolesAsync(userId)).ReturnsAsync(existingUser);

            await _userService.UpdateUser(userDto);

            Assert.Equal(userDto.Email, existingUser.Email);
            Assert.Equal(userDto.Fullname, existingUser.FullName);
            Assert.Equal(userDto.Phone, existingUser.Phone);

            _userRepoMock.Verify(r => r.UpdateAsync(existingUser), Times.Once);
        }
    }
}
