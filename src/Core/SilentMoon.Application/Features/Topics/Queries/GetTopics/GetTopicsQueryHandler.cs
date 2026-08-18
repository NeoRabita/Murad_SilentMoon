using Application.Abstractions.Messaging;
using SilentMoon.Application.Common.Extensions;
using SilentMoon.Domain.Entities;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SilentMoon.Application.Features.Topics.Queries.GetTopics
{
    public class GetTopicsQueryHandler : IQueryHandler<GetTopicsQuery, List<TopicResponse>>
    {
        private readonly IUow _uow;

        public GetTopicsQueryHandler(IUow uow)
        {
            _uow = uow;
        }

        public async Task<Result<List<TopicResponse>>> Handle(
            GetTopicsQuery query,
            CancellationToken ct)
        {
            var topicRepo = _uow.GetRepository<Topic>();
            var translationRepo = _uow.GetRepository<Translation>();

            var topics = await topicRepo.GetAllAsync(ct);
            var translations = (await translationRepo.GetAllAsync(ct))
                .ToLanguageLookup(CultureInfo.CurrentUICulture.TwoLetterISOLanguageName);

            return topics
                .Select(x => new TopicResponse
                {
                    Id = x.Id,
                    Name = translations.Localize(TranslationKeys.Topic(x.Id, "Name"), x.Name)
                })
                .ToList();
        }
    }
}
