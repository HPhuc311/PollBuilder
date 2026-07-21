using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PollBuilder.Application.Features.Polls.Commands.ClosePoll;
using PollBuilder.Application.Features.Polls.Commands.DeletePoll;
using PollBuilder.Application.Features.Polls.Queries.GetPollResult;
using PollBuilder.Application.Interfaces.Repositories;
using PollBuilder.Infrastructure.Identity;
using PollBuilder.Web.ViewModels.Result;

namespace PollBuilder.Web.Controllers;

public class ResultController : Controller
{
    private readonly IMediator _mediator;
    private readonly UserManager<ApplicationUser> _userManager;

    private readonly IPollRepository _repository;

    public ResultController(IMediator mediator, UserManager<ApplicationUser> userManager,
    IPollRepository repository)
    {
        _mediator = mediator;
        _userManager = userManager;
        _repository = repository;
    }

    [HttpGet]
    public async Task<IActionResult> Index(Guid id)
    {
        var result = await _mediator.Send(
            new GetPollResultQuery(id));

        if (result == null)
            return NotFound();

        var vm = new PollResultVM
        {
            PollId = result.PollId,

            Title = result.Title,

            Description = result.Description,

            CreatedAt = result.CreatedAt,

            ExpiredAt = result.ExpiredAt,

            IsClosed = result.IsClosed,

            CreatedById = result.CreatedById,

            TotalVotes = result.TotalVotes,

            CanManage = User.Identity!.IsAuthenticated &&
         (
             User.IsInRole("Admin") ||
             result.CreatedById ==
             User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
         ),

            Options = result.Options
         .Select(x => new PollOptionVM
         {
             Content = x.Content,
             VoteCount = x.VoteCount,
             Percentage = x.Percentage
         })
         .ToList()
        };

        return View(vm);
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    [ActionName("Delete")]
    public async Task<IActionResult> DeleteConfirmed(Guid id)
    {
        var poll = await _repository.GetByIdAsync(id);

        if (poll == null)
            return NotFound();

        var currentUser = await _userManager.GetUserAsync(User);

        if (currentUser == null)
            return Challenge();

        bool isAdmin = await _userManager.IsInRoleAsync(currentUser, "Admin");

        if (!isAdmin && poll.CreatedById != currentUser.Id)
        {
            return Forbid();
        }

        await _mediator.Send(new DeletePollCommand(id));


        return Json(new
        {
            success = true,
            message = "Poll deleted successfully."
        });
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Close(Guid id)
    {
        var poll = await _repository.GetByIdAsync(id);

        if (poll == null)
            return Json(new
            {
                success = false,
                message = "Poll not found."
            });

        var currentUser = await _userManager.GetUserAsync(User);

        if (currentUser == null)
            return Challenge();

        bool isAdmin =
            await _userManager.IsInRoleAsync(currentUser, "Admin");

        // Chỉ Admin hoặc người tạo poll mới được đóng
        if (!isAdmin && poll.CreatedById != currentUser.Id)
        {
            return Json(new
            {
                success = false,
                message = "You are not authorized."
            });
        }

        await _mediator.Send(new ClosePollCommand(id));

        return Json(new
        {
            success = true,
            message = "Poll closed successfully."
        });
    }
}