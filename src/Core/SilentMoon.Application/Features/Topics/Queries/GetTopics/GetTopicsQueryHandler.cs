using Application.Abstractions.Messaging;
using SilentMoon.Domain.Entities;
using System.Collections.Generic;
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

            var topics = await topicRepo.GetAllAsync(ct);

            return topics
                .Select(x => new TopicResponse
                {
                    Id = x.Id,
                    Name = x.Name
                })
                .ToList();
        }
    }
}
