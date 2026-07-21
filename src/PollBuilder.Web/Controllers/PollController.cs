using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PollBuilder.Application.Features.Polls.Commands.CreatePoll;
using PollBuilder.Application.Features.Polls.Queries.GetAllPolls;
using PollBuilder.Web.ViewModels.Poll;
using System.Security.Claims;

namespace PollBuilder.Web.Controllers;

public class PollController : Controller
{
    private readonly IMediator _mediator;



    public PollController(
    IMediator mediator)
    {
        _mediator = mediator;

    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var polls = await _mediator.Send(new GetAllPollsQuery());
        return View(polls);
    }


    [Authorize]
    [HttpGet]
    public IActionResult Create()
    {
        return View(new CreatePollVM());
    }


    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreatePollVM model)
    {
        if (!ModelState.IsValid)
            return View(model);

        if (!model.ExpiredAt.HasValue)
        {
            ModelState.AddModelError(nameof(model.ExpiredAt),
                "Expiration date is required.");

            return View(model);
        }

        if (model.ExpiredAt.Value <= DateTime.Now)
        {
            ModelState.AddModelError(nameof(model.ExpiredAt),
                "Expiration time must be greater than the current time.");

            return View(model);
        }

        var command = new CreatePollCommand
        {
            Title = model.Title,
            Description = model.Description,

            ExpiredAt = DateTime.SpecifyKind(
                model.ExpiredAt.Value,
                DateTimeKind.Local).ToUniversalTime(),

            CreatedById = User.FindFirstValue(ClaimTypes.NameIdentifier)!
        };

        command.Options.Add(model.Option1);
        command.Options.Add(model.Option2);

        if (!string.IsNullOrWhiteSpace(model.Option3))
            command.Options.Add(model.Option3);

        if (!string.IsNullOrWhiteSpace(model.Option4))
            command.Options.Add(model.Option4);

        if (!string.IsNullOrWhiteSpace(model.Option5))
            command.Options.Add(model.Option5);

        if (!string.IsNullOrWhiteSpace(model.Option6))
            command.Options.Add(model.Option6);

        var pollId = await _mediator.Send(command);

        return Json(new
        {
            success = true,
            message = "Poll created successfully.",
            redirectUrl = Url.Action("Index", "Vote", new { id = pollId })
        });
    }




}