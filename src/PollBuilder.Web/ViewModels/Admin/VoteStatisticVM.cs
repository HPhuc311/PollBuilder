namespace PollBuilder.Web.ViewModels.Admin
{
    public class VoteStatisticVM
    {
        public Guid PollId { get; set; }

        public string PollTitle { get; set; } = string.Empty;

        public int VoteCount { get; set; }

        public double Percentage { get; set; }
    }
}