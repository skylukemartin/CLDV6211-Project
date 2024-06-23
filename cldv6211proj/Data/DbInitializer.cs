using cldv6211proj.Models.Database;

namespace cldv6211proj.Data
{
    public static class DbInitializer
    { // ref: https://learn.microsoft.com/en-us/ef/core/modeling/data-seeding
        public static void Initialize(AppDbContext context)
        {
            context.Database.EnsureCreated();
            if (context.Users.Any() && context.Products.Any() && context.Orders.Any())
                return; // Db already seeded
            if (!context.Users.Any())
                context.Users.AddRange(SeedUsers());
            if (!context.Products.Any())
                context.Products.AddRange(SeedProducts());
            if (!context.Orders.Any())
                context.Orders.AddRange(SeedOrders());
            context.SaveChanges();
        }

        static User[] SeedUsers()
        {
            return
            [
                new User()
                {
                    Name = "admin",
                    Surname = "admin",
                    Email = "admin",
                    Password = "H4shMeIfYouCan,Mr. Holms!",
                    Balance = 13333337
                },
                new User()
                {
                    Name = "James",
                    Surname = "Khumalo",
                    Email = "jk@khumalo.crafts",
                    Password = "writethisdown",
                    Balance = -13333337
                }
            ];
        }

        static Product[] SeedProducts()
        {
            return
            [
                new Product()
                {
                    Name = "Classic Copper Kettle",
                    Price = 450.00,
                    Availability = 10,
                    Description =
                        "A tried and tested copper kettle with a classic rustic aesthetic.",
                    Category = "Kitchen",
                    ImageURL = "/images/MyCrafts/copper-kettle.webp",
                    UserID = 2
                },
                new Product()
                {
                    Name = "Ceramic Mug",
                    Price = 180.00,
                    Availability = 10,
                    Description = "Hot chocolate will taste great in this.",
                    Category = "Kitchen",
                    ImageURL = "/images/MyCrafts/ceramic-mug.webp",
                    UserID = 2
                },
                new Product()
                {
                    Name = "Basket",
                    Price = 80.00,
                    Availability = 10,
                    Description = "Perfect for putting stuff in.",
                    Category = "Kitchen",
                    ImageURL = "/images/MyCrafts/basket.webp",
                    UserID = 2
                },
                new Product()
                {
                    Name = "Clay Flowerpot",
                    Price = 888.00,
                    Availability = 10,
                    Description = "This pot is perfect for flowers or even small trees.",
                    Category = "Kitchen",
                    ImageURL = "/images/MyCrafts/clay-flowerpot.webp",
                    UserID = 2
                },
                new Product()
                {
                    Name = "Leather Journal",
                    Price = 175.00,
                    Availability = 10,
                    Description =
                        "This can be used to write things down. Handcrafted with premium home-made leather (ethically sourced from animals with their written consent).",
                    Category = "Crafts",
                    ImageURL = "/images/MyCrafts/leather-journal.webp",
                    UserID = 2
                },
                new Product()
                {
                    Name = "Leather Wallet",
                    Price = 150.00,
                    Availability = 10,
                    Description =
                        "Do you suffer from too much pocket space? Try putting your money in this!",
                    Category = "Crafts",
                    ImageURL = "/images/MyCrafts/leather-wallet.webp",
                    UserID = 2
                },
                new Product()
                {
                    Name = "Metal Windchime",
                    Price = 420.00,
                    Availability = 10,
                    Description =
                        "It makes a jingling sound when the wind blows, your neighbours will love it.",
                    Category = "Crafts",
                    ImageURL = "/images/MyCrafts/metal-windchime.webp",
                    UserID = 2
                },
                new Product()
                {
                    Name = "Wood Bowl",
                    Price = 350.00,
                    Availability = 10,
                    Description = "Perfect for putting stuff inside of.",
                    Category = "Crafts",
                    ImageURL = "/images/MyCrafts/wood-bowl.webp",
                    UserID = 2
                },
                new Product()
                {
                    Name = "Wood Chopping Board",
                    Price = 550.00,
                    Availability = 10,
                    Description =
                        "This is sort of like a helmet, but it's for your kitchen counter.",
                    Category = "Crafts",
                    ImageURL = "/images/MyCrafts/wood-chopping-board.webp",
                    UserID = 2
                },
                new Product()
                {
                    Name = "Paper Lantern",
                    Price = 770.00,
                    Availability = 10,
                    Description =
                        "It's light because it's made of paper, helps to see when it's dark.",
                    Category = "Crafts",
                    ImageURL = "/images/MyCrafts/paper-lantern.webp",
                    UserID = 2
                },
                new Product()
                {
                    Name = "Glass Vase",
                    Price = 1337.00,
                    Availability = 10,
                    Description =
                        "This is where the flowers go when you want them inside the house.",
                    Category = "Crafts",
                    ImageURL = "/images/MyCrafts/glass-vase.webp",
                    UserID = 2
                },
                new Product()
                {
                    Name = "Woolie Blanket",
                    Price = 2500.00,
                    Availability = 10,
                    Description =
                        "A warm and cosy blanket made from 100% real wool sourced 100% from real handcrafted jerseys.",
                    Category = "Crafts",
                    ImageURL = "/images/MyCrafts/woolie-blanket.webp",
                    UserID = 2
                },
                new Product()
                {
                    Name = "Wooden Birdhouse",
                    Price = 480.00,
                    Availability = 10,
                    Description = "Give your feathered friends a cozy birdhouse cottage!",
                    Category = "Crafts",
                    ImageURL = "/images/MyCrafts/wood-birdhouse.webp",
                    UserID = 2
                },
                new Product()
                {
                    Name = "Rustic Woven Rug",
                    Price = 3000.00,
                    Availability = 10,
                    Description =
                        "A cozy woven rug, this item feels great to put on the floor and step on.",
                    Category = "Crafts",
                    ImageURL = "/images/MyCrafts/rustic-rug.webp",
                    UserID = 2
                },
                new Product()
                {
                    Name = "Candle Holders",
                    Price = 9001.00,
                    Availability = 10,
                    Description =
                        "Handcrafted from 100% iron, this cutting-edge candle technology was developed specifically to ensure candles do not ever need to fall over again.",
                    Category = "Crafts",
                    ImageURL = "/images/MyCrafts/iron-candle-holders.webp",
                    UserID = 2
                },
                new Product()
                {
                    Name = "Necklace",
                    Price = 2700.00,
                    Availability = 10,
                    Description =
                        "This is a necklace, but I think you might actually be able to put stuff inside of it so.",
                    Category = "Crafts",
                    ImageURL = "/images/MyCrafts/necklace-maybe.webp",
                    UserID = 2
                }
            ];
        }

        static Order[] SeedOrders()
        {
            return
            [
                new Order()
                {
                    UserID = 1,
                    ProductID = 1,
                    Quantity = 1,
                    Address = "fake",
                    Processed = true
                },
                new Order()
                {
                    UserID = 1,
                    ProductID = 2,
                    Quantity = 1,
                    Address = "fake",
                    Processed = true
                },
                new Order()
                {
                    UserID = 1,
                    ProductID = 3,
                    Quantity = 1,
                    Address = "fake",
                    Processed = true
                },
                new Order()
                {
                    UserID = 1,
                    ProductID = 4,
                    Quantity = 1,
                    Address = "fake",
                    Processed = true
                }
            ];
        }
    }
}
