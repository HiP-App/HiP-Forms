using System.Threading.Tasks;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Models.User;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.ServiceAccessLayer.AuthApiDto;

namespace PaderbornUniversity.SILab.Hip.Mobile.Shared.ServiceAccessLayer.AuthenticationApiAccess
{
    public interface IAuthApiAccess
    {
        Task<Token> Login(string email, string password);

        Task<bool> Register(string username, string password, string firstName, string lastName, string email);

        Task<bool> ForgotPassword(string email);

        Task<CurrentUser> GetCurrentUser(string accessToken);
    
    }
}