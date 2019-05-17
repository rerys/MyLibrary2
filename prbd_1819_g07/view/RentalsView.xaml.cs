using PRBD_Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static prbd_1819_g07.App;

namespace prbd_1819_g07
{
    /// <summary>
    /// Logique d'interaction pour RentalsView.xaml
    /// </summary>
    public partial class RentalsView : UserControlBase
    {


        /******************************
         *                            *
         *   PROPERTIES               * 
         *                            *
         *****************************/


        //Propriété de la liste des rentals. 
        public ObservableCollection<Rental> Rentals { get; set; }


        //Propriété de la liste des rentalitems.
        public ObservableCollection<RentalItem> RentalItems { get; set; }

        //Propriété du rental selectionné, refresh la liste de rentalItem.
        private Rental selectedRental;
        public Rental SelectedRental
        {
            get { return selectedRental; }
            set
            {
                selectedRental = value;
                if (selectedRental == null)
                {
                    RentalItems = null;
                }
                else
                {
                    RentalItems = new ObservableCollection<RentalItem>(selectedRental.Items);
                }
                RaisePropertyChanged(nameof(SelectedRental));
                RaisePropertyChanged(nameof(RentalItems));
                RaisePropertyChanged(nameof(HasRentalSelected));
            }
        }

        //Renvoie true si un rental a été selectionné. 
        public bool HasRentalSelected
        {
            get { return selectedRental != null; }
        }

        /******************************
         *                            *
         *   ICOMMAND                 * 
         *                            *
         *****************************/


        //Commande pour retourner un book.
        public ICommand ReturnBook { get; set; }

        //Commande supprimer un livre loué. 
        public ICommand DeleteRent { get; set; }

        public ICommand CancelReturnBook { get; set; }


        /******************************
         *                            *
         *   VIEW CONSTRUCTOR         * 
         *                            *
         *****************************/

        public RentalsView()
        {
            InitializeComponent();
            DataContext = this;
            NotifyAllFied();
            ReturnBook = new RelayCommand<RentalItem>(rental => { ReturnBookAction(rental); });//Un peu débile mais pas eu le choix.
            CancelReturnBook = new RelayCommand<RentalItem>(rental => { CancelReturnBookAction(rental); });
            DeleteRent = new RelayCommand<RentalItem>(item => { DeleteRentAction(item); });

            App.Register(this, AppMessages.MSG_RENTAL_CHANGED, () => { NotifyAllFied(); });
        }



        /*************************************************
         *                                               *
         *   METHODE D'ACTIVATION DES BOUTONS            * 
         *                                               *
         *************************************************/


        /******************************
         *                            *
         *   METHODE ACTION           * 
         *                            *
         *****************************/

        private void CancelReturnBookAction(RentalItem rental)
        {
            rental.CancelReturn();
            NotifyAllFied();
            App.NotifyColleagues(AppMessages.MSG_RENTAL_CHANGED);
        }

        //Méthode d'action pour le bouton returnbook
        private void ReturnBookAction(RentalItem rental)
        {

            rental.DoReturn();
            NotifyAllFied();
            App.NotifyColleagues(AppMessages.MSG_RENTAL_CHANGED);

        }

        //Méthode d'action pour le bouton deleterent
        private void DeleteRentAction(RentalItem item)
        {
            SelectedRental.RemoveItem(item);
            NotifyAllFied();
            App.NotifyColleagues(AppMessages.MSG_RENTAL_CHANGED);
        }

        //Méthode qui notitife tous les champs et met à jours liste. 
        private void NotifyAllFied()
        {
            if (App.CurrentUser.Role != Role.Admin)
            {
                Rentals = new ObservableCollection<Rental>(from r in App.Model.Rentals
                                                           where r.RentalDate != null
                                                           && App.CurrentUser.UserId == r.User.UserId
                                                           select r);
            }
            else
            {
                Rentals = new ObservableCollection<Rental>(from r in App.Model.Rentals
                                                           where r.RentalDate != null
                                                           select r);

            }

            if (HasRentalSelected)
            {
                RentalItems = new ObservableCollection<RentalItem>(selectedRental.Items);
            }
            RaisePropertyChanged(nameof(SelectedRental));
            RaisePropertyChanged(nameof(Rentals));
            RaisePropertyChanged(nameof(RentalItems));
            RaisePropertyChanged(nameof(HasRentalSelected));
        }

    }
}
