namespace cldv6211proj.Models.Db
{
    using Base;

    public class User : RecordModel
    {
        public string? Name { get; set; }
        public string? Surname { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public double? Balance { get; set; }
    }

    public static class UserManager
    {
        public static readonly Table<User> table = new();

        public static string HashPassword(string password)
        {
            return password; // TODO: implement !
        }

        public static User? Signup(User user)
        {
            if (user.Password == null)
                return null; // user needs a password ..
            user.Password = HashPassword(user.Password);

            var record = table.ModelToRecord(user);
            if (table.SelectRecord(user, ["Email"]) != null)
                return null; // email already in use

            var userID = table.AddRecord(record);
            if (userID < 1)
                return null;
            return FindUser(userID);
        }

        public static User? Login(User user)
        {
            if (user.Password == null)
                return null; // user needs a password ..
            user.Password = HashPassword(user.Password);
            var found = table.SelectRecord(user, ["Email", "Password"]);
            return found?.Model;
        }

        public static bool UpdateBalance(User user, double delta)
        {
            if (user.ID < 1)
                return false;
            user.Balance += delta;
            return table.UpdateRecord(user);
        }

        public static bool TransferBalance(User sender, User receiver, double balance)
        {
            return sender.ID > 0
                && receiver.ID > 0
                && UpdateBalance(sender, -balance)
                && UpdateBalance(receiver, balance);
        }

        public static List<User> GetAllUsers() =>
            (table.Records.Count > 0 ? table.Records : table.FetchAllRecords())
                .Select(rec => rec.Model)
                .ToList();

        public static User? FindUser(int userID) => table.LazyFindRecord(userID)?.Model;

        public static List<User>? FindUsers(Func<User, bool> where) =>
            GetAllUsers().Where(where).ToList();
    }
}
