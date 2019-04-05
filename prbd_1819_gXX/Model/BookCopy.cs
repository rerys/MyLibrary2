using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PRBD_Framework;

namespace prbd_1819_gXX
{
    public class BookCopy : EntityBase<Model>
    {
        protected BookCopy() { }
        [Key]
        public int BookCopyId { get; set; }
        public DateTime AcquisitionDate { get; set; }
        [NotMapped]
        public User RentedBy { get
            {
                return (from r in Model.RentalItems
                        where r.BookCopy.BookCopyId == BookCopyId && r.ReturnDate == null
                        select r.Rental.User).FirstOrDefault();
            }           
        }
        [Required]
        public virtual Book Book { get; set; }
        public virtual ICollection<RentalItem> RentalItems { get; set; } = new HashSet<RentalItem>();

        public override string ToString()
        {
            return string.Format("-----------------------\n" +
             "BookCopyId : {0}\n" +
             "AcquisitionDate : {1}\n" +
             "RentedBy : {2}\n" +
             "-----------------------\n"
                , BookCopyId, AcquisitionDate, RentedBy);
        }
    }


}
