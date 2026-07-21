namespace PollBuilder.Web.ViewModels.Admin
{
    public class AdminPollVM
    {
        public Guid Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }

        public DateTime? ExpiredAt { get; set; }

        public bool IsClosed { get; set; }

        public string CreatedById { get; set; } = string.Empty;

        public int VoteCount { get; set; }

        public string Code { get; set; } = string.Empty;
    }
}