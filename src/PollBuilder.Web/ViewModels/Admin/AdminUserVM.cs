namespace PollBuilder.Web.ViewModels.Admin
{
    public class AdminUserVM
    {
        public string Id { get; set; } = string.Empty;

        public string UserName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public int PollCount { get; set; }

        public int VoteCount { get; set; }
    }
}