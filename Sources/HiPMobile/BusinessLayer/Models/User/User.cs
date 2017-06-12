using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.UserManagement;
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
        private static UserManager instance = null;

        public Token Token { get; internal set; }

        public UserStatus CurrentStatus { get; set; }

        private Error Error { get; set; }
        }
    }
