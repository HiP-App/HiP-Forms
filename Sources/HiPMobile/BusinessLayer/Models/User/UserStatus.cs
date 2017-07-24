namespace PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Models.User
    {
    public enum UserStatus
        {
        LoggedIn,
        LoggedOut,
        InCorrectUserNameandPassword,
        ServerError,
        NetworkConnectionFailed,
        Registered,
        UnkownError,
        PasswordResetEmailSent
        }
    }
