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
        public ObservableCollection<Rental> Rentals{ get; set; }


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
                RentalItems = new ObservableCollection<RentalItem>(selectedRental.Items);
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
        public ICommand ReturnBook{ get; set; }

        //Commande supprimer un livre loué. 
        public ICommand DeleteRent { get; set; }


        /******************************
         *                            *
         *   VIEW CONSTRUCTOR         * 
         *                            *
         *****************************/
        public RentalsView()
        {
            InitializeComponent();
            DataContext = this;
            Rentals = new ObservableCollection<Rental>(from r in App.Model.Rentals
                                                       where r.RentalDate != null
                                                       select r);
            ReturnBook = new RelayCommand<RentalItem>(rental =>
            {
                rental.DoReturn();
                NotifyAllFied();
            });
            DeleteRent = new RelayCommand<RentalItem>(rental =>
            {
               
            });
        }

        /******************************
         *                            *
         *   METHODE ACTION           * 
         *                            *
         *****************************/

        private void NotifyAllFied()
        {
                RentalItems = new ObservableCollection<RentalItem>(selectedRental.Items);
                RaisePropertyChanged(nameof(SelectedRental));
                RaisePropertyChanged(nameof(RentalItems));
                RaisePropertyChanged(nameof(HasRentalSelected));
        }
        public void DeleteAction()
        {

        }
    }
}
