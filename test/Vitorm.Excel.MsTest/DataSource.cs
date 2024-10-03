﻿namespace Vitorm.MsTest
{
    using DbContext = Vitorm.Excel.DbContext;

    [System.ComponentModel.DataAnnotations.Schema.Table("User")]
    public class User
    {
        public User() { }

        public User(int id) { this.id = id; }
        public User(string name) { this.name = name; }


        [System.ComponentModel.DataAnnotations.Key]
        [System.ComponentModel.DataAnnotations.Schema.Column("userId")]
        [System.ComponentModel.DataAnnotations.Schema.DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        [System.ComponentModel.DataAnnotations.Schema.Column("userName")]
        public string name { get; set; }
        [System.ComponentModel.DataAnnotations.Schema.Column("userBirth")]
        public DateTime? birth { get; set; }
        [System.ComponentModel.DataAnnotations.Schema.Column("userFatherId")]
        public int? fatherId { get; set; }
        [System.ComponentModel.DataAnnotations.Schema.Column("userMotherId")]
        public int? motherId { get; set; }
        [System.ComponentModel.DataAnnotations.Schema.Column("userClassId")]
        public int? classId { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.NotMapped]
        public string test { get; set; }

        public static User NewUser(int id, bool forAdd = false) => new User { id = forAdd ? 0 : id, name = "testUser" + id };

        public static List<User> NewUsers(int startId, int count = 1, bool forAdd = false)
        {
            return Enumerable.Range(startId, count).Select(id => NewUser(id, forAdd)).ToList();
        }
    }

    [System.ComponentModel.DataAnnotations.Schema.Table("UserClass")]
    public class UserClass
    {
        [System.ComponentModel.DataAnnotations.Key]
        [System.ComponentModel.DataAnnotations.Schema.Column("classId")]
        [System.ComponentModel.DataAnnotations.Schema.DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        [System.ComponentModel.DataAnnotations.Schema.Column("className")]
        public string name { get; set; }

        public static List<UserClass> NewClasses(int startId, int count = 1)
        {
            return Enumerable.Range(startId, count).Select(id => new UserClass { id = 0, name = "class" + id }).ToList();
        }
    }


    public class DataSource
    {
        public static void WaitForUpdate() { }

        public static DbContext CreateDbContextForWriting(bool autoInit = true) => CreateDbContext(autoInit);
        public static DbContext CreateDbContext(bool autoInit = true)
        {
            var guid = Guid.NewGuid().ToString();
            var filePath = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, $"{guid}.xlsx");


            var dbContext = new Vitorm.Excel.DbContext(filePath);



            if (autoInit)
                InitDbContext(dbContext);

            return dbContext;
        }


        public static void InitDbContext(DbContext dbContext)
        {
            #region #1 init User
            {
                dbContext.TryDropTable<User>();
                dbContext.TryCreateTable<User>();

                var users = new List<User> {
                    new User { name="u146", fatherId=4, motherId=6 },
                    new User { name="u246", fatherId=4, motherId=6 },
                    new User { name="u356", fatherId=5, motherId=6 },
                    new User { name="u400" },
                    new User { name="u500" },
                    new User { name="u600" },
                };
                dbContext.AddRange(users);

                users.ForEach(user =>
                {
                    user.birth = DateTime.Parse("2021-01-01 00:00:00").AddHours(user.id);
                    user.classId = user.id % 2 + 1;
                });

                dbContext.UpdateRange(users);
            }
            #endregion

            #region #2 init Class
            {
                dbContext.TryDropTable<UserClass>();
                dbContext.TryCreateTable<UserClass>();
                dbContext.AddRange(UserClass.NewClasses(1, 6));
            }
            #endregion
        }

    }
}
