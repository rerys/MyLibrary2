using MySql.Data.EntityFramework;
using System;
using System.Linq;
using System.Data.Entity;
using System.Collections.Generic;

namespace prbd_1819_g07
{
    public enum DbType { MsSQL, MySQL }
    public enum EFDatabaseInitMode { CreateIfNotExists, DropCreateIfChanges, DropCreateAlways }

    [DbConfigurationType(typeof(MySqlEFConfiguration))]
    public class MySqlModel : Model
    {
        public MySqlModel(EFDatabaseInitMode initMode) : base("name=library-mysql")
        {
            switch (initMode)
            {
                case EFDatabaseInitMode.CreateIfNotExists:
                    Database.SetInitializer<MySqlModel>(new CreateDatabaseIfNotExists<MySqlModel>());
                    break;
                case EFDatabaseInitMode.DropCreateIfChanges:
                    Database.SetInitializer<MySqlModel>(new DropCreateDatabaseIfModelChanges<MySqlModel>());
                    break;
                case EFDatabaseInitMode.DropCreateAlways:
                    Database.SetInitializer<MySqlModel>(new DropCreateDatabaseAlways<MySqlModel>());
                    break;
            }
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // see: https://blog.craigtp.co.uk/Post/2017/04/05/Entity_Framework_with_MySQL_-_Booleans,_Bits_and_%22String_was_not_recognized_as_a_valid_boolean%22_errors.
            modelBuilder.Properties<bool>().Configure(c => c.HasColumnType("bit"));
        }

        public override void Reseed(string tableName)
        {
            Database.ExecuteSqlCommand($"ALTER TABLE {tableName} AUTO_INCREMENT=1");
        }
    }

    public class MsSqlModel : Model
    {
        public MsSqlModel(EFDatabaseInitMode initMode) : base("name=library-mssql")
        {
            switch (initMode)
            {
                case EFDatabaseInitMode.CreateIfNotExists:
                    Database.SetInitializer<MsSqlModel>(new CreateDatabaseIfNotExists<MsSqlModel>());
                    break;
                case EFDatabaseInitMode.DropCreateIfChanges:
                    Database.SetInitializer<MsSqlModel>(new DropCreateDatabaseIfModelChanges<MsSqlModel>());
                    break;
                case EFDatabaseInitMode.DropCreateAlways:
                    Database.SetInitializer<MsSqlModel>(new DropCreateDatabaseAlways<MsSqlModel>());
                    break;
            }
        }

        public override void Reseed(string tableName)
        {
            Database.ExecuteSqlCommand($"DBCC CHECKIDENT('{tableName}', RESEED, 0)");
        }
    }

    public abstract class Model : DbContext
    {
        protected Model(string name) : base(name) { }

        public static Model CreateModel(DbType type, EFDatabaseInitMode initMode = EFDatabaseInitMode.DropCreateIfChanges)
        {
            switch (type)
            {
                case DbType.MsSQL:
                    return new MsSqlModel(initMode);
                case DbType.MySQL:
                    return new MySqlModel(initMode);
                default:
                    throw new ApplicationException("Undefined database type");
            }
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<Rental> Rentals { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<RentalItem> RentalItems { get; set; }
        public DbSet<BookCopy> BookCopies { get; set; }

        public void ClearDatabase()
        {

            Users.RemoveRange(Users);
            Books.RemoveRange(Books);
            Categories.RemoveRange(Categories);
            
            SaveChanges();

            Reseed(nameof(Categories));
            Reseed(nameof(Books));
            Reseed(nameof(BookCopies));
            Reseed(nameof(RentalItems));
            Reseed(nameof(Rentals));
            Reseed(nameof(Users));
        }

        public abstract void Reseed(string tableName);

        public void CreateTestData()
        {
            Console.WriteLine("Creating test data... ");
            // création de quelques users
            var member = CreateUser("member", "member", "member", "member@epfc.eu");
            var manager = CreateUser("manager", "manager", "manager", "manager@epfc.eu", null, Role.Manager);
            var admin = CreateUser("admin", "admin", "admin", "admin@epfc.eu", null, Role.Admin);

            //création de quelques books
            var book1 = CreateBook("1234567891234", "title1", "autor1", "editor1");
            var book2 = CreateBook("2234567891234", "title2", "autor2", "editor2");
            var book3 = CreateBook("3234567891234", "title3", "autor3", "editor3");


            //création de quelques categories
            var category1 = CreateCategory("category1");
            var category2 = CreateCategory("category2");
            var category3 = CreateCategory("category3");
        }
        public User CreateUser(string userName, string password, string fullName, string email, DateTime? birthDate = null, Role role = Role.Member)
        {
            var user = Users.Create();
            user.UserName = userName;
            user.Password = password;
            user.FullName = fullName;
            user.Email = email;
            user.BirthDate = birthDate;
            user.Role = role;
            Users.Add( user );
            SaveChanges();
            return user;
        }

        public Book CreateBook(string isbn, string title, string author, string editor, int numCopies = 1)
        {
            var book = Books.Create();
            book.Isbn = isbn;
            book.Title = title;
            book.Author = author;
            book.Editor = editor;
            Books.Add(book);
            SaveChanges();
            for (int i = 0; i < numCopies; ++i)
            {
                book.CreateBookCopy(DateTime.Now,book);
            }
            return book;
        }

        public Category CreateCategory(string name)
        {
            var category = Categories.Create();
            category.Name = name;
            Categories.Add(category);
            SaveChanges();
            return category;
        }

        public List<Book> FindBooksByText(string key)
        {
            return (from b in Books where b.Title.StartsWith(key) || b.Author.StartsWith(key)
                    || b.Isbn.StartsWith(key) || b.Editor.StartsWith(key)
                    select b).ToList();
        }

        public List<RentalItem> GetActiveRentalItems()
        {
            return (from r in RentalItems where r.ReturnDate == null select r).ToList();
        }

        
    }
}
