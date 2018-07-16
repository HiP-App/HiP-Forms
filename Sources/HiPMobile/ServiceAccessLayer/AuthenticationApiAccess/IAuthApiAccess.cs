using System.Threading.Tasks;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.ServiceAccessLayer.AuthApiDto;

namespace PaderbornUniversity.SILab.Hip.Mobile.Shared.ServiceAccessLayer.AuthenticationApiAccess
{
    public interface IAuthApiAccess
    {
        ///Task<Token> Login(string username, string password);
        Task<Token> Login(string email, string password);

        Task<bool> Register(string username, string password, string Firstname, string Lastname, string email);

        //Task<bool> ForgotPassword(string username);
        Task<bool> ForgotPassword(string email);
    }
}