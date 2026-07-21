using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PollBuilder.Application.Features.Polls.Commands.DeletePoll;
using PollBuilder.Infrastructure.Identity;
using PollBuilder.Infrastructure.Persistence;
using PollBuilder.Web.ViewModels.Admin;

namespace PollBuilder.Web.Controllers;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IMediator _mediator;

    public AdminController(
      ApplicationDbContext context,
      UserManager<ApplicationUser> userManager,
      IMediator mediator)
    {
        _context = context;
        _userManager = userManager;
        _mediator = mediator;
    }

    public async Task<IActionResult> Dashboard()
    {
        var model = new DashboardVM
        {
            TotalUsers = await _userManager.Users.CountAsync(),
            TotalPolls = await _context.Polls.CountAsync(),
            TotalVotes = await _context.Votes.CountAsync(),
            ActivePolls = await _context.Polls.CountAsync(x => !x.IsClosed)
        };

        var users = await _userManager.Users.ToListAsync();

        foreach (var user in users)
        {
            model.TopUsers.Add(new AdminUserVM
            {
                Id = user.Id,
                UserName = string.IsNullOrWhiteSpace(user.FullName)
                    ? user.UserName ?? ""
                    : user.FullName,
                Email = user.Email ?? "",
                PollCount = await _context.Polls.CountAsync(x => x.CreatedById == user.Id),
                VoteCount = await _context.Votes.CountAsync(v => v.Poll.CreatedById == user.Id)
            });
        }

        model.TopUsers = model.TopUsers
            .OrderByDescending(x => x.PollCount)
            .Take(5)
            .ToList();

        model.RecentPolls = await _context.Polls
            .Include(x => x.Votes)
            .OrderByDescending(x => x.CreatedAt)
            .Take(5)
            .Select(x => new AdminPollVM
            {
                Id = x.Id,
                Title = x.Title,
                Description = x.Description ?? "",
                CreatedAt = x.CreatedAt,
                ExpiredAt = x.ExpiredAt,
                CreatedById = x.CreatedById,
                VoteCount = x.Votes.Count,
                IsClosed = x.IsClosed,
            })
            .ToListAsync();

        return View(model);
    }

    public async Task<IActionResult> Users()
    {
        var result = new List<AdminUserVM>();

        var users = await _userManager.Users.ToListAsync();

        foreach (var user in users)
        {
            result.Add(new AdminUserVM
            {
                Id = user.Id,
                UserName = string.IsNullOrWhiteSpace(user.FullName)
                    ? user.UserName ?? ""
                    : user.FullName,
                Email = user.Email ?? "",
                PollCount = await _context.Polls.CountAsync(x => x.CreatedById == user.Id),
                VoteCount = await _context.Votes.CountAsync(v => v.Poll.CreatedById == user.Id)
            });
        }

        return View(result.OrderByDescending(x => x.PollCount).ToList());
    }

    public async Task<IActionResult> Polls(string search)
    {
        var polls = await _context.Polls
            .Include(x => x.Votes)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();

        var result = polls.Select(x => new AdminPollVM
        {
            Id = x.Id,
            Title = x.Title,
            Description = x.Description ?? "",
            CreatedAt = x.CreatedAt,
            ExpiredAt = x.ExpiredAt,
            CreatedById = x.CreatedById,
            VoteCount = x.Votes.Count,
            IsClosed = x.IsClosed,
        }).ToList();

        if (!string.IsNullOrWhiteSpace(search))
        {
            result = result
                .Where(x => x.Title.Contains(search,
                    StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        return View(result);
    }

    public async Task<IActionResult> Votes()
    {
        var result = await _context.Polls
            .Include(x => x.Votes)
            .OrderByDescending(x => x.Votes.Count)
            .Select(x => new VoteStatisticVM
            {
                PollId = x.Id,
                PollTitle = x.Title,
                VoteCount = x.Votes.Count
            })
            .ToListAsync();

        var totalVotes = result.Sum(x => x.VoteCount);

        foreach (var item in result)
        {
            item.Percentage = totalVotes == 0
                ? 0
                : Math.Round(item.VoteCount * 100.0 / totalVotes, 2);
        }

        return View(result);
    }

    // Delete User
    [HttpPost]
    public async Task<IActionResult> DeleteUser(string id)
    {
        var user = await _userManager.FindByIdAsync(id);

        if (user == null)
            return Json(new { success = false });


        var result = await _userManager.DeleteAsync(user);

        return Json(new
        {
            success = result.Succeeded,
            message = "Poll deleted successfully."
        });
    }


    // Lock User
    [HttpPost]
    public async Task<IActionResult> LockUser(string id)
    {
        var user = await _userManager.FindByIdAsync(id);

        if (user == null)
        {
            return Json(new
            {
                success = false,
                message = "User not found."
            });
        }

        // Không cho phép tự khóa chính mình
        if (user.Id == _userManager.GetUserId(User))
        {
            return Json(new
            {
                success = false,
                message = "You cannot lock your own account."
            });
        }

        user.LockoutEnd = DateTimeOffset.UtcNow.AddYears(100);

        var result = await _userManager.UpdateAsync(user);

        return Json(new
        {
            success = result.Succeeded,
            message = result.Succeeded
                ? "User locked successfully."
                : "Unable to lock user."
        });
    }


    // Unlock User
    [HttpPost]
    public async Task<IActionResult> UnlockUser(string id)
    {
        var user = await _userManager.FindByIdAsync(id);

        if (user == null)
        {
            return Json(new
            {
                success = false,
                message = "User not found."
            });
        }

        user.LockoutEnd = null;

        var result = await _userManager.UpdateAsync(user);

        return Json(new
        {
            success = result.Succeeded,
            message = result.Succeeded
                ? "User unlocked successfully."
                : "Unable to unlock user."
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [ActionName("Delete")]
    public async Task<IActionResult> DeletePoll(Guid id)
    {
        var poll = await _context.Polls
            .FirstOrDefaultAsync(x => x.Id == id);

        if (poll == null)
        {
            return Json(new
            {
                success = false,
                message = "Poll not found."
            });
        }

        await _mediator.Send(new DeletePollCommand(id));

        return Json(new
        {
            success = true,
            message = "Poll deleted successfully."
        });
    }
}