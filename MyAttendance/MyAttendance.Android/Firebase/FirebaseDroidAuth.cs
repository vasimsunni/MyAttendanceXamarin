using Firebase.Auth;
using MyAttendance.Droid.Firebase;
using MyAttendance.Firebase;
using System.Threading.Tasks;
using Xamarin.Forms;

[assembly: Dependency(typeof(FirebaseDroidAuth))]
namespace MyAttendance.Droid.Firebase
{
    public class FirebaseDroidAuth : IFirebaseAuth
    {
        public async Task<string> LoginWithEmailPassword(string email, string password)
        {
            try
            {
                IAuthResult user = await FirebaseAuth.Instance.SignInWithEmailAndPasswordAsync(email, password);
                GetTokenResult token = await user.User.GetTokenAsync(false);
                return token.Token;
            }
            catch (FirebaseAuthInvalidUserException e)
            {
                e.PrintStackTrace();
                return "";
            }
        }
    }
}