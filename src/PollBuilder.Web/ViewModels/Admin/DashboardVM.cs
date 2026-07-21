namespace PollBuilder.Web.ViewModels.Admin
{
    public class DashboardVM
    {
        public int TotalUsers { get; set; }

        public int TotalPolls { get; set; }

        public int TotalVotes { get; set; }

        public int ActivePolls { get; set; }

        public List<AdminUserVM> TopUsers { get; set; } = new();

        public List<AdminPollVM> RecentPolls { get; set; } = new();
    }
}