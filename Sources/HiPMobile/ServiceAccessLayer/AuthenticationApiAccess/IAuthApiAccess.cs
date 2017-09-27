using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.ServiceAccessLayer.AuthApiDto;

namespace PaderbornUniversity.SILab.Hip.Mobile.Shared.ServiceAccessLayer.AuthenticationApiAccess
{
    public interface IAuthApiAccess
    {
        Task<Token> Login(string username, string password);

        Task<bool> Register(string username, string password, string confirmPassword);

        Task<bool> ForgotPassword(string username);
    }
}
