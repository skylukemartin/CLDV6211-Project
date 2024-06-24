using Shared.Models;

namespace Shared.Services
{
    public interface IUserService
    {
        int CreateUser(string name, string surname, string email, string password);
        int LoginUser(string email, string password);
        User? GetUser(int userID);
        double GetBalance(int userID);
        bool UpdateBalance(int userID, double delta);
        bool TransferBalance(int fromUserID, int toUserID, double balance, bool allowDebt = true);
    }
}
