using System.Threading.Tasks;
using DatingApp.API.Controllers;

namespace DatingApp.API.Data
{
    public interface IAuthRepository
    {
        Task<User> Register(User user, string password);

        Task<User> Login(User user, string password);

        Task<bool> UserExists(string username);     
     
    }
}