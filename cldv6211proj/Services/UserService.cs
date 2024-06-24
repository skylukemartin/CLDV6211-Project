using cldv6211proj.Data;
using cldv6211proj.Models.Database;
using cldv6211proj.Models.ViewModels;

namespace cldv6211proj.Services
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _context;

        public UserService(AppDbContext context)
        {
            _context = context;
        }

        private string HashPassword(string password)
        {
            return password; // TODO: Implement
        }

        public int CreateUser(UserRegister userRegister)
        {
            if (_context.Users.Where(u => u.Email == userRegister.Email).Count() > 0)
                return -1; // Email must be unique
            User user =
                new()
                {
                    Name = userRegister.Name,
                    Surname = userRegister.Surname,
                    Email = userRegister.Email,
                    Password = HashPassword(userRegister.Password!) // ? [Required]!
                };
            _context.Users.Add(user);
            _context.SaveChanges();
            return user.ID;
        }

        public int LoginUser(UserLogin userLogin)
        {
            var user = _context.Users.FirstOrDefault(u =>
                u.Email == userLogin.Email && u.Password == userLogin.Password
            );
            return user?.ID ?? -1;
        }

        public User? GetUser(int userID) => _context.Users.Find(userID);

        public double GetBalance(int userID) => _context.Users.Find(userID)?.Balance ?? 0;

        public bool UpdateBalance(int userID, double delta)
        {
            var user = _context.Users.Find(userID);
            if (user == null)
                return false;
            user.Balance += delta;
            return true;
        }

        public bool TransferBalance(
            int fromUserID,
            int toUserID,
            double amount,
            bool allowDebt = true
        )
        {
            var sender = _context.Users.Find(fromUserID);
            var receiver = _context.Users.Find(toUserID);
            if (sender == null || receiver == null || !allowDebt && sender.Balance < amount)
                return false;
            sender.Balance -= amount;
            receiver.Balance += amount;
            _context.SaveChanges();
            return true;
        }
    }
}
