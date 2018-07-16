using PaderbornUniversity.SILab.Hip.Mobile.Shared.ServiceAccessLayer.AuthApiDto;

namespace PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Models.User
{
    public class User
    {
        public string Username { get; }
        public string Password { get; }
        public string Email { get; }

        public User(string email, string password, string username)
        {
            Username = username; //taking email as a parameter
            Password = password;
            Email = email;

        }

        public Token Token { get; internal set; }

        public UserStatus CurrentStatus { get; set; }
    }
}