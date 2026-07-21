using Microsoft.AspNetCore.SignalR;

namespace PollBuilder.Web.Hubs;

public class PollHub : Hub
{
    public override async Task OnConnectedAsync()
    {
        await base.OnConnectedAsync();
    }

    public async Task JoinPoll(string pollId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, pollId);
    }

    public async Task LeavePoll(string pollId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, pollId);
    }
}