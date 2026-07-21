using Microsoft.AspNetCore.SignalR;
using PollBuilder.Application.Interfaces.Notifications;
using PollBuilder.Web.Hubs;

namespace PollBuilder.Infrastructure.Services;

public class SignalRPollNotifier : IPollNotifier
{
    private readonly IHubContext<PollHub> _hubContext;

    public SignalRPollNotifier(
        IHubContext<PollHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task NotifyVoteUpdated(Guid pollId)
    {
        await _hubContext
            .Clients
            .Group(pollId.ToString())
            .SendAsync("VoteUpdated", pollId);
    }
}