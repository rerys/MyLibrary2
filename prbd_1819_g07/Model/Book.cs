﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PRBD_Framework;

namespace prbd_1819_g07
{
    public class Book : EntityBase<Model>
    {
        protected Book() { }
        [Key]
        public int BookId { get; set; }
        public string Isbn { get; set; }
        public string Author { get; set; }
        public string Title { get; set; }
        public string Editor { get; set; }
        public string PicturePath { get; set; }
        [NotMapped]
        public int NumAvailableCopies
        {
            get
            {
               var res =  (from c in this.Model.BookCopies
                       where c.Book.BookId == BookId &&
                       (from i in c.RentalItems where i.ReturnDate == null select i).Count() == 0
                       select c).Count();
                return res;
            }
        }

        [NotMapped]

        public string AbsolutePicturePath
        {

            get { return PicturePath != null ? App.IMAGE_PATH + "\\" + PicturePath : null; }

        }

        public virtual ICollection<BookCopy> Copies { get; set; } = new HashSet<BookCopy>();

        public virtual ICollection<Category> Categories { get; set; } = new HashSet<Category>();

        public BookCopy CreateBookCopy(DateTime aquisitionDate, Book book)
        {
            var BookCopy = Model.BookCopies.Create();
            BookCopy.AcquisitionDate = aquisitionDate;
            BookCopy.Book = book;
            Model.BookCopies.Add(BookCopy);
            Copies.Add(BookCopy);
            //Model.SaveChanges();
            return BookCopy;
        }

        public void AddCategory(Category category)
        {
            if (!Categories.Contains(category))
            {
                Categories.Add(category);
                Model.SaveChanges();
            }
        }

        public void AddCategories(Category[] cats)
        {
            foreach (var c in cats)
            {
                AddCategory(c);
            }
        }

        public void RemoveCategory(Category category)
        {
            if (Categories.Contains(category))
            {
                Categories.Remove(category);
                Model.SaveChanges();
            }
        }

        public void AddCopies(int quantity, DateTime date)
        {
            for (int i = 0; i < quantity; ++i)
            {
                Copies.Add(CreateBookCopy(date, this));
            }
            Model.SaveChanges();
        }

        public BookCopy GetAvailableCopy()
        {
            BookCopy copy;
            if (Attached)
            {
                copy = (
                    from c in this.Model.BookCopies
                     where c.Book.BookId == BookId &&
                     (from i in c.RentalItems where i.ReturnDate == null select i).Count() == 0
                     select c
                ).FirstOrDefault();
            }
            else
            {
                copy = (from c in this.Copies
                        where (from i in c.RentalItems where i.ReturnDate == null select i).Count() == 0
                        select c).FirstOrDefault();
            }
            return copy;
        }

        public void DeleteCopy(BookCopy copy)
        {
            Copies.Remove(copy);
            Model.BookCopies.Remove(copy);
            Model.SaveChanges();
        }

        public void Delete()
        {
            Model.Books.Remove(this);
          //  Model.BookCopies.Remove(this.Copies);
            Model.SaveChanges();
        }

        public override string ToString()
        {
            var s = "";
            foreach (var C in Categories)
            {
                s += C;
            }

            return string.Format("-----------------------\n" +
                "IDBook : {0} \nISBN : {1} \nAuthor : {2} \nTitle : {3} \nEditor : {4} \nCategories : {5}\n" +
                "-----------------------\n",
                            BookId, Isbn, Author, Title, Editor, s);
        }


    }
}
