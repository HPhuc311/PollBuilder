using PollBuilder.Domain.Entities;

namespace PollBuilder.Application.Interfaces.Repositories;

public interface IPollRepository
{
    Task AddAsync(Poll poll);

    Task<Poll?> GetByIdAsync(Guid id);

    Task<List<Poll>> GetAllAsync();

    Task SaveChangesAsync();

    Task<bool> PollExistsAsync(Guid pollId);

    Task<bool> IsPollClosedAsync(Guid pollId);

    Task<bool> OptionBelongsToPollAsync(Guid pollId, Guid optionId);

    Task DeleteAsync(Guid id);


}