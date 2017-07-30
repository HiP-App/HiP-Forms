using PaderbornUniversity.SILab.Hip.Mobile.Shared.ServiceAccessLayer.AuthApiDto;

namespace PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Models.User
    {
    public class User
        {
        public string UserName { get; }
        public string Password { get; }

       public User (string userName, string password)
       {
           UserName = userName;
           Password = password;
       }

        public Token Token { get; internal set; }

        public UserStatus CurrentStatus { get; set; }

        }
    }
