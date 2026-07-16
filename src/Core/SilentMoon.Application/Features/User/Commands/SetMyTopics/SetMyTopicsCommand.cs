using Application.Abstractions.Messaging;
using SilentMoon.Application.Features.Topics;
using System.Collections.Generic;

namespace SilentMoon.Application.Features.User.Commands.SetMyTopics
{
    public class SetMyTopicsCommand : ICommand<List<TopicResponse>>
    {
        public List<int> TopicIds { get; set; }
    }
}
