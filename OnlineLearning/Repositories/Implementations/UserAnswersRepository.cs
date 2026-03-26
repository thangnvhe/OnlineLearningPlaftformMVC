using OnlineLearning.Data;
using OnlineLearning.Models.Domains.UserCourseRelationship;
using OnlineLearning.Repositories.Interfaces;

namespace OnlineLearning.Repositories.Implementations
{
    public class UserAnswersRepository : BaseRepository<UserAnswer>, IUserAnswersRepository
    {
        public UserAnswersRepository(OnlLearnDBContext context) : base(context)
        {
        }
    }
}
