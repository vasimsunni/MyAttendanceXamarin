using System.Threading.Tasks;

namespace MyAttendance.Firebase
{
    public interface IFirebaseAuth
    {
        Task<string> LoginWithEmailPassword(string email, string password);
    }
}
