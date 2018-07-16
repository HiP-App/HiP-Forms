using System;
using System.Threading.Tasks;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.FeatureToggling;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Models.User;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Common;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Common.Contracts;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Helpers;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.ServiceAccessLayer;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.ServiceAccessLayer.AuthenticationApiAccess;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.ServiceAccessLayer.Exceptions;

namespace PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.UserManagement
{
    public interface IUserManager
    {
        Task<UserStatus> Login(User user);
        Task<UserStatus> Logout(User user);
        Task<UserStatus> Register(User user, string Firstname, string Lastname);
        Task<UserStatus> ForgotPassword(User user);
    }

    public class UserManager : IUserManager
    {
        private static readonly IAuthApiAccess AuthApiAccess = IoCManager.Resolve<IAuthApiAccess>();

        public async Task<UserStatus> Login(User user)
        {
            try
            {
                if (!CheckNetworkAccess())
                {
                    user.CurrentStatus = UserStatus.NetworkConnectionFailed;
                    return user.CurrentStatus;
                }

                user.Token = await AuthApiAccess.Login(user.Email, user.Password);
                user.CurrentStatus = UserStatus.LoggedIn;
                Settings.Username = user.Username;
                Settings.Password = user.Password;
                Settings.EMail = user.Email;
                Settings.AccessToken = user.Token.AccessToken; 
                await IoCManager.Resolve<IFeatureToggleRouter>().RefreshEnabledFeaturesAsync();
            }

            catch (Exception ex)
            {
                if (ex is NetworkAccessFailedException)
                {
                    user.CurrentStatus = UserStatus.NetworkConnectionFailed;
                }

               /* if (ex is InvalidUserNamePassword)
                {
                    user.CurrentStatus = UserStatus.IncorrectUserNameAndPassword;
                    
                }*/

                if (ex is InvalidEmailPassword)
                {
                     user.CurrentStatus = UserStatus.IncorrectEmailAndPassword;

                }
                else
                {
                    user.CurrentStatus = UserStatus.UnknownError;
                }
            }

            return user.CurrentStatus;
        }

        public async Task<UserStatus> Logout(User user)
        {
            user.Token = null;
            await IoCManager.Resolve<IFeatureToggleRouter>().RefreshEnabledFeaturesAsync();
            return UserStatus.LoggedOut;
        }

        public async Task<UserStatus> Register(User user, string firstname, string lastname)
        {
            var isRegistered = await AuthApiAccess.Register(user.Username, user.Password, firstname, lastname, user.Email);

            if (isRegistered)
            {
                return UserStatus.Registered;
            }
            else
            {
                return UserStatus.UnknownError;
            }
        }

        public bool CheckNetworkAccess()
        {
            var networkAccessStatus = IoCManager.Resolve<INetworkAccessChecker>().GetNetworkAccessStatus();
            return networkAccessStatus != NetworkAccessStatus.NoAccess;
        }

        public async Task<UserStatus> ForgotPassword(User user)
        {
            if (!CheckNetworkAccess())
            {
                user.CurrentStatus = UserStatus.NetworkConnectionFailed;
            }
            else
            {
                var isResetPasswordEmailSent = await AuthApiAccess.ForgotPassword(user.Email);

                if (isResetPasswordEmailSent)
                {
                    user.CurrentStatus = UserStatus.PasswordResetEmailSent;
                }
                else
                {
                    user.CurrentStatus = UserStatus.UnknownError;
                }
            }

            return user.CurrentStatus;
        }
    }
}