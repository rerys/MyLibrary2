using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PRBD_Framework;

namespace prbd_1819_g07
{
    public class Category : EntityBase<Model>
    {
        protected Category() { }
        [Key]
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public virtual ICollection<Book> Books { get; set; } = new HashSet<Book>();
        [NotMapped]
        public int NumBooksCategory
        {
            get
            {
                return (from b in Model.Categories where b.CategoryId == CategoryId select b.Books).Count();
            }
        }


        public bool HasBook(Book book)
        {
            return Books.Contains(book);
        }

        public void AddBook(Book book)
        {
            Books.Add(book);
            Model.SaveChanges();
        }

        public void RemoveBook(Book book)
        {
            Books.Remove(book);
            Model.SaveChanges();
        }

        public void Delete()
        {
            Model.Categories.Remove(this);
            Model.SaveChanges();
        }

        public override string ToString()
        {
            return Name;
        }

    }
}
