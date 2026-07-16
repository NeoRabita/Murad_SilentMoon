using Application.Abstractions.Messaging;
using SilentMoon.Application.Features.Topics;
using SilentMoon.Application.Interfaces.Authentication;
using SilentMoon.Domain.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SilentMoon.Application.Features.User.Commands.SetMyTopics
{
    public class SetMyTopicsCommandHandler : ICommandHandler<SetMyTopicsCommand, List<TopicResponse>>
    {
        private readonly IUow _uow;
        private readonly ICurrentUser _currentUser;

        public SetMyTopicsCommandHandler(IUow uow,ICurrentUser currentUser)
        {
            _uow = uow;
            _currentUser = currentUser;
        }

        public async Task<Result<List<TopicResponse>>> Handle(SetMyTopicsCommand command,CancellationToken ct)
        {
            var topicRepo = _uow.GetRepository<Topic>();
            var userTopicRepo = _uow.GetRepository<UserTopic>();

            var topics = await topicRepo.GetAllAsync(ct);

            var validTopicIds = topics
                .Select(x => x.Id)
                .Where(id => command.TopicIds.Contains(id))
                .Distinct()
                .ToList();

            var existingUserTopics = await userTopicRepo.GetAllAsync(ct);

            var myExistingUserTopics = existingUserTopics.Where(x => x.ApplicationUserId == _currentUser.UserId).ToList();

            userTopicRepo.DeleteRange(myExistingUserTopics);

            var newUserTopics = validTopicIds.Select(topicId => new UserTopic
            {
                ApplicationUserId = _currentUser.UserId,
                TopicId = topicId
            });

            await userTopicRepo.AddRangeAsync(newUserTopics, ct);

            return topics.Where(x => validTopicIds.Contains(x.Id)).Select(x => new TopicResponse
            {
                Id = x.Id,
                Name = x.Name
            }).ToList();
        }
    }
}
