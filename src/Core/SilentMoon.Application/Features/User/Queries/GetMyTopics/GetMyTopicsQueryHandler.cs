using Application.Abstractions.Messaging;
using SilentMoon.Application.Common.Extensions;
using SilentMoon.Application.Features.Topics;
using SilentMoon.Application.Interfaces.Authentication;
using SilentMoon.Domain.Entities;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SilentMoon.Application.Features.User.Queries.GetMyTopics
{
    public class GetMyTopicsQueryHandler : IQueryHandler<GetMyTopicsQuery, List<TopicResponse>>
    {
        private readonly IUow _uow;
        private readonly ICurrentUser _currentUser;

        public GetMyTopicsQueryHandler(
            IUow uow,
            ICurrentUser currentUser)
        {
            _uow = uow;
            _currentUser = currentUser;
        }

        public async Task<Result<List<TopicResponse>>> Handle(
            GetMyTopicsQuery query,
            CancellationToken ct)
        {
            var userTopicRepo = _uow.GetRepository<UserTopic>();
            var topicRepo = _uow.GetRepository<Topic>();
            var translationRepo = _uow.GetRepository<Translation>();

            var userTopics = await userTopicRepo.GetAllAsync(ct);
            var topics = await topicRepo.GetAllAsync(ct);
            var translations = (await translationRepo.GetAllAsync(ct))
                .ToLanguageLookup(CultureInfo.CurrentUICulture.TwoLetterISOLanguageName);

            var myTopicIds = userTopics
                .Where(x => x.ApplicationUserId == _currentUser.UserId)
                .Select(x => x.TopicId)
                .ToHashSet();

            return topics
                .Where(x => myTopicIds.Contains(x.Id))
                .Select(x => new TopicResponse
                {
                    Id = x.Id,
                    Name = translations.Localize(TranslationKeys.For("Topic", x.Id, "Name"), x.Name)
                })
                .ToList();
        }
    }
}
