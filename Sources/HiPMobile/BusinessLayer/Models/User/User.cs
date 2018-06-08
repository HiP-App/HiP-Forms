using PaderbornUniversity.SILab.Hip.Mobile.Shared.ServiceAccessLayer.AuthApiDto;

namespace PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Models.User
{
    public class User
    {
        public string Username { get; } // userame is taking email of real usr
        public string Password { get; }
        public string Email { get; } // taking value of max@power

        public User(string username, string password, string email)
        {
            Username = username;
            Password = password;
            Email = email;
        }

        public Token Token { get; internal set; }

        public UserStatus CurrentStatus { get; set; }
    }
}