using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PRBD_Framework;


namespace prbd_1819_gXX
{
    public class RentalItem : EntityBase<Model>
    {
        protected RentalItem() { }
        [Key]
        public int RentalItemId { get; set; }
        public DateTime? ReturnDate { get; set; }
        [Required]
        public virtual Rental Rental { get; set; }
        [Required]
        public virtual BookCopy BookCopy { get; set; }


        public void DoReturn()
        {
            ReturnDate = DateTime.Now;
            Model.SaveChanges();
        }

        public void CancelReturn()
        {
            ReturnDate = null;
            Model.SaveChanges();
        }

        public override string ToString()
        {
            return string.Format("-----------------------\n" +
                "RentalItemId : {0}\n" +
                "ReturnDate : {1}\n" +
                "Rental : {2}\n" +
                "BookCopy {3}\n"
                , RentalItemId, ReturnDate, Rental.RentalId, BookCopy.BookCopyId);
        }
    }
}
