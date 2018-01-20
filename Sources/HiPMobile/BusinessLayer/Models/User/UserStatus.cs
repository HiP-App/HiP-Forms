namespace PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Models.User
{
    public enum UserStatus
    {
        LoggedIn,
        LoggedOut,
        IncorrectUserNameAndPassword,
        ServerError,
        NetworkConnectionFailed,
        Registered,
        UnknownError,
        PasswordResetEmailSent
    }
}