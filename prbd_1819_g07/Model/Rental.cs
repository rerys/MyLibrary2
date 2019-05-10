using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PRBD_Framework;

namespace prbd_1819_g07
{
    public class Rental : EntityBase<Model>
    {
        protected Rental() { }
        [Key]
        public int RentalId { get; set; }
        public DateTime? RentalDate { get; set; }
        public int NumOpenItems
        {
            get
            {
                return (from i in Items where i.ReturnDate == null select i).Count();

                //IEnumerable<RentalItem> items = from i in App.Model.RentalItems
                //                                where i.Rental.User.UserId == User.UserId 
                //                                && i.Rental.RentalDate != null
                //                                && i.ReturnDate == null
                //                               && i.Rental.RentalId == this.RentalId
                //                                select i;
                //return items.Count();
            }
        }

        public virtual ICollection<RentalItem> Items { get; set; } = new HashSet<RentalItem>();
        [Required]
        public virtual User User { get; set; }

        public RentalItem CreateRentalItem(Rental rental, BookCopy bookCopy)
        {
            var rentalItem = Model.RentalItems.Create();
            rentalItem.BookCopy = bookCopy;
            rentalItem.Rental = rental;
            Model.RentalItems.Add(rentalItem);
            //Model.SaveChanges();
            return rentalItem;
        }

        public RentalItem RentCopy(BookCopy copy)
        {
            var rentalItem = CreateRentalItem(this, copy);
            Items.Add(rentalItem);
            //Model.SaveChanges();
            return rentalItem;

        }

        public void RemoveCopy(BookCopy copy)
        {

            var item = (from i in Items where i.BookCopy.BookCopyId == copy.BookCopyId select i).SingleOrDefault();
            if (item != null)
            {
                this.RemoveItem(item);
                Model.SaveChanges();
            }
        }

        public void RemoveItem(RentalItem item)
        {
            Model.RentalItems.Remove(item);
            Items.Remove(item);
            Model.SaveChanges();
        }

        public void Return(RentalItem item)
        {
            item.ReturnDate = DateTime.Now;
            Model.SaveChanges();
        }

        public void Confirm()
        {
            RentalDate = DateTime.Now;
            Model.SaveChanges();
        }

        public void Clear()
        {
            Model.Rentals.Remove(this);
            Model.SaveChanges();
        }

        public override string ToString()
        {
            var s = "";
            foreach (var i in Items)
            {
                s += " : " + i.RentalItemId + " ";
            }
            return string.Format("-----------------------\n" +
                "RentalId : {0}\n" +
                "RentalDate : {1}\n" +
                "Items  {2}\n" +
                "-----------------------\n"
                , RentalId, RentalDate, s);
        }
    }
}
