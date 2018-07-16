// Copyright (C) 2017 History in Paderborn App - Universität Paderborn
//  
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//  
//      http://www.apache.org/licenses/LICENSE-2.0
//  
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using PaderbornUniversity.SILab.Hip.Mobile.UI.ViewModels.Pages;

namespace PaderbornUniversity.SILab.Hip.Mobile.UI.ViewModels.Views
{
    class LeaderboardViewModel : NavigationViewModel
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
       /* public Ranking(int position, int points, string username)
        {
            Position = position;
            Points = points;
            Username = username;
        } */
        
        public Ranking(int position, int points, string email)
        {
            Position = position;
            Points = points;
            Email = email;
        }

        public int Position { get; set; }

        public int Points { get; set; }

        //public string Username { get; set; }
        public string Email { get; set; }

    }
}