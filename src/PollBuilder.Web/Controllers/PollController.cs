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

    public PollController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var polls = await _mediator.Send(
            new GetAllPollsQuery()
        );

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
    public async Task<IActionResult> Create(
        CreatePollVM model
    )
    {
        ValidateOptionalOptions(model);
        ValidateExpirationDate(model);

        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values
                .SelectMany(value => value.Errors)
                .Select(error => error.ErrorMessage)
                .Where(message =>
                    !string.IsNullOrWhiteSpace(message)
                )
                .Distinct()
                .ToList();

            /*
             * Form hiện đang submit bằng AJAX.
             * Vì vậy cần trả JSON thay vì return View(model).
             */
            return Json(new
            {
                success = false,

                message = errors.FirstOrDefault()
                          ?? "Please check the entered information.",

                errors
            });
        }

        var userId = User.FindFirstValue(
            ClaimTypes.NameIdentifier
        );

        if (string.IsNullOrWhiteSpace(userId))
        {
            return Json(new
            {
                success = false,
                message = "You must sign in before creating a poll."
            });
        }

        var command = new CreatePollCommand
        {
            Title = model.Title.Trim(),

            Description =
                string.IsNullOrWhiteSpace(model.Description)
                    ? null
                    : model.Description.Trim(),

            ExpiredAt = DateTime.SpecifyKind(
                    model.ExpiredAt!.Value,
                    DateTimeKind.Local
                )
                .ToUniversalTime(),

            CreatedById = userId
        };

        AddOption(command, model.Option1);
        AddOption(command, model.Option2);
        AddOption(command, model.Option3);
        AddOption(command, model.Option4);
        AddOption(command, model.Option5);
        AddOption(command, model.Option6);

        var pollId = await _mediator.Send(command);

        return Json(new
        {
            success = true,
            message = "Poll created successfully.",

            redirectUrl = Url.Action(
                "Index",
                "Vote",
                new { id = pollId }
            )
        });
    }

    private void ValidateOptionalOptions(
        CreatePollVM model
    )
    {
        var activeOptions = (
                model.ActiveOptionalOptions
                ?? string.Empty
            )
            .Split(
                ',',
                StringSplitOptions.RemoveEmptyEntries |
                StringSplitOptions.TrimEntries
            )
            .ToHashSet(
                StringComparer.OrdinalIgnoreCase
            );

        ValidateActiveOption(
            activeOptions,
            nameof(model.Option3),
            model.Option3,
            "Option 3 is required."
        );

        ValidateActiveOption(
            activeOptions,
            nameof(model.Option4),
            model.Option4,
            "Option 4 is required."
        );

        ValidateActiveOption(
            activeOptions,
            nameof(model.Option5),
            model.Option5,
            "Option 5 is required."
        );

        ValidateActiveOption(
            activeOptions,
            nameof(model.Option6),
            model.Option6,
            "Option 6 is required."
        );
    }

    private void ValidateActiveOption(
        HashSet<string> activeOptions,
        string propertyName,
        string? value,
        string errorMessage
    )
    {
        if (
            activeOptions.Contains(propertyName) &&
            string.IsNullOrWhiteSpace(value)
        )
        {
            ModelState.AddModelError(
                propertyName,
                errorMessage
            );
        }
    }

    private void ValidateExpirationDate(
        CreatePollVM model
    )
    {
        if (!model.ExpiredAt.HasValue)
        {
            ModelState.AddModelError(
                nameof(model.ExpiredAt),
                "Expiration date is required."
            );

            return;
        }

        if (model.ExpiredAt.Value <= DateTime.Now)
        {
            ModelState.AddModelError(
                nameof(model.ExpiredAt),
                "Expiration time must be greater than the current time."
            );
        }
    }

    private static void AddOption(
        CreatePollCommand command,
        string? option
    )
    {
        if (!string.IsNullOrWhiteSpace(option))
        {
            command.Options.Add(option.Trim());
        }
    }
}