using Microsoft.EntityFrameworkCore;
using PollBuilder.Application.Interfaces.Repositories;
using PollBuilder.Domain.Entities;
using PollBuilder.Infrastructure.Persistence;

namespace PollBuilder.Infrastructure.Repositories;

public class PollRepository : IPollRepository
{
    private readonly ApplicationDbContext _context;

    public PollRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Poll poll)
    {
        await _context.Polls.AddAsync(poll);
    }

    public async Task<Poll?> GetByIdAsync(Guid id)
    {
        return await _context.Polls
            .Include(x => x.Options)
                .ThenInclude(o => o.Votes)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<List<Poll>> GetAllAsync()
    {
        return await _context.Polls.ToListAsync();
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }

    public async Task<bool> PollExistsAsync(Guid pollId)
    {
        return await _context.Polls
            .AnyAsync(x => x.Id == pollId);
    }

    public async Task<bool> IsPollClosedAsync(Guid pollId)
    {
        var poll = await _context.Polls
            .FirstOrDefaultAsync(x => x.Id == pollId);

        if (poll == null)
            return true;

        if (poll.IsClosed)
            return true;

        if (poll.ExpiredAt.HasValue &&
            poll.ExpiredAt.Value <= DateTime.UtcNow)
            return true;

        return false;
    }

    public async Task<bool> OptionBelongsToPollAsync(
        Guid pollId,
        Guid optionId)
    {
        return await _context.PollOptions
            .AnyAsync(x =>
                x.Id == optionId &&
                x.PollId == pollId);
    }

    public async Task DeleteAsync(Guid id)
    {
        // Lấy Poll
        var poll = await _context.Polls
            .FirstOrDefaultAsync(x => x.Id == id);

        if (poll == null)
            return;

        // Lấy tất cả PollOption
        var options = await _context.PollOptions
            .Where(x => x.PollId == id)
            .ToListAsync();

        // Lấy tất cả Vote thuộc Poll
        var votes = await _context.Votes
            .Where(x => x.PollId == id)
            .ToListAsync();

        // Lấy Vote của từng Option
        var optionVotes = await _context.Votes
            .Where(x => options.Select(o => o.Id).Contains(x.PollOptionId))
            .ToListAsync();

        _context.Votes.RemoveRange(optionVotes);

        _context.Votes.RemoveRange(votes);

        _context.PollOptions.RemoveRange(options);

        _context.Polls.Remove(poll);

        await _context.SaveChangesAsync();
    }
}