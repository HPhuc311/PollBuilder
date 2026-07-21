namespace PollBuilder.Application.Interfaces.Notifications;

public interface IPollNotifier
{
    Task NotifyVoteUpdated(Guid pollId);
}