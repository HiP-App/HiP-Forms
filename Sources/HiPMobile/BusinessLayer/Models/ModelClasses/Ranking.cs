namespace PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Models
{
    class Ranking
    {

        public Ranking (int position, int points, string username)
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
