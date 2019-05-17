using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PRBD_Framework;

namespace prbd_1819_g07
{
    public enum Role { Member, Manager, Admin }

    public class User : EntityBase<Model>
    {
        protected User() { }
        [Key]
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public DateTime? BirthDate { get; set; }
        public Role Role { get; set; }
        public virtual ICollection<RentalItem> ActiveRentalItems { get; } = new HashSet<RentalItem>();
        public int Age { get; }
        public virtual ICollection<Rental> Rentals { get; set; } = new HashSet<Rental>();



        //Renvoie true si l'user connecté est admin.
        public bool IsAdmin
        {
            get { return this.Role == Role.Admin; }
        }

        //Renvoie true si l'user connecté n'est pas admin.
        public bool IsNotAdmin
        {
            get { return this.Role != Role.Admin; }
        }

        public Rental CreateRental()
        {
            var rental = Model.Rentals.Create();
            rental.User = this;
            Model.Rentals.Add(rental);
            Model.SaveChanges();
            return rental;
        }

        public Rental CreateBasket()
        {
            return CreateRental();
        }

        public RentalItem AddToBasket(Book book)
        {

            var basket = GetBasket();
            if (basket == null)
            {
                basket = CreateBasket();
            }
            var copy = book.GetAvailableCopy();
            if(copy != null)
            {
                var rental = basket.RentCopy(copy);
                Model.SaveChanges();
                return rental;
            }
            return null;
        }

        public void RemoveFromBasket(RentalItem item)
        {
            GetBasket().RemoveItem(item); 
        }

        public void ConfirmBasket()
        {
            GetBasket().Confirm();
        }

        public void Return(BookCopy copy)
        {
            var item = (from r in Model.RentalItems
                        where r.Rental.User.UserId == UserId
                        && r.BookCopy.BookCopyId == copy.BookCopyId
                        && r.ReturnDate == null
                        select r).FirstOrDefault();
            if(item != null)
            {
                item.DoReturn();
            }
        }

        public void ClearBasket()
        {
            GetBasket().Clear();
        }
        public Rental GetBasket()
        {
            var query = (from r in Model.Rentals
                    where r.User.UserId == UserId && r.RentalDate == null 
                    select r).FirstOrDefault();
            return query;
        }

        public Rental Basket { get => GetBasket(); }

        public override bool Validate()
        {
            ClearErrors();
            var member = App.Model.Users.Where(u => u.UserName == UserName).SingleOrDefault();
            if (string.IsNullOrEmpty(UserName))
            {
                AddError("UserName", Properties.Resources.Error_Required);
            }
            else
            {
                if (UserName.Length < 3)
                {
                    AddError("UserName", Properties.Resources.Error_LengthGreaterEqual3);
                }
            }
            if (string.IsNullOrEmpty(FullName))
            {
                AddError("FullName", Properties.Resources.Error_Required);
            }
            if (string.IsNullOrEmpty(Email))
            {
                AddError("Email", Properties.Resources.Error_Required);
            }
            RaiseErrors();
            return !HasErrors;
        }
    }
}
