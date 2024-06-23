using cldv6211proj.Models.Database;
using cldv6211proj.Models.ViewModels;

namespace cldv6211proj.Services
{
    public interface IUserService
    {
        int CreateUser(UserRegister userRegister);
        int LoginUser(UserLogin userLogin);
        User? GetUser(int userID);
        double GetBalance(int userID);
        bool UpdateBalance(int userID, double delta);
        bool TransferBalance(int fromUserID, int toUserID, double balance, bool allowDebt = true);
    }
}
