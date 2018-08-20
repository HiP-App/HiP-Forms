using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Newtonsoft.Json;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.FeatureToggling;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Models.User;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Common;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Common.Contracts;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Helpers;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.ServiceAccessLayer;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.ServiceAccessLayer.AuthenticationApiAccess;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.ServiceAccessLayer.Exceptions;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.ServiceAccessLayer.UserApiAccesses;

namespace PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.UserManagement
{
    public interface IUserManager
    {
        Task<UserStatus> Login(User user);
        Task<UserStatus> Logout(User user);
        Task<UserStatus> Register(User user, string firstName, string lastName);
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

                user.Token = await AuthApiAccess.Login(user.Username, user.Password);
                user.CurrentStatus = UserStatus.LoggedIn;
                Settings.Username = user.Username;
                Settings.Password = user.Password;
                Settings.AccessToken = user.Token.AccessToken;
                Settings.ProfilePicture = null;

                var currentUser = await GetCurrentUser(user.Token.AccessToken);
                if (currentUser.Id != null)
                {
                    Settings.UserId = currentUser.Id;
                }
                

                await IoCManager.Resolve<IFeatureToggleRouter>().RefreshEnabledFeaturesAsync();
            }

            catch (Exception ex)
            {
                if (ex is NetworkAccessFailedException)
                {
                    user.CurrentStatus = UserStatus.NetworkConnectionFailed;
                }

                if (ex is InvalidUserNamePassword)
                {
                    user.CurrentStatus = UserStatus.IncorrectUserNameAndPassword;
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

        public async Task<UserStatus> Register(User user, string firstName, string lastName)
        {
            var isRegistered = await AuthApiAccess.Register(user.Username, user.Password, firstName, lastName);

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
                var isResetPasswordEmailSent = await AuthApiAccess.ForgotPassword(user.Username);

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

        public async Task<CurrentUser> GetCurrentUser(string accessToken)
        {
            var path = "/Me";
            UserApiClient userClient = new UserApiClient();
            var response = await userClient.GetResponseFromUrlAsString(path, accessToken);
            if (response != null)
            {
                return JsonConvert.DeserializeObject<CurrentUser>(response);
            }

            return new CurrentUser(null, null);
        }
    }

    public class CurrentUser
    {
        [JsonProperty("id")]
        public string Id { get; private set; }

        [JsonProperty("email")]
        public string EMail { get; private set; }

        public CurrentUser([CanBeNull] string id, [CanBeNull] string email)
        {
            Id = id;
            EMail = email;
        }

    }
}