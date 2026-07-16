using Application.Abstractions.Messaging;
using SilentMoon.Application.Features.Topics;
using System.Collections.Generic;

namespace SilentMoon.Application.Features.User.Queries.GetMyTopics
{
    public class GetMyTopicsQuery : IQuery<List<TopicResponse>>
    {
    }
}
