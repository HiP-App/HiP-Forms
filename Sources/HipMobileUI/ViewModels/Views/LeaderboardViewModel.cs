using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Models.User;

namespace PaderbornUniversity.SILab.Hip.Mobile.UI.ViewModels.Views
{
    class LeaderboardViewModel : NavigationViewModel {

        private static Ranking[] leaderboard;

        public LeaderboardViewModel ()
        {
            CreateLeaderboard ();
        }

        public Ranking[] Leaderboard {
            get { return leaderboard; }
            set { SetProperty(ref leaderboard, value); }
        }

        public Ranking OwnRanking {
            get { return Leaderboard [0]; }
        }

        private void CreateLeaderboard ()
        {
            int numberOfUsersInLeaderboard = 10;

            Leaderboard = new Ranking[numberOfUsersInLeaderboard];

            for (int i = 0; i < numberOfUsersInLeaderboard; i++)
            {
                Leaderboard[i] = new Ranking (i + 1, 100 - i, "User " + (i + 1));
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
