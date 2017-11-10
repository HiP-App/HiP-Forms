using PaderbornUniversity.SILab.Hip.Mobile.UI.ViewModels.Pages;

namespace PaderbornUniversity.SILab.Hip.Mobile.UI.ViewModels.Views
{
    class LeaderboardViewModel : ExtendedNavigationViewModel
    {
        private static Ranking[] leaderboard;

        public LeaderboardViewModel()
        {
            CreateLeaderboard();
        }

        public Ranking[] Leaderboard
        {
            get { return leaderboard; }
            set { SetProperty(ref leaderboard, value); }
        }

        public Ranking OwnRanking => Leaderboard[0];

        private void CreateLeaderboard()
        {
            const int numberOfUsersInLeaderboard = 10;

            Leaderboard = new Ranking[numberOfUsersInLeaderboard];

            for (var i = 0; i < numberOfUsersInLeaderboard; i++)
            {
                Leaderboard[i] = new Ranking(i + 1, 100 - i, "User " + (i + 1));
            }
        }
    }

    internal class Ranking
    {
        public Ranking(int position, int points, string username)
        {
            Position = position;
            Points = points;
            Username = username;
        }

        public int Position { get; set; }

        public int Points { get; set; }

        public string Username { get; set; }
    }
}