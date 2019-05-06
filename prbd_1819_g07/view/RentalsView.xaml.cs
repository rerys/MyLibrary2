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

        private ObservableCollection<Rental> rentals;
        public ObservableCollection<Rental> Rentals
        {
            get { return rentals; }
            set
            {
                rentals = value;
                RaisePropertyChanged(nameof(Rentals));
                //RaisePropertyChanged(nameof(UsersListView));
            }
        }

        private ObservableCollection<RentalItem> rentalItems;
        public ObservableCollection<RentalItem> RentalItems
        {
            get { return rentalItems; }
            set
            {
                rentalItems = value;
                RaisePropertyChanged(nameof(RentalItems));
                //RaisePropertyChanged(nameof(UsersListView));
            }
        }


        public RentalsView()
        {
            InitializeComponent();
            DataContext = this;
            Rentals = new ObservableCollection<Rental>(App.Model.Rentals);
            RentalItems = new ObservableCollection<RentalItem>(App.Model.RentalItems);
            NotifyAllField();
        }

        private void NotifyAllField()
        {
            RaisePropertyChanged(nameof(Rentals));
            RaisePropertyChanged(nameof(RentalItems));
        }
    }
}
