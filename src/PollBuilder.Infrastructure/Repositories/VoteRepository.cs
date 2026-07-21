using Microsoft.EntityFrameworkCore;
using PollBuilder.Application.Interfaces.Repositories;
using PollBuilder.Domain.Entities;
using PollBuilder.Infrastructure.Persistence;

namespace PollBuilder.Infrastructure.Repositories;

public class VoteRepository : IVoteRepository
{
    private readonly ApplicationDbContext _context;

    public VoteRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Vote vote)
    {
        await _context.Votes.AddAsync(vote);
    }

    public async Task<bool> HasUserVotedAsync(
        Guid pollId,
        string fingerprint)
    {
        return await _context.Votes
            .AnyAsync(x =>
                x.PollId == pollId &&
                x.FingerPrint == fingerprint);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}