using PollBuilder.Domain.Entities;

namespace PollBuilder.Application.Interfaces.Repositories;

public interface IVoteRepository
{
    Task AddAsync(Vote vote);

    Task<bool> HasUserVotedAsync(
        Guid pollId,
        string fingerprint);

    Task SaveChangesAsync();
}