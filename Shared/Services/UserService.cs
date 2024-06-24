using Shared.Data;
using Shared.Models;

namespace Shared.Services
{
    public class UserService : IUserService
    {
        private readonly SharedDbContext _context;

        public UserService(SharedDbContext context)
        {
            _context = context;
        }

        private string HashPassword(string password)
        {
            return password; // TODO: Implement
        }

        public int CreateUser(string name, string surname, string email, string password)
        {
            if (_context.Users.Where(u => u.Email == email).Count() > 0)
                return -1; // Email must be unique
            User user =
                new()
                {
                    Name = name,
                    Surname = surname,
                    Email = email,
                    Password = HashPassword(password)
                };
            _context.Users.Add(user);
            _context.SaveChanges();
            return user.ID;
        }

        public int LoginUser(string email, string password)
        {
            var user = _context.Users.FirstOrDefault(u =>
                u.Email == email && u.Password == password
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
            if (sender == null || receiver == null)
                return false;

            if (
                !allowDebt
                && (
                    amount > 0 && amount > sender.Balance
                    || amount < 0 && -amount > receiver.Balance
                )
            )
                return false;

            sender.Balance -= amount;
            receiver.Balance += amount;
            _context.SaveChanges();
            return true;
        }
    }
}
