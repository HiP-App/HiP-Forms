namespace PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Models.User
{
    public enum UserStatus
    {
        LoggedIn,
        LoggedOut,
        //IncorrectUserNameAndPassword,
        IncorrectEmailAndPassword,
        ServerError,
        NetworkConnectionFailed,
        Registered,
        UnknownError,
        PasswordResetEmailSent
    }
}