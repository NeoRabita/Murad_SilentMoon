using Application.Abstractions.Messaging;
using SilentMoon.Application.Common.Pagination;

namespace SilentMoon.Application.Features.Player.Queries.GetHistory
{
    public class GetHistoryQuery : PagedQuery, IQuery<PagedResponse<HistoryItemResponse>>
    {
    }
}
