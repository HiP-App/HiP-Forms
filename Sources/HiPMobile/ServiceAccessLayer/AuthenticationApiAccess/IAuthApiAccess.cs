using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.ServiceAccessLayer.AuthApiDto;

namespace PaderbornUniversity.SILab.Hip.Mobile.Shared.ServiceAccessLayer.AuthenticationApiAccess{
    public interface IAuthApiAccess {

     Task<Token> GetToken (string userName, string password);

     Task<bool> Register (string userName, string password, string confirmPassword);

     Task<string> ForgotPassword (string userName);

    }
    }
