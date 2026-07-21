using MediatR;
using Microsoft.AspNetCore.Mvc;
using PollBuilder.Application.Features.Polls.Commands.VotePoll;
using PollBuilder.Application.Features.Polls.Queries.GetPollById;
using PollBuilder.Web.ViewModels.Vote;
using Microsoft.AspNetCore.RateLimiting;
using PollBuilder.Infrastructure.Services;

namespace PollBuilder.Web.Controllers;

public class VoteController : Controller
{
    private readonly IMediator _mediator;
    private readonly QrCodeService _qrCodeService;

    public VoteController(
        IMediator mediator,
        QrCodeService qrCodeService)
    {
        _mediator = mediator;
        _qrCodeService = qrCodeService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(Guid id)
    {
        var poll = await _mediator.Send(new GetPollByIdQuery(id));

        if (poll == null)
            return NotFound();

        var vm = new VotePollVM
        {
            PollId = poll.Id,
            Title = poll.Title,
            Description = poll.Description,
            IsClosed = poll.IsClosed,
            Options = poll.Options.Select(x => new VoteOptionVM
            {
                Id = x.Id,
                Content = x.Content
            }).ToList()
        };

        string voteUrl =
            $"{Request.Scheme}://{Request.Host}/Vote/Index?id={poll.Id}";

        ViewBag.VoteUrl = voteUrl;
        ViewBag.QrCode = _qrCodeService.GenerateQrCode(voteUrl);

        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [EnableRateLimiting("VoteLimiter")]
    public async Task<IActionResult> Index(VotePollVM model)
    {
        if (!ModelState.IsValid)
        {
            return Json(new
            {
                success = false,
                message = "Please select an option."
            });
        }
        var command = new VotePollCommand
        {
            PollId = model.PollId,
            PollOptionId = model.SelectedOptionId,
            IPAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown",
            Fingerprint = GetFingerprint()
        };

        var poll = await _mediator.Send(new GetPollByIdQuery(model.PollId));

        if (poll == null)
        {
            return Json(new
            {
                success = false,
                message = "Poll not found."
            });
        }

        if (poll.IsClosed)
        {
            return Json(new
            {
                success = false,
                message = "This poll has been closed. Voting is no longer allowed."
            });
        }

        var result = await _mediator.Send(command);

        if (!result)
        {
            return Json(new
            {
                success = false,
                message = "Unable to submit your vote."
            });
        }
        return Json(new
        {
            success = true,
            message = "Thank you for your vote.",
            redirectUrl = Url.Action(nameof(Success))
        });
    }


    public IActionResult Success()
    {
        return View();
    }

    private string GetFingerprint()
    {
        const string cookieName = "PollBuilderFingerprint";

        if (Request.Cookies.TryGetValue(cookieName, out var fingerprint))
            return fingerprint!;

        fingerprint = Guid.NewGuid().ToString("N");

        Response.Cookies.Append(
            cookieName,
            fingerprint,
            new CookieOptions
            {
                HttpOnly = true,
                IsEssential = true,
                Expires = DateTimeOffset.UtcNow.AddYears(1)
            });

        return fingerprint;
    }

    //[HttpGet("/poll/{code}")]
    //public async Task<IActionResult> Poll(string code)
    //{
    //    var poll = await _pollRepository.GetByCodeAsync(code);

    //    if (poll == null)
    //        return NotFound();

    //    return RedirectToAction("Index", new { id = poll.Id });
    //}
}