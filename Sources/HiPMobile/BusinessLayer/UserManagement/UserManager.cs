﻿using System;
using System.Threading.Tasks;
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
        Task<UserStatus> Register(User user);
        Task<UserStatus> ForgotPassword(User user);
    }

    public class UserManager : IUserManager
    {
        private readonly static IAuthApiAccess AuthApiAccess = IoCManager.Resolve<IAuthApiAccess>();

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
            }

            catch (Exception ex)
            {
                if (ex is NetworkAccessFailedException)
                    user.CurrentStatus = UserStatus.NetworkConnectionFailed;

                if (ex is InvalidUserNamePassword)
                    user.CurrentStatus = UserStatus.InCorrectUserNameandPassword;

                else
                    user.CurrentStatus = UserStatus.UnkownError;
            }
            return user.CurrentStatus;
        }

        public async Task<UserStatus> Logout(User user)
        {
            user.Token = null;
            return UserStatus.LoggedOut;
        }

        public async Task<UserStatus> Register(User user)
        {
            bool isRegistered = await AuthApiAccess.Register(user.Username, user.Password);

            if (isRegistered)
            {
                return UserStatus.Registered;
            }
            else
            {
                return UserStatus.UnkownError;
            }
        }

        public bool CheckNetworkAccess()
        {
            var networkAccessStatus = IoCManager.Resolve<INetworkAccessChecker>().GetNetworkAccessStatus();
            if (networkAccessStatus == NetworkAccessStatus.NoAccess)
                return false;
            return true;
        }

        public async Task<UserStatus> ForgotPassword(User user)
        {
            if (!CheckNetworkAccess())
            {
                user.CurrentStatus = UserStatus.NetworkConnectionFailed;
            }
            else
            {
                bool isResetPasswordEmailSent = await AuthApiAccess.ForgotPassword(user.Username);

                if (isResetPasswordEmailSent)
                {
                    user.CurrentStatus = UserStatus.PasswordResetEmailSent;
                }
                else
                {
                    user.CurrentStatus = UserStatus.UnkownError;
                }
            }

            return user.CurrentStatus;
        }
    }
}