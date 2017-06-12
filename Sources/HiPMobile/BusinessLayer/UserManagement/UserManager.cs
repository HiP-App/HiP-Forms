using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Models.User;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Common;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.DataAccessLayer;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.ServiceAccessLayer;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.ServiceAccessLayer.AuthApiDto;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.ServiceAccessLayer.AuthenticationApiAccess;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.ServiceAccessLayer.Exceptions;

namespace PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.UserManagement
    {
    public interface IUserManager {
        Task<UserStatus> LoginUser (User user);
        Task<UserStatus> Logout (User user);
        Task<UserStatus> RegisterUser (User user);

    }
    public class UserManager:IUserManager {

        private readonly static IAuthApiAccess AuthApiAccess = IoCManager.Resolve<IAuthApiAccess> ();
        public async Task<UserStatus> LoginUser (User user)
            {
            try
                {
                user.Token = await AuthApiAccess.GetToken (user.UserName, user.Password);
                user.CurrentStatus = UserStatus.LoggedIn;
                
                }
            catch (InvalidUserNamePassword)
                {
                user.CurrentStatus = UserStatus.InCorrectUserNameandPassword;
                }
            return user.CurrentStatus;
            }

        public  async Task<UserStatus> Logout (User user)
            {
            user.Token = null;
            return UserStatus.LoggedOut;
            }

        public async Task<UserStatus> RegisterUser (User user)
        {
        return UserStatus.Registered;
        }

        }
    }

